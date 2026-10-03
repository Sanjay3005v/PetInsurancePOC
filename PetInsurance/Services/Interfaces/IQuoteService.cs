using System;
using PetInsurance.DTOs;

namespace PetInsurance.Services.Interfaces;

public interface IQuoteService
{
    Task<int> CreateQuoteAsync(CreateQuoteDto dto);
 
    Task<QuoteDetailsDto?> GetQuoteByIdAsync(int quoteId);
     
    Task<bool> ConvertQuoteAsync(int quoteId);
     
    Task<bool> CancelQuoteAsync(int quoteId);
     
    Task<bool> RecalculateQuoteAsync(int quoteId,bool applyDiscount);
}
