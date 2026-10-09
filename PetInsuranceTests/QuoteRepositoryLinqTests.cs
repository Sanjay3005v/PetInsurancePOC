using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PetInsurance.Data;
using PetInsurance.DTOs;
using PetInsurance.Models;
using PetInsurance.Repositories;

namespace PetInsuranceTests;

[TestFixture]
public class QuoteRepositoryLinqTests
{
    private SqliteConnection _connection = null!;
    private DbContextOptions<AppDbContext> _dbContextOptions = null!;
    private AppDbContext _context = null!;
    private QuoteRepository _repository = null!;

    [SetUp]
    public async Task Setup()
    {
        // Open an in-memory SQLite connection so it persists for the duration of the test
        _connection = new SqliteConnection("Filename=:memory:");
        await _connection.OpenAsync();

        _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AppDbContext(_dbContextOptions);
        await _context.Database.EnsureCreatedAsync();

        _repository = new QuoteRepository(_context);

        // Seed SQLite In-Memory Data
        await SeedTestDataAsync();
    }

    [TearDown]
    public async Task TearDown()
    {
        await _context.DisposeAsync();
        await _connection.CloseAsync();
        await _connection.DisposeAsync();
    }

    private async Task SeedTestDataAsync()
    {
        var cust1 = new Customer { FirstName = "John", LastName = "Doe", Email = "john@example.com", Phone = "555-1111", ZipCode = "10001" };
        var cust2 = new Customer { FirstName = "Jane", LastName = "Smith", Email = "jane@example.com", Phone = "555-2222", ZipCode = "90210" };

        await _context.Customers.AddRangeAsync(cust1, cust2);
        await _context.SaveChangesAsync();

        var pet1 = new Pet { CustomerId = cust1.CustomerId, PetName = "Max", Species = "Dog", Breed = "Beagle", DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-2)), Gender = "Male" };
        var pet2 = new Pet { CustomerId = cust2.CustomerId, PetName = "Luna", Species = "Cat", Breed = "Persian", DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-5)), Gender = "Female" };

        await _context.Pets.AddRangeAsync(pet1, pet2);
        await _context.SaveChangesAsync();

        var q1 = new Quote { QuoteNumber = "QT-001", CustomerId = cust1.CustomerId, PetId = pet1.PetId, CreatedDate = DateTime.UtcNow.AddDays(-10), ExpiryDate = DateTime.UtcNow.AddYears(1), Status = QuoteStatus.Active, FinalPremium = 100.00m };
        var q2 = new Quote { QuoteNumber = "QT-002", CustomerId = cust2.CustomerId, PetId = pet2.PetId, CreatedDate = DateTime.UtcNow.AddDays(-5), ExpiryDate = DateTime.UtcNow.AddYears(1), Status = QuoteStatus.Converted, FinalPremium = 200.00m };
        var q3 = new Quote { QuoteNumber = "QT-003", CustomerId = cust1.CustomerId, PetId = pet1.PetId, CreatedDate = DateTime.UtcNow.AddDays(-1), ExpiryDate = DateTime.UtcNow.AddYears(1), Status = QuoteStatus.Active, FinalPremium = 300.00m };

        await _context.Quotes.AddRangeAsync(q1, q2, q3);
        await _context.SaveChangesAsync();
    }

    [Test]
    public async Task GetDashboardMetricsAsync_CalculatesAveragePremiumAndGroupsByStatus()
    {
        // Act (Executes LINQ Average and GroupBy queries)
        var metrics = await _repository.GetDashboardMetricsAsync();

        // Assert: Premiums are 100, 200, 300 -> Average = 200
        metrics.TotalQuotes.Should().Be(3);
        metrics.ActiveQuotes.Should().Be(2);
        metrics.ConvertedQuotes.Should().Be(1);
        metrics.AveragePremium.Should().Be(200.00m);
        metrics.QuotesByStatus["Active"].Should().Be(2);
        metrics.QuotesByStatus["Converted"].Should().Be(1);
    }

    [Test]
    public async Task SearchQuotesAsync_FilterBySpeciesAndPageResults_ReturnsFilteredQuotes()
    {
        // Arrange: Search Dog species with page size 1
        var dto = new SearchQuoteDto
        {
            Species = "Dog",
            PageNumber = 1,
            PageSize = 1
        };

        // Act (Executes LINQ Where, OrderByDescending, Skip, Take)
        var (items, totalCount) = await _repository.SearchQuotesAsync(dto);

        // Assert: 2 Dog quotes exist (q1 and q3). Paging returns 1 item, newest first (q3 with FinalPremium = 300).
        totalCount.Should().Be(2);
        items.Should().HaveCount(1);
        items[0].QuoteNumber.Should().Be("QT-003");
    }

    [Test]
    public async Task HasDuplicateActiveQuoteAsync_ExistingActiveQuote_ReturnsTrue()
    {
        // Arrange
        var q = await _context.Quotes.FirstAsync(q => q.QuoteNumber == "QT-001");
        var coverage = new QuoteCoverage { QuoteId = q.QuoteId, AnnualLimit = 5000, Deductible = 250, ReimbursementPct = 80, Wellness = false };
        await _context.QuoteCoverages.AddAsync(coverage);
        await _context.SaveChangesAsync();

        // Act (Executes LINQ Any)
        bool exists = await _repository.HasDuplicateActiveQuoteAsync(q.CustomerId, q.PetId, 5000, 250, 80, false);

        // Assert
        exists.Should().BeTrue();
    }
}
