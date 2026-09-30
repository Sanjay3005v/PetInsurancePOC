using System;

namespace PetInsurance.Models;

public class QuoteCoverage
{
    public int QuoteCoverageId {get; set;}
    public int QuoteId {get; set;}
    public decimal AnnualLimit {get; set;}
    public decimal Deductible {get; set;}
    public decimal ReimbursementPct {get; set;}
    public bool Wellness {get; set;}
    public Quote? Quote {get; set;}
}
