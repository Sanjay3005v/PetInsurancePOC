using System;

namespace PetInsurance.DTOs;

public class UpdateQuoteDto
{
    public decimal AnnualLimit {get; set;}
    public decimal Deductible {get; set;}
    public decimal ReimbursementPct {get; set;}
    public bool Wellness {get; set;}
    
}
