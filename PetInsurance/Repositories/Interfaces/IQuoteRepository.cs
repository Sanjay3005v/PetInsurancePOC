using System;
using PetInsurance.Models;

namespace PetInsurance.Repositories.Interfaces;

public interface IQuoteRepository
{
    Task<Quote?> GetByIdAsync(int quoteId);
    Task<Quote?> GetByQuoteNumberAsync(string quoteNumber);
    Task<List<Quote>> GetAllAsync();
    Task AddAsync(Quote quote);
    void Update(Quote quote);
    void Delete(Quote quote);
    Task<bool> HasDuplicateActiveQuoteAsync(int customerId,int petId,decimal annualLimit,decimal deductible,decimal reimbursementPct,bool wellness);
    Task SaveChangesAsync();
}
