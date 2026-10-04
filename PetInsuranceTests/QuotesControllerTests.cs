using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using PetInsurance.Controllers;
using PetInsurance.DTOs;
using PetInsurance.Services.Interfaces;

namespace PetInsuranceTests;

[TestFixture]
public class QuotesControllerTests
{
    private Mock<IQuoteService> _mockQuoteService = null!;
    private QuotesController _controller = null!;

    [SetUp]
    public void Setup()
    {
        _mockQuoteService = new Mock<IQuoteService>();
        _controller = new QuotesController(_mockQuoteService.Object);
    }

    [Test]
    public async Task CreateQuote_ValidDto_ReturnsCreatedAtActionResult()
    {
        // Arrange
        var dto = new CreateQuoteDto { Email = "john@example.com", PetName = "Buddy" };
        _mockQuoteService.Setup(s => s.CreateQuoteAsync(dto)).ReturnsAsync(101);

        // Act
        var result = await _controller.CreateQuote(dto);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdAt = result as CreatedAtActionResult;
        createdAt!.ActionName.Should().Be(nameof(QuotesController.GetQuote));
    }

    [Test]
    public async Task GetQuote_ExistingId_ReturnsOkResultWithQuote()
    {
        // Arrange
        var details = new QuoteDetailsDto { QuoteId = 1, QuoteNumber = "QT-001", FinalPremium = 75.00m };
        _mockQuoteService.Setup(s => s.GetQuoteByIdAsync(1)).ReturnsAsync(details);

        // Act
        var result = await _controller.GetQuote(1);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(details);
    }

    [Test]
    public async Task GetQuote_MissingId_ReturnsNotFoundResult()
    {
        // Arrange
        _mockQuoteService.Setup(s => s.GetQuoteByIdAsync(999)).ReturnsAsync((QuoteDetailsDto?)null);

        // Act
        var result = await _controller.GetQuote(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Test]
    public async Task ConvertQuote_ValidId_ReturnsOkResult()
    {
        // Arrange
        _mockQuoteService.Setup(s => s.ConvertQuoteAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.ConvertQuote(1);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }
}
