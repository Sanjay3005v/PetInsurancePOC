using System;
using PetInsurance.DTOs;

namespace PetInsurance.Services.Interfaces;

public interface IQuoteService
{
    Task<int> CreateQuoteAsync(CreateQuoteDto dto);
    Task<PagedResultDto<QuoteListDto>> GetQuotesAsync(SearchQuoteDto dto);
    Task<QuoteDetailsDto?> GetQuoteByIdAsync(int quoteId);
    Task<bool> UpdateQuoteAsync(int quoteId, UpdateQuoteDto dto);
    Task<bool> ConvertQuoteAsync(int quoteId);
     
    Task<bool> CancelQuoteAsync(int quoteId);
     
    Task<bool> RecalculateQuoteAsync(int quoteId,bool applyDiscount);
    Task<DashboardDto> GetDashboardMetricsAsync();
}
