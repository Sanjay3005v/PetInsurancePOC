using System;
using Microsoft.EntityFrameworkCore;
using PetInsurance.Data;
using PetInsurance.Models;
using PetInsurance.Repositories.Interfaces;

namespace PetInsurance.Repositories;

public class QuoteCoverageRepository : IQuoteCoverageRepository
{
    private readonly AppDbContext _context;
 
    public QuoteCoverageRepository(AppDbContext context)
    {
        _context = context;
    }
     
    public async Task<QuoteCoverage?> GetQuoteCoverageByQuoteIdAsync(int quoteId)
    {
        return await _context.QuoteCoverages
            .FirstOrDefaultAsync(qc => qc.QuoteId == quoteId);
    }
     
    public async Task AddQuoteAsync(QuoteCoverage quoteCoverage)
    {
        await _context.QuoteCoverages.AddAsync(quoteCoverage);
    }
     
    public async Task SaveQuoteChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
