using System;

namespace PetInsurance.Models;

public class Quote
{
    public int QuoteId {get; set;}
    public string QuoteNumber {get; set;} = string.Empty;
    public int CustomerId {get; set;}
    public int PetId {get; set;}
    public DateTime CreatedDate {get; set;}
    public DateTime ExpiryDate {get; set;}
    public QuoteStatus Status {get; set;}
    public decimal BasePremium {get; set;}
    public decimal AgeAdjustment {get; set;}
    public decimal WellnessAmount {get; set;}
    public decimal DiscountAmount {get; set;}
    public decimal FinalPremium {get; set;}
    public Customer? Customer {get; set;}
    public Pet? Pet {get; set;}
    public QuoteCoverage? QuoteCoverage {get; set;}
}
