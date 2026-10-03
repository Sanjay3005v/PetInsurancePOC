using System;

namespace PetInsurance.DTOs;

public class QuoteListDto
{
    public int QuoteId { get; set; }
    public string QuoteNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string PetName { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public decimal FinalPremium { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string Status { get; set; } = string.Empty;
}
