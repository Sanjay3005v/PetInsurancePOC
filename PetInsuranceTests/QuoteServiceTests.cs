using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using PetInsurance.DTOs;
using PetInsurance.Exceptions;
using PetInsurance.Models;
using PetInsurance.Repositories.Interfaces;
using PetInsurance.Services;
using PetInsurance.Services.Interfaces;
using PetInsurance.Settings;

namespace PetInsuranceTests;

[TestFixture]
public class QuoteServiceTests
{
    private Mock<ICustomerRepository> _mockCustomerRepo = null!;
    private Mock<IPetRepository> _mockPetRepo = null!;
    private Mock<IQuoteRepository> _mockQuoteRepo = null!;
    private Mock<IQuoteCoverageRepository> _mockCoverageRepo = null!;
    private Mock<IPremiumCalculatorService> _mockCalculator = null!;
    private Mock<ILogger<QuoteService>> _mockLogger = null!;
    private IOptions<PremiumSettings> _settings = null!;
    private QuoteService _quoteService = null!;

    [SetUp]
    public void Setup()
    {
        _mockCustomerRepo = new Mock<ICustomerRepository>();
        _mockPetRepo = new Mock<IPetRepository>();
        _mockQuoteRepo = new Mock<IQuoteRepository>();
        _mockCoverageRepo = new Mock<IQuoteCoverageRepository>();
        _mockCalculator = new Mock<IPremiumCalculatorService>();
        _mockLogger = new Mock<ILogger<QuoteService>>();

        _settings = Options.Create(new PremiumSettings
        {
            BasePremium = 50.00m,
            SeniorPetAge = 7,
            SeniorPetAdjustment = 20.00m,
            WellnessAmount = 15.00m,
            DiscountAmount = 10.00m,
            MaxEligibleAge = 15
        });

        _quoteService = new QuoteService(
            _mockCustomerRepo.Object,
            _mockPetRepo.Object,
            _mockQuoteRepo.Object,
            _mockCoverageRepo.Object,
            _mockCalculator.Object,
            _settings,
            _mockLogger.Object);
    }

    [Test]
    public async Task CreateQuoteAsync_ValidInput_CalculatesPremiumAndSets30DayExpiry()
    {
        // Arrange
        var dto = new CreateQuoteDto
        {
            FirstName = "Alice",
            LastName = "Smith",
            Email = "alice@example.com",
            Phone = "555-1234",
            ZipCode = "12345",
            PetName = "Max",
            Species = "Dog",
            Breed = "Labrador",
            DateOfBirth = DateTime.Today.AddYears(-3),
            Gender = "Male",
            HasPreExistingCondition = false,
            AnnualLimit = 5000,
            Deductible = 250,
            ReimbursementPct = 80,
            Wellness = true
        };

        var customer = new Customer { CustomerId = 1, Email = dto.Email };
        var pet = new Pet { PetId = 10, CustomerId = 1, PetName = dto.PetName };

        _mockCustomerRepo.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(customer);
        _mockPetRepo.Setup(r => r.GetByCustomerAndNameAsync(1, dto.PetName)).ReturnsAsync(pet);
        _mockQuoteRepo.Setup(r => r.HasDuplicateActiveQuoteAsync(1, 10, 5000, 250, 80, true)).ReturnsAsync(false);
        _mockCalculator.Setup(c => c.CalculatePremium(3, true, false)).Returns((50, 0, 15, 0, 65));

        // Act
        int quoteId = await _quoteService.CreateQuoteAsync(dto);

        // Assert
        _mockQuoteRepo.Verify(r => r.AddAsync(It.Is<Quote>(q =>
            q.Status == QuoteStatus.Active &&
            q.FinalPremium == 65 &&
            (q.ExpiryDate - q.CreatedDate).TotalDays >= 29)), Times.Once);

        _mockQuoteRepo.Verify(r => r.SaveChangesAsync(), Times.AtLeastOnce);
    }

    [Test]
    public void CreateQuoteAsync_ZeroOrNegativeAge_ThrowsBusinessRuleException()
    {
        // Arrange
        var dto = new CreateQuoteDto
        {
            Email = "test@example.com",
            PetName = "Puppy",
            DateOfBirth = DateTime.Today // Age 0
        };

        // Act
        Func<Task> act = async () => await _quoteService.CreateQuoteAsync(dto);

        // Assert
        act.Should().ThrowAsync<BusinessRuleException>().WithMessage("*Pet age must be greater than zero*");
    }

    [Test]
    public void CreateQuoteAsync_DuplicateActiveQuote_ThrowsBusinessRuleException()
    {
        // Arrange
        var dto = new CreateQuoteDto
        {
            Email = "dup@example.com",
            PetName = "Rover",
            DateOfBirth = DateTime.Today.AddYears(-2),
            AnnualLimit = 5000,
            Deductible = 250,
            ReimbursementPct = 80,
            Wellness = false
        };

        var customer = new Customer { CustomerId = 2, Email = dto.Email };
        var pet = new Pet { PetId = 20, CustomerId = 2, PetName = dto.PetName };

        _mockCustomerRepo.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(customer);
        _mockPetRepo.Setup(r => r.GetByCustomerAndNameAsync(2, dto.PetName)).ReturnsAsync(pet);
        _mockQuoteRepo.Setup(r => r.HasDuplicateActiveQuoteAsync(2, 20, 5000, 250, 80, false)).ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _quoteService.CreateQuoteAsync(dto);

        // Assert
        act.Should().ThrowAsync<BusinessRuleException>().WithMessage("*Duplicate active quote exists*");
    }

    [Test]
    public async Task ConvertQuoteAsync_ActiveQuote_ConvertsSuccessfully()
    {
        // Arrange
        var quote = new Quote { QuoteId = 5, Status = QuoteStatus.Active };
        _mockQuoteRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(quote);

        // Act
        bool result = await _quoteService.ConvertQuoteAsync(5);

        // Assert
        result.Should().BeTrue();
        quote.Status.Should().Be(QuoteStatus.Converted);
        _mockQuoteRepo.Verify(r => r.Update(quote), Times.Once);
        _mockQuoteRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public void ConvertQuoteAsync_AlreadyConvertedOrExpired_ThrowsBusinessRuleException()
    {
        // Arrange
        var quote = new Quote { QuoteId = 6, Status = QuoteStatus.Converted };
        _mockQuoteRepo.Setup(r => r.GetByIdAsync(6)).ReturnsAsync(quote);

        // Act
        Func<Task> act = async () => await _quoteService.ConvertQuoteAsync(6);

        // Assert
        act.Should().ThrowAsync<BusinessRuleException>().WithMessage("*Quote cannot be converted*");
    }

    [Test]
    public void UpdateQuoteAsync_ExpiredQuote_ThrowsBusinessRuleException()
    {
        // Arrange
        var quote = new Quote { QuoteId = 7, Status = QuoteStatus.Expired };
        _mockQuoteRepo.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(quote);

        // Act
        Func<Task> act = async () => await _quoteService.UpdateQuoteAsync(7, new UpdateQuoteDto());

        // Assert
        act.Should().ThrowAsync<BusinessRuleException>().WithMessage("*An expired, cancelled, or converted quote cannot be modified*");
    }
}
