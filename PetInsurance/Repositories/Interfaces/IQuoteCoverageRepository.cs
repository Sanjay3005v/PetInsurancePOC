using System;
using PetInsurance.Models;

namespace PetInsurance.Repositories.Interfaces;

public interface IQuoteCoverageRepository
{
    Task<QuoteCoverage?> GetQuoteCoverageByQuoteIdAsync(int quoteId);
    Task AddQuoteAsync(QuoteCoverage quoteCoverage);
    Task SaveQuoteChangesAsync();
}
