using System;
using System.Collections.Generic;
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
        FormatHeaderRow(wsCustomers.Row(1), 6);

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
        wsPets.Cell(1, 7).Value = "Gender";
        wsPets.Cell(1, 8).Value = "HasPreExistingCondition";
        FormatHeaderRow(wsPets.Row(1), 8);

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
            wsPets.Cell(row, 7).Value = p.Gender;
            wsPets.Cell(row, 8).Value = p.HasPreExistingCondition;
        }
        wsPets.Columns().AdjustToContents();

        // 3. Quotes Sheet (Complete Master Data: Customer, Pet, Premium Breakdown & Coverage)
        var quotes = await _context.Quotes
            .Include(q => q.Customer)
            .Include(q => q.Pet)
            .Include(q => q.QuoteCoverage)
            .ToListAsync();

        var wsQuotes = workbook.Worksheets.Add("Quotes");
        string[] quoteHeaders = [
            "QuoteId", "QuoteNumber", "CustomerId", "CustomerName", "CustomerEmail",
            "PetId", "PetName", "Species", "Breed", "DateOfBirth", "Gender", "HasPreExistingCondition",
            "CreatedDate", "ExpiryDate", "Status",
            "BasePremium", "AgeAdjustment", "WellnessAmount", "DiscountAmount", "FinalPremium",
            "AnnualLimit", "Deductible", "ReimbursementPct", "WellnessAddon"
        ];

        for (int h = 0; h < quoteHeaders.Length; h++)
        {
            wsQuotes.Cell(1, h + 1).Value = quoteHeaders[h];
        }
        FormatHeaderRow(wsQuotes.Row(1), quoteHeaders.Length);

        for (int i = 0; i < quotes.Count; i++)
        {
            var q = quotes[i];
            int row = i + 2;
            wsQuotes.Cell(row, 1).Value = q.QuoteId;
            wsQuotes.Cell(row, 2).Value = q.QuoteNumber;
            wsQuotes.Cell(row, 3).Value = q.CustomerId;
            wsQuotes.Cell(row, 4).Value = q.Customer != null ? $"{q.Customer.FirstName} {q.Customer.LastName}".Trim() : "";
            wsQuotes.Cell(row, 5).Value = q.Customer?.Email ?? "";
            wsQuotes.Cell(row, 6).Value = q.PetId;
            wsQuotes.Cell(row, 7).Value = q.Pet?.PetName ?? "";
            wsQuotes.Cell(row, 8).Value = q.Pet?.Species ?? "";
            wsQuotes.Cell(row, 9).Value = q.Pet?.Breed ?? "";
            wsQuotes.Cell(row, 10).Value = q.Pet != null ? q.Pet.DateOfBirth.ToString("yyyy-MM-dd") : "";
            wsQuotes.Cell(row, 11).Value = q.Pet?.Gender ?? "";
            wsQuotes.Cell(row, 12).Value = q.Pet?.HasPreExistingCondition ?? false;
            wsQuotes.Cell(row, 13).Value = q.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss");
            wsQuotes.Cell(row, 14).Value = q.ExpiryDate.ToString("yyyy-MM-dd HH:mm:ss");
            wsQuotes.Cell(row, 15).Value = q.Status.ToString();

            wsQuotes.Cell(row, 16).Value = q.BasePremium;
            wsQuotes.Cell(row, 16).Style.NumberFormat.Format = "$#,##0.00";
            wsQuotes.Cell(row, 17).Value = q.AgeAdjustment;
            wsQuotes.Cell(row, 17).Style.NumberFormat.Format = "$#,##0.00";
            wsQuotes.Cell(row, 18).Value = q.WellnessAmount;
            wsQuotes.Cell(row, 18).Style.NumberFormat.Format = "$#,##0.00";
            wsQuotes.Cell(row, 19).Value = q.DiscountAmount;
            wsQuotes.Cell(row, 19).Style.NumberFormat.Format = "$#,##0.00";
            wsQuotes.Cell(row, 20).Value = q.FinalPremium;
            wsQuotes.Cell(row, 20).Style.NumberFormat.Format = "$#,##0.00";

            wsQuotes.Cell(row, 21).Value = q.QuoteCoverage?.AnnualLimit ?? 0m;
            wsQuotes.Cell(row, 21).Style.NumberFormat.Format = "$#,##0.00";
            wsQuotes.Cell(row, 22).Value = q.QuoteCoverage?.Deductible ?? 0m;
            wsQuotes.Cell(row, 22).Style.NumberFormat.Format = "$#,##0.00";
            wsQuotes.Cell(row, 23).Value = q.QuoteCoverage?.ReimbursementPct ?? 0m;
            wsQuotes.Cell(row, 23).Style.NumberFormat.Format = "0%";
            wsQuotes.Cell(row, 24).Value = q.QuoteCoverage?.Wellness ?? false;
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
        FormatHeaderRow(wsCoverages.Row(1), 6);

        for (int i = 0; i < coverages.Count; i++)
        {
            var c = coverages[i];
            int row = i + 2;
            wsCoverages.Cell(row, 1).Value = c.QuoteCoverageId;
            wsCoverages.Cell(row, 2).Value = c.QuoteId;
            wsCoverages.Cell(row, 3).Value = c.AnnualLimit;
            wsCoverages.Cell(row, 3).Style.NumberFormat.Format = "$#,##0.00";
            wsCoverages.Cell(row, 4).Value = c.Deductible;
            wsCoverages.Cell(row, 4).Style.NumberFormat.Format = "$#,##0.00";
            wsCoverages.Cell(row, 5).Value = c.ReimbursementPct;
            wsCoverages.Cell(row, 5).Style.NumberFormat.Format = "0%";
            wsCoverages.Cell(row, 6).Value = c.Wellness;
        }
        wsCoverages.Columns().AdjustToContents();

        var ms = new MemoryStream();
        workbook.SaveAs(ms);
        ms.Position = 0;
        var bytes = ms.ToArray();
        ms.Dispose();
        return bytes;
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

        // Map excel IDs to DB IDs
        var customerIdMap = new Dictionary<int, int>();
        var petIdMap = new Dictionary<int, int>();
        var quoteIdMap = new Dictionary<int, int>();

        // 1. Import Customers
        var wsCustomers = workbook.Worksheet("Customers");
        var custMap = GetColumnMap(wsCustomers);
        var customerRows = wsCustomers.RowsUsed().Skip(1);

        foreach (var row in customerRows)
        {
            int excelCustomerId = GetVal<int>(GetCell(row, custMap, "CustomerId", 1));
            string email = GetVal<string>(GetCell(row, custMap, "Email", 4))?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(email)) continue;

            var existingCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower());
            if (existingCustomer is null)
            {
                existingCustomer = new Customer
                {
                    FirstName = GetVal<string>(GetCell(row, custMap, "FirstName", 2)) ?? "",
                    LastName = GetVal<string>(GetCell(row, custMap, "LastName", 3)) ?? "",
                    Email = email,
                    Phone = GetVal<string>(GetCell(row, custMap, "Phone", 5)) ?? "",
                    ZipCode = GetVal<string>(GetCell(row, custMap, "ZipCode", 6)) ?? ""
                };
                await _context.Customers.AddAsync(existingCustomer);
                await _context.SaveChangesAsync();
                importedCustomers++;
            }
            if (excelCustomerId > 0)
            {
                customerIdMap[excelCustomerId] = existingCustomer.CustomerId;
            }
        }

        // 2. Import Pets
        if (workbook.Worksheets.Contains("Pets"))
        {
            var wsPets = workbook.Worksheet("Pets");
            var petMap = GetColumnMap(wsPets);
            var petRows = wsPets.RowsUsed().Skip(1);

            foreach (var row in petRows)
            {
                int excelPetId = GetVal<int>(GetCell(row, petMap, "PetId", 1));
                int excelCustomerId = GetVal<int>(GetCell(row, petMap, "CustomerId", 2));
                string petName = GetVal<string>(GetCell(row, petMap, "PetName", 3))?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(petName)) continue;

                if (!customerIdMap.TryGetValue(excelCustomerId, out int dbCustomerId))
                {
                    continue;
                }

                var existingPet = await _context.Pets.FirstOrDefaultAsync(p => p.CustomerId == dbCustomerId && p.PetName.ToLower() == petName.ToLower());
                if (existingPet is null)
                {
                    string dobStr = GetVal<string>(GetCell(row, petMap, "DateOfBirth", 6)) ?? GetVal<string>(GetCell(row, petMap, "DOB", 6)) ?? "";
                    DateOnly dob = DateOnly.TryParse(dobStr, out var parsedDob) ? parsedDob : DateOnly.FromDateTime(DateTime.Today).AddYears(-2);
                    string gender = GetVal<string>(GetCell(row, petMap, "Gender", 7)) ?? "Unknown";
                    bool preExisting = ParseBool(GetCell(row, petMap, "HasPreExistingCondition", 8) ?? GetCell(row, petMap, "PreExistingCondition", 8));

                    existingPet = new Pet
                    {
                        CustomerId = dbCustomerId,
                        PetName = petName,
                        Species = GetVal<string>(GetCell(row, petMap, "Species", 4)) ?? "Dog",
                        Breed = GetVal<string>(GetCell(row, petMap, "Breed", 5)) ?? "Mixed",
                        DateOfBirth = dob,
                        Gender = string.IsNullOrWhiteSpace(gender) ? "Unknown" : gender,
                        HasPreExistingCondition = preExisting
                    };
                    await _context.Pets.AddAsync(existingPet);
                    await _context.SaveChangesAsync();
                    importedPets++;
                }
                if (excelPetId > 0)
                {
                    petIdMap[excelPetId] = existingPet.PetId;
                }
            }
        }

        // 3. Import Quotes
        var wsQuotes = workbook.Worksheet("Quotes");
        var quoteMap = GetColumnMap(wsQuotes);
        var quoteRows = wsQuotes.RowsUsed().Skip(1);

        foreach (var row in quoteRows)
        {
            int excelQuoteId = GetVal<int>(GetCell(row, quoteMap, "QuoteId", 1));
            string quoteNumber = GetVal<string>(GetCell(row, quoteMap, "QuoteNumber", 2))?.Trim() ?? "";
            int excelCustomerId = GetVal<int>(GetCell(row, quoteMap, "CustomerId", 3));
            int excelPetId = GetVal<int>(GetCell(row, quoteMap, "PetId", 4)) != 0 ? GetVal<int>(GetCell(row, quoteMap, "PetId", 4)) : GetVal<int>(GetCell(row, quoteMap, "PetId", 6));

            if (string.IsNullOrWhiteSpace(quoteNumber))
            {
                quoteNumber = $"QT-{Guid.NewGuid():N}"[..10];
            }

            if (!customerIdMap.TryGetValue(excelCustomerId, out int dbCustomerId)) continue;
            if (!petIdMap.TryGetValue(excelPetId, out int dbPetId)) continue;

            var existingQuote = await _context.Quotes.FirstOrDefaultAsync(q => q.QuoteNumber == quoteNumber);
            if (existingQuote is null)
            {
                string statusStr = GetVal<string>(GetCell(row, quoteMap, "Status", 7)) ?? GetVal<string>(GetCell(row, quoteMap, "Status", 15)) ?? "Active";
                Enum.TryParse<QuoteStatus>(statusStr, true, out var status);

                decimal basePrem = ParseDecimal(GetCell(row, quoteMap, "BasePremium", 16), 50m);
                decimal ageAdj = ParseDecimal(GetCell(row, quoteMap, "AgeAdjustment", 17), 10m);
                decimal wellnessAmt = ParseDecimal(GetCell(row, quoteMap, "WellnessAmount", 18), 15m);
                decimal discAmt = ParseDecimal(GetCell(row, quoteMap, "DiscountAmount", 19), 0m);
                decimal finalPrem = ParseDecimal(GetCell(row, quoteMap, "FinalPremium", 8), 0m);
                if (finalPrem == 0m)
                {
                    finalPrem = ParseDecimal(GetCell(row, quoteMap, "FinalPremium", 20), basePrem + ageAdj + wellnessAmt - discAmt);
                }

                existingQuote = new Quote
                {
                    QuoteNumber = quoteNumber,
                    CustomerId = dbCustomerId,
                    PetId = dbPetId,
                    CreatedDate = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddYears(1),
                    Status = status == default ? QuoteStatus.Active : status,
                    BasePremium = basePrem,
                    AgeAdjustment = ageAdj,
                    WellnessAmount = wellnessAmt,
                    DiscountAmount = discAmt,
                    FinalPremium = finalPrem
                };
                await _context.Quotes.AddAsync(existingQuote);
                await _context.SaveChangesAsync();
                importedQuotes++;
            }
            if (excelQuoteId > 0)
            {
                quoteIdMap[excelQuoteId] = existingQuote.QuoteId;
            }

            // Inline QuoteCoverage check if present in Quotes sheet headers
            if (quoteMap.ContainsKey("AnnualLimit") || quoteMap.ContainsKey("Deductible"))
            {
                var existingCoverage = await _context.QuoteCoverages.FirstOrDefaultAsync(qc => qc.QuoteId == existingQuote.QuoteId);
                if (existingCoverage is null)
                {
                    decimal annualLimit = ParseDecimal(GetCell(row, quoteMap, "AnnualLimit", 21), 5000m);
                    decimal deductible = ParseDecimal(GetCell(row, quoteMap, "Deductible", 22), 250m);
                    decimal reimPct = ParseDecimal(GetCell(row, quoteMap, "ReimbursementPct", 23), 0.80m);
                    bool wellnessAddon = ParseBool(GetCell(row, quoteMap, "WellnessAddon", 24) ?? GetCell(row, quoteMap, "Wellness", 24));

                    var coverage = new QuoteCoverage
                    {
                        QuoteId = existingQuote.QuoteId,
                        AnnualLimit = annualLimit,
                        Deductible = deductible,
                        ReimbursementPct = reimPct,
                        Wellness = wellnessAddon
                    };
                    await _context.QuoteCoverages.AddAsync(coverage);
                    await _context.SaveChangesAsync();
                }
            }
        }

        // 4. Import QuoteCoverages (if separate worksheet exists)
        if (workbook.Worksheets.Contains("QuoteCoverages"))
        {
            var wsCoverages = workbook.Worksheet("QuoteCoverages");
            var covMap = GetColumnMap(wsCoverages);
            var coverageRows = wsCoverages.RowsUsed().Skip(1);

            foreach (var row in coverageRows)
            {
                int excelQuoteId = GetVal<int>(GetCell(row, covMap, "QuoteId", 2));

                if (!quoteIdMap.TryGetValue(excelQuoteId, out int dbQuoteId)) continue;

                var existingCoverage = await _context.QuoteCoverages.FirstOrDefaultAsync(qc => qc.QuoteId == dbQuoteId);
                if (existingCoverage is null)
                {
                    var coverage = new QuoteCoverage
                    {
                        QuoteId = dbQuoteId,
                        AnnualLimit = ParseDecimal(GetCell(row, covMap, "AnnualLimit", 3), 5000m),
                        Deductible = ParseDecimal(GetCell(row, covMap, "Deductible", 4), 250m),
                        ReimbursementPct = ParseDecimal(GetCell(row, covMap, "ReimbursementPct", 5), 0.80m),
                        Wellness = ParseBool(GetCell(row, covMap, "Wellness", 6))
                    };
                    await _context.QuoteCoverages.AddAsync(coverage);
                    await _context.SaveChangesAsync();
                }
            }
        }

        return (importedQuotes, importedCustomers, importedPets);
    }

    #region Helper Methods

    private static void FormatHeaderRow(IXLRow row, int columnCount)
    {
        row.Height = 24;
        for (int col = 1; col <= columnCount; col++)
        {
            var cell = row.Cell(col);
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1F4E78");
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }
    }

    private static Dictionary<string, int> GetColumnMap(IXLWorksheet ws)
    {
        var headerRow = ws.Row(1);
        var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        int lastCol = headerRow.LastCellUsed()?.Address.ColumnNumber ?? 0;
        for (int col = 1; col <= lastCol; col++)
        {
            string headerName = headerRow.Cell(col).GetValue<string>().Trim();
            if (!string.IsNullOrEmpty(headerName) && !map.ContainsKey(headerName))
            {
                map[headerName] = col;
            }
        }
        return map;
    }

    private static IXLCell? GetCell(IXLRow row, Dictionary<string, int> colMap, string headerName, int fallbackColIndex)
    {
        if (colMap.TryGetValue(headerName, out int colIdx))
        {
            return row.Cell(colIdx);
        }
        if (fallbackColIndex > 0 && fallbackColIndex <= (row.LastCellUsed()?.Address.ColumnNumber ?? 0))
        {
            return row.Cell(fallbackColIndex);
        }
        return null;
    }

    private static T? GetVal<T>(IXLCell? cell)
    {
        if (cell is null || cell.IsEmpty()) return default;
        try
        {
            return cell.GetValue<T>();
        }
        catch
        {
            return default;
        }
    }

    private static bool ParseBool(IXLCell? cell)
    {
        if (cell is null || cell.IsEmpty()) return false;
        try
        {
            if (cell.TryGetValue<bool>(out var b)) return b;
            var str = cell.GetValue<string>()?.Trim().ToLower();
            return str == "true" || str == "yes" || str == "1" || str == "y";
        }
        catch
        {
            return false;
        }
    }

    private static decimal ParseDecimal(IXLCell? cell, decimal defaultValue)
    {
        if (cell is null || cell.IsEmpty()) return defaultValue;
        try
        {
            if (cell.TryGetValue<decimal>(out var val)) return val;
            var str = cell.GetValue<string>()?.Replace("$", "").Replace("%", "").Replace(",", "").Trim();
            return decimal.TryParse(str, out val) ? val : defaultValue;
        }
        catch
        {
            return defaultValue;
        }
    }

    #endregion
}
