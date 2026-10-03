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
     
    public async Task<Quote?> GetByIdAsync(int quoteId)
    {
        return await _context.Quotes
        .Include(q => q.Customer)
        .Include(q => q.Pet)
        .Include(q => q.QuoteCoverage)
        .FirstOrDefaultAsync(q => q.QuoteId == quoteId);
    }
     
    public async Task<Quote?> GetByQuoteNumberAsync(
    string quoteNumber)
    {
        return await _context.Quotes
        .FirstOrDefaultAsync(q =>
        q.QuoteNumber == quoteNumber);
    }
     
    public async Task<List<Quote>> GetAllAsync()
    {
        return await _context.Quotes
        .Include(q => q.Customer)
        .Include(q => q.Pet)
        .Include(q => q.QuoteCoverage)
        .ToListAsync();
    }
     
    public async Task AddAsync(Quote quote)
    {
        await _context.Quotes.AddAsync(quote);
    }
     
    public void Update(Quote quote)
    {
        _context.Quotes.Update(quote);
    }
     
    public void Delete(Quote quote)
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
     
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
