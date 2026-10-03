using System;
using PetInsurance.Models;

namespace PetInsurance.Repositories.Interfaces;

public interface IQuoteCoverageRepository
{
    Task<QuoteCoverage?> GetByQuoteIdAsync(int quoteId);
    Task AddAsync(QuoteCoverage quoteCoverage);
    Task SaveChangesAsync();
}
