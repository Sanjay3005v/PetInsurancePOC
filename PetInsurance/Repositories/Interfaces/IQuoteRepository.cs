using System;
using PetInsurance.Models;
using PetInsurance.DTOs;

namespace PetInsurance.Repositories.Interfaces;

public interface IQuoteRepository
{
    Task<Quote?> GetQuoteByIdAsync(int quoteId);
    Task<Quote?> GetQuoteByQuoteNumberAsync(string quoteNumber);
    Task<List<Quote>> GetAllQuoteAsync();
    Task AddQuoteAsync(Quote quote);
    void UpdateQuote(Quote quote);
    void DeleteQuote(Quote quote);
    Task<bool> HasDuplicateActiveQuoteAsync(int customerId,int petId,decimal annualLimit,decimal deductible,decimal reimbursementPct,bool wellness);
    Task<(List<Quote> Items, int TotalCount)> SearchQuotesAsync(SearchQuoteDto dto);
    Task<DashboardDto> GetDashboardMetricsAsync();
    Task SaveQuoteChangesAsync();
}
