using System;

namespace PetInsurance.DTOs;

public class QuoteDetailsDto
{
    public int QuoteId {get; set;}
    public string QuoteNumber {get; set;} = string.Empty;
    public string CustomerName {get; set;} = string.Empty;
    public string Email {get; set;} =string.Empty;
    public string PetName {get; set;} =string.Empty;
    public string Species { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal BasePremium { get; set; }
    public decimal AgeAdjustment { get; set; }
    public decimal WellnessAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalPremium { get; set; }
}
