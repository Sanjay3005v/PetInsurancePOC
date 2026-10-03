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
     
    public async Task<QuoteCoverage?> GetByQuoteIdAsync(int quoteId)
    {
        return await _context.QuoteCoverages
            .FirstOrDefaultAsync(qc => qc.QuoteId == quoteId);
    }
     
    public async Task AddAsync(QuoteCoverage quoteCoverage)
    {
        await _context.QuoteCoverages.AddAsync(quoteCoverage);
    }
     
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
