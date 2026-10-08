using System;

namespace PetInsurance.DTOs;

public class QuoteDetailsDto
{
    public int QuoteId {get; set;}
    public string QuoteNumber {get; set;} = string.Empty;
    public string CustomerName {get; set;} = string.Empty;
    public string FirstName{get; set;} = string.Empty;
    public string LastName{get; set;} = string.Empty;
    public string Phone {get; set;} = string.Empty;
    public string ZipCode {get; set;} = string.Empty;
    public string Email {get; set;} =string.Empty;
    public string PetName {get; set;} =string.Empty;
    public string Species { get; set; } = string.Empty;
    public string Breed {get; set;} = string.Empty;
    public DateOnly? DateOfBirth {get; set;} 
    public string Gender {get; set;} = string.Empty;
    public bool HasPreExistingCondition {get; set;} 
    public DateTime CreatedDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal BasePremium { get; set; }
    public decimal AgeAdjustment { get; set; }
    public decimal WellnessAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalPremium { get; set; }
    public decimal AnnualLimit {get; set;}
    public decimal Deductible {get; set;}
    public decimal ReimbursementPct {get; set;}
    public bool Wellness {get; set;}
}
