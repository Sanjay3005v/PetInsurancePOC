using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using PetInsurance.Data;
using PetInsurance.Exceptions;
using PetInsurance.Models;
using PetInsurance.Services.Interfaces;

namespace PetInsurance.Services;

public class ExcelService : IExcelService
{
    private readonly AppDbContext _context;

    public ExcelService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]> ExportQuotesToExcelAsync()
    {
        using var workbook = new XLWorkbook();

        // 1. Customers Sheet
        var customers = await _context.Customers.ToListAsync();
        var wsCustomers = workbook.Worksheets.Add("Customers");
        wsCustomers.Cell(1, 1).Value = "CustomerId";
        wsCustomers.Cell(1, 2).Value = "FirstName";
        wsCustomers.Cell(1, 3).Value = "LastName";
        wsCustomers.Cell(1, 4).Value = "Email";
        wsCustomers.Cell(1, 5).Value = "Phone";
        wsCustomers.Cell(1, 6).Value = "ZipCode";
        wsCustomers.Row(1).Style.Font.Bold = true;

        for (int i = 0; i < customers.Count; i++)
        {
            var c = customers[i];
            int row = i + 2;
            wsCustomers.Cell(row, 1).Value = c.CustomerId;
            wsCustomers.Cell(row, 2).Value = c.FirstName;
            wsCustomers.Cell(row, 3).Value = c.LastName;
            wsCustomers.Cell(row, 4).Value = c.Email;
            wsCustomers.Cell(row, 5).Value = c.Phone;
            wsCustomers.Cell(row, 6).Value = c.ZipCode;
        }
        wsCustomers.Columns().AdjustToContents();

        // 2. Pets Sheet
        var pets = await _context.Pets.ToListAsync();
        var wsPets = workbook.Worksheets.Add("Pets");
        wsPets.Cell(1, 1).Value = "PetId";
        wsPets.Cell(1, 2).Value = "CustomerId";
        wsPets.Cell(1, 3).Value = "PetName";
        wsPets.Cell(1, 4).Value = "Species";
        wsPets.Cell(1, 5).Value = "Breed";
        wsPets.Cell(1, 6).Value = "DateOfBirth";
        wsPets.Row(1).Style.Font.Bold = true;

        for (int i = 0; i < pets.Count; i++)
        {
            var p = pets[i];
            int row = i + 2;
            wsPets.Cell(row, 1).Value = p.PetId;
            wsPets.Cell(row, 2).Value = p.CustomerId;
            wsPets.Cell(row, 3).Value = p.PetName;
            wsPets.Cell(row, 4).Value = p.Species;
            wsPets.Cell(row, 5).Value = p.Breed;
            wsPets.Cell(row, 6).Value = p.DateOfBirth.ToString("yyyy-MM-dd");
        }
        wsPets.Columns().AdjustToContents();

        // 3. Quotes Sheet
        var quotes = await _context.Quotes.ToListAsync();
        var wsQuotes = workbook.Worksheets.Add("Quotes");
        wsQuotes.Cell(1, 1).Value = "QuoteId";
        wsQuotes.Cell(1, 2).Value = "QuoteNumber";
        wsQuotes.Cell(1, 3).Value = "CustomerId";
        wsQuotes.Cell(1, 4).Value = "PetId";
        wsQuotes.Cell(1, 5).Value = "CreatedDate";
        wsQuotes.Cell(1, 6).Value = "ExpiryDate";
        wsQuotes.Cell(1, 7).Value = "Status";
        wsQuotes.Cell(1, 8).Value = "FinalPremium";
        wsQuotes.Row(1).Style.Font.Bold = true;

        for (int i = 0; i < quotes.Count; i++)
        {
            var q = quotes[i];
            int row = i + 2;
            wsQuotes.Cell(row, 1).Value = q.QuoteId;
            wsQuotes.Cell(row, 2).Value = q.QuoteNumber;
            wsQuotes.Cell(row, 3).Value = q.CustomerId;
            wsQuotes.Cell(row, 4).Value = q.PetId;
            wsQuotes.Cell(row, 5).Value = q.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss");
            wsQuotes.Cell(row, 6).Value = q.ExpiryDate.ToString("yyyy-MM-dd HH:mm:ss");
            wsQuotes.Cell(row, 7).Value = q.Status.ToString();
            wsQuotes.Cell(row, 8).Value = q.FinalPremium;
        }
        wsQuotes.Columns().AdjustToContents();

        // 4. QuoteCoverages Sheet
        var coverages = await _context.QuoteCoverages.ToListAsync();
        var wsCoverages = workbook.Worksheets.Add("QuoteCoverages");
        wsCoverages.Cell(1, 1).Value = "QuoteCoverageId";
        wsCoverages.Cell(1, 2).Value = "QuoteId";
        wsCoverages.Cell(1, 3).Value = "AnnualLimit";
        wsCoverages.Cell(1, 4).Value = "Deductible";
        wsCoverages.Cell(1, 5).Value = "ReimbursementPct";
        wsCoverages.Cell(1, 6).Value = "Wellness";
        wsCoverages.Row(1).Style.Font.Bold = true;

        for (int i = 0; i < coverages.Count; i++)
        {
            var c = coverages[i];
            int row = i + 2;
            wsCoverages.Cell(row, 1).Value = c.QuoteCoverageId;
            wsCoverages.Cell(row, 2).Value = c.QuoteId;
            wsCoverages.Cell(row, 3).Value = c.AnnualLimit;
            wsCoverages.Cell(row, 4).Value = c.Deductible;
            wsCoverages.Cell(row, 5).Value = c.ReimbursementPct;
            wsCoverages.Cell(row, 6).Value = c.Wellness;
        }
        wsCoverages.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }

    public async Task<(int importedQuotes, int importedCustomers, int importedPets)> ImportQuotesFromExcelAsync(Stream fileStream)
    {
        using var workbook = new XLWorkbook(fileStream);

        if (!workbook.Worksheets.Contains("Customers") || !workbook.Worksheets.Contains("Quotes"))
        {
            throw new BusinessRuleException("Excel file must contain 'Customers' and 'Quotes' worksheets.");
        }

        int importedCustomers = 0;
        int importedPets = 0;
        int importedQuotes = 0;

        // Map excel old IDs to DB IDs
        var customerIdMap = new Dictionary<int, int>();
        var petIdMap = new Dictionary<int, int>();
        var quoteIdMap = new Dictionary<int, int>();

        // 1. Import Customers
        var wsCustomers = workbook.Worksheet("Customers");
        var customerRows = wsCustomers.RowsUsed().Skip(1);
        foreach (var row in customerRows)
        {
            int excelCustomerId = row.Cell(1).GetValue<int>();
            string email = row.Cell(4).GetValue<string>();

            if (string.IsNullOrWhiteSpace(email)) continue;

            var existingCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower());
            if (existingCustomer is null)
            {
                existingCustomer = new Customer
                {
                    FirstName = row.Cell(2).GetValue<string>(),
                    LastName = row.Cell(3).GetValue<string>(),
                    Email = email,
                    Phone = row.Cell(5).GetValue<string>(),
                    ZipCode = row.Cell(6).GetValue<string>()
                };
                await _context.Customers.AddAsync(existingCustomer);
                await _context.SaveChangesAsync();
                importedCustomers++;
            }
            customerIdMap[excelCustomerId] = existingCustomer.CustomerId;
        }

        // 2. Import Pets
        if (workbook.Worksheets.Contains("Pets"))
        {
            var wsPets = workbook.Worksheet("Pets");
            var petRows = wsPets.RowsUsed().Skip(1);
            foreach (var row in petRows)
            {
                int excelPetId = row.Cell(1).GetValue<int>();
                int excelCustomerId = row.Cell(2).GetValue<int>();
                string petName = row.Cell(3).GetValue<string>();

                if (string.IsNullOrWhiteSpace(petName)) continue;

                if (!customerIdMap.TryGetValue(excelCustomerId, out int dbCustomerId))
                {
                    continue;
                }

                var existingPet = await _context.Pets.FirstOrDefaultAsync(p => p.CustomerId == dbCustomerId && p.PetName.ToLower() == petName.ToLower());
                if (existingPet is null)
                {
                    DateTime dob = DateTime.TryParse(row.Cell(6).GetValue<string>(), out var parsedDob) ? parsedDob : DateTime.Today.AddYears(-2);
                    existingPet = new Pet
                    {
                        CustomerId = dbCustomerId,
                        PetName = petName,
                        Species = row.Cell(4).GetValue<string>(),
                        Breed = row.Cell(5).GetValue<string>(),
                        DateOfBirth = dob,
                        Gender = "Unknown",
                        HasPreExistingCondition = false
                    };
                    await _context.Pets.AddAsync(existingPet);
                    await _context.SaveChangesAsync();
                    importedPets++;
                }
                petIdMap[excelPetId] = existingPet.PetId;
            }
        }

        // 3. Import Quotes
        var wsQuotes = workbook.Worksheet("Quotes");
        var quoteRows = wsQuotes.RowsUsed().Skip(1);
        foreach (var row in quoteRows)
        {
            int excelQuoteId = row.Cell(1).GetValue<int>();
            string quoteNumber = row.Cell(2).GetValue<string>();
            int excelCustomerId = row.Cell(3).GetValue<int>();
            int excelPetId = row.Cell(4).GetValue<int>();

            if (string.IsNullOrWhiteSpace(quoteNumber))
            {
                quoteNumber = $"QT-{Guid.NewGuid():N}"[..10];
            }

            if (!customerIdMap.TryGetValue(excelCustomerId, out int dbCustomerId)) continue;
            if (!petIdMap.TryGetValue(excelPetId, out int dbPetId)) continue;

            var existingQuote = await _context.Quotes.FirstOrDefaultAsync(q => q.QuoteNumber == quoteNumber);
            if (existingQuote is null)
            {
                string statusStr = row.Cell(7).GetValue<string>();
                Enum.TryParse<QuoteStatus>(statusStr, true, out var status);

                existingQuote = new Quote
                {
                    QuoteNumber = quoteNumber,
                    CustomerId = dbCustomerId,
                    PetId = dbPetId,
                    CreatedDate = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddDays(30),
                    Status = status == default ? QuoteStatus.Active : status,
                    BasePremium = 50,
                    AgeAdjustment = 10,
                    WellnessAmount = 15,
                    DiscountAmount = 0,
                    FinalPremium = row.Cell(8).GetValue<decimal>()
                };
                await _context.Quotes.AddAsync(existingQuote);
                await _context.SaveChangesAsync();
                importedQuotes++;
            }
            quoteIdMap[excelQuoteId] = existingQuote.QuoteId;
        }

        // 4. Import QuoteCoverages
        if (workbook.Worksheets.Contains("QuoteCoverages"))
        {
            var wsCoverages = workbook.Worksheet("QuoteCoverages");
            var coverageRows = wsCoverages.RowsUsed().Skip(1);
            foreach (var row in coverageRows)
            {
                int excelQuoteId = row.Cell(2).GetValue<int>();

                if (!quoteIdMap.TryGetValue(excelQuoteId, out int dbQuoteId)) continue;

                var existingCoverage = await _context.QuoteCoverages.FirstOrDefaultAsync(qc => qc.QuoteId == dbQuoteId);
                if (existingCoverage is null)
                {
                    var coverage = new QuoteCoverage
                    {
                        QuoteId = dbQuoteId,
                        AnnualLimit = row.Cell(3).GetValue<decimal>(),
                        Deductible = row.Cell(4).GetValue<decimal>(),
                        ReimbursementPct = row.Cell(5).GetValue<decimal>(),
                        Wellness = row.Cell(6).GetValue<bool>()
                    };
                    await _context.QuoteCoverages.AddAsync(coverage);
                    await _context.SaveChangesAsync();
                }
            }
        }

        return (importedQuotes, importedCustomers, importedPets);
    }
}
