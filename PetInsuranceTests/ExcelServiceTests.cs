using System;
using System.IO;
using System.Threading.Tasks;
using ClosedXML.Excel;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PetInsurance.Data;
using PetInsurance.Models;
using PetInsurance.Services;

namespace PetInsuranceTests;

[TestFixture]
public class ExcelServiceTests
{
    private SqliteConnection _connection = null!;
    private AppDbContext _context = null!;
    private ExcelService _excelService = null!;

    [SetUp]
    public async Task Setup()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        await _connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AppDbContext(options);
        await _context.Database.EnsureCreatedAsync();

        // Seed customer, pet, quote, coverage
        var customer = new Customer
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Phone = "555-0199",
            ZipCode = "90210"
        };
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        var pet = new Pet
        {
            CustomerId = customer.CustomerId,
            PetName = "Buddy",
            Species = "Dog",
            Breed = "Golden Retriever",
            DateOfBirth = new DateOnly(2020, 5, 10),
            Gender = "Male",
            HasPreExistingCondition = true
        };
        _context.Pets.Add(pet);
        await _context.SaveChangesAsync();

        var quote = new Quote
        {
            QuoteNumber = "QT-10000001",
            CustomerId = customer.CustomerId,
            PetId = pet.PetId,
            CreatedDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(30),
            Status = QuoteStatus.Active,
            BasePremium = 30m,
            AgeAdjustment = 6m,
            WellnessAmount = 15m,
            DiscountAmount = 0m,
            FinalPremium = 51m
        };
        _context.Quotes.Add(quote);
        await _context.SaveChangesAsync();

        var coverage = new QuoteCoverage
        {
            QuoteId = quote.QuoteId,
            AnnualLimit = 5000m,
            Deductible = 250m,
            ReimbursementPct = 0.80m,
            Wellness = true
        };
        _context.QuoteCoverages.Add(coverage);
        await _context.SaveChangesAsync();

        _excelService = new ExcelService(_context);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _context.DisposeAsync();
        await _connection.CloseAsync();
        await _connection.DisposeAsync();
    }

    [Test]
    public async Task ExportQuotesToExcelAsync_ExportsCompleteQuoteAndPetData()
    {
        // Act
        byte[] bytes = await _excelService.ExportQuotesToExcelAsync();

        // Assert
        bytes.Should().NotBeNullOrEmpty();

        using var ms = new MemoryStream(bytes);
        using var workbook = new XLWorkbook(ms);

        workbook.Worksheets.Contains("Customers").Should().BeTrue();
        workbook.Worksheets.Contains("Pets").Should().BeTrue();
        workbook.Worksheets.Contains("Quotes").Should().BeTrue();
        workbook.Worksheets.Contains("QuoteCoverages").Should().BeTrue();

        // Verify Pets sheet has complete data (Gender & HasPreExistingCondition)
        var wsPets = workbook.Worksheet("Pets");
        wsPets.Cell(1, 7).GetValue<string>().Should().Be("Gender");
        wsPets.Cell(1, 8).GetValue<string>().Should().Be("HasPreExistingCondition");
        wsPets.Cell(2, 3).GetValue<string>().Should().Be("Buddy");
        wsPets.Cell(2, 7).GetValue<string>().Should().Be("Male");
        wsPets.Cell(2, 8).GetValue<bool>().Should().BeTrue();

        // Verify Quotes sheet has complete data (Pet, Customer, Breakdown, Premium, Coverage)
        var wsQuotes = workbook.Worksheet("Quotes");
        wsQuotes.Cell(1, 4).GetValue<string>().Should().Be("CustomerName");
        wsQuotes.Cell(1, 7).GetValue<string>().Should().Be("PetName");
        wsQuotes.Cell(1, 16).GetValue<string>().Should().Be("BasePremium");
        wsQuotes.Cell(1, 20).GetValue<string>().Should().Be("FinalPremium");

        wsQuotes.Cell(2, 4).GetValue<string>().Should().Be("John Doe");
        wsQuotes.Cell(2, 7).GetValue<string>().Should().Be("Buddy");
        wsQuotes.Cell(2, 16).GetValue<decimal>().Should().Be(30m);
        wsQuotes.Cell(2, 20).GetValue<decimal>().Should().Be(51m);
    }

    [Test]
    public async Task ImportQuotesFromExcelAsync_ImportsCompleteDataCorrectly()
    {
        // Export existing context to Excel stream
        byte[] bytes = await _excelService.ExportQuotesToExcelAsync();

        // Create fresh SQLite In-Memory DbContext
        using var newConn = new SqliteConnection("Filename=:memory:");
        await newConn.OpenAsync();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(newConn)
            .Options;

        using var newContext = new AppDbContext(options);
        await newContext.Database.EnsureCreatedAsync();

        var newExcelService = new ExcelService(newContext);

        using var stream = new MemoryStream(bytes);

        // Act
        var (importedQuotes, importedCustomers, importedPets) = await newExcelService.ImportQuotesFromExcelAsync(stream);

        // Assert
        importedCustomers.Should().Be(1);
        importedPets.Should().Be(1);
        importedQuotes.Should().Be(1);

        var importedPet = await newContext.Pets.FirstOrDefaultAsync(p => p.PetName == "Buddy");
        importedPet.Should().NotBeNull();
        importedPet!.Gender.Should().Be("Male");
        importedPet.HasPreExistingCondition.Should().BeTrue();
    }
}
