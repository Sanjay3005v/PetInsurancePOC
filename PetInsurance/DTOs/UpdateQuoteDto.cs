using System;

namespace PetInsurance.DTOs;

public class UpdateQuoteDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? ZipCode { get; set; }

    public string? PetName { get; set; }
    public string? Species { get; set; }
    public string? Breed { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public bool? HasPreExistingCondition { get; set; }

    public decimal AnnualLimit { get; set; }
    public decimal Deductible { get; set; }
    public decimal ReimbursementPct { get; set; }
    public bool Wellness { get; set; }
}
