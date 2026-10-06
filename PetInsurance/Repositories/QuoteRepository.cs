using System;
using Microsoft.EntityFrameworkCore;
using PetInsurance.Data;
using PetInsurance.Models;
using PetInsurance.Repositories.Interfaces;

namespace PetInsurance.Repositories;

public class QuoteRepository :IQuoteRepository
{
    private readonly AppDbContext _context;
 
    public QuoteRepository(AppDbContext context)
    {
        _context = context;
    }
     
    public async Task<Quote?> GetQuoteByIdAsync(int quoteId)
    {
        return await _context.Quotes
        .Include(q => q.Customer)
        .Include(q => q.Pet)
        .Include(q => q.QuoteCoverage)
        .FirstOrDefaultAsync(q => q.QuoteId == quoteId);
    }
     
    public async Task<Quote?> GetQuoteByQuoteNumberAsync(
    string quoteNumber)
    {
        return await _context.Quotes
        .FirstOrDefaultAsync(q =>
        q.QuoteNumber == quoteNumber);
    }
     
    public async Task<List<Quote>> GetAllQuoteAsync()
    {
        return await _context.Quotes
        .Include(q => q.Customer)
        .Include(q => q.Pet)
        .Include(q => q.QuoteCoverage)
        .ToListAsync();
    }
     
    public async Task AddQuoteAsync(Quote quote)
    {
        await _context.Quotes.AddAsync(quote);
    }
     
    public void UpdateQuote(Quote quote)
    {
        _context.Quotes.Update(quote);
    }
     
    public void DeleteQuote(Quote quote)
    {
        _context.Quotes.Remove(quote);
    }
     
    public async Task<bool> HasDuplicateActiveQuoteAsync(
        int customerId,
        int petId,
        decimal annualLimit,
        decimal deductible,
        decimal reimbursementPct,
        bool wellness)
    {
        return await _context.Quotes
            .Include(q => q.QuoteCoverage)
            .AnyAsync(q =>
            q.CustomerId == customerId &&
            q.PetId == petId &&
            q.Status == QuoteStatus.Active &&
            q.QuoteCoverage != null &&
            q.QuoteCoverage.AnnualLimit == annualLimit &&
            q.QuoteCoverage.Deductible == deductible &&
            q.QuoteCoverage.ReimbursementPct == reimbursementPct &&
            q.QuoteCoverage.Wellness == wellness);
    }

    public async Task<(List<Quote> Items, int TotalCount)> SearchQuotesAsync(PetInsurance.DTOs.SearchQuoteDto dto)
    {
        var query = _context.Quotes
            .Include(q => q.Customer)
            .Include(q => q.Pet)
            .Include(q => q.QuoteCoverage)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            var emailLower = dto.Email.ToLower();
            query = query.Where(q => q.Customer != null && q.Customer.Email.ToLower().Contains(emailLower));
        }

        if (!string.IsNullOrWhiteSpace(dto.PetName))
        {
            var petNameLower = dto.PetName.ToLower();
            query = query.Where(q => q.Pet != null && q.Pet.PetName.ToLower().Contains(petNameLower));
        }

        if (!string.IsNullOrWhiteSpace(dto.Species))
        {
            var speciesLower = dto.Species.ToLower();
            query = query.Where(q => q.Pet != null && q.Pet.Species.ToLower().Contains(speciesLower));
        }

        if (!string.IsNullOrWhiteSpace(dto.Status) && Enum.TryParse<QuoteStatus>(dto.Status, true, out var parsedStatus))
        {
            query = query.Where(q => q.Status == parsedStatus);
        }

        if (dto.MinPremium.HasValue)
        {
            query = query.Where(q => q.FinalPremium >= dto.MinPremium.Value);
        }

        if (dto.MaxPremium.HasValue)
        {
            query = query.Where(q => q.FinalPremium <= dto.MaxPremium.Value);
        }

        if (dto.FromDate.HasValue)
        {
            query = query.Where(q => q.CreatedDate >= dto.FromDate.Value);
        }

        if (dto.ToDate.HasValue)
        {
            query = query.Where(q => q.CreatedDate <= dto.ToDate.Value);
        }

        int totalCount = await query.CountAsync();

        int pageNumber = dto.PageNumber > 0 ? dto.PageNumber : 1;
        int pageSize = dto.PageSize > 0 ? dto.PageSize : 10;

        var items = await query
            .OrderByDescending(q => q.CreatedDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<PetInsurance.DTOs.DashboardDto> GetDashboardMetricsAsync()
    {
        var totalQuotes = await _context.Quotes.CountAsync();
        var activeQuotes = await _context.Quotes.CountAsync(q => q.Status == QuoteStatus.Active);
        var expiredQuotes = await _context.Quotes.CountAsync(q => q.Status == QuoteStatus.Expired);
        var convertedQuotes = await _context.Quotes.CountAsync(q => q.Status == QuoteStatus.Converted);
        var cancelledQuotes = await _context.Quotes.CountAsync(q => q.Status == QuoteStatus.Cancelled);

        var avgPremium = totalQuotes > 0
            ? await _context.Quotes.AverageAsync(q => q.FinalPremium)
            : 0m;

        var expiringSoonThreshold = DateTime.UtcNow.AddDays(7);
        var expiringSoonCount = await _context.Quotes.CountAsync(q =>
            q.Status == QuoteStatus.Active &&
            q.ExpiryDate <= expiringSoonThreshold &&
            q.ExpiryDate >= DateTime.UtcNow);

        var quotesByStatusList = await _context.Quotes
            .GroupBy(q => q.Status)
            .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
            .ToListAsync();

        var quotesByStatus = quotesByStatusList.ToDictionary(x => x.Status, x => x.Count);

        var quotesBySpeciesList = await _context.Quotes
            .Where(q => q.Pet != null)
            .GroupBy(q => q.Pet!.Species)
            .Select(g => new { Species = g.Key, Count = g.Count() })
            .ToListAsync();

        var quotesBySpecies = quotesBySpeciesList.ToDictionary(x => x.Species, x => x.Count);

        return new PetInsurance.DTOs.DashboardDto
        {
            TotalQuotes = totalQuotes,
            ActiveQuotes = activeQuotes,
            ExpiredQuotes = expiredQuotes,
            ConvertedQuotes = convertedQuotes,
            CancelledQuotes = cancelledQuotes,
            AveragePremium = Math.Round(avgPremium, 2),
            ExpiringSoonCount = expiringSoonCount,
            QuotesByStatus = quotesByStatus,
            QuotesBySpecies = quotesBySpecies
        };
    }
     
    public async Task SaveQuoteChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
