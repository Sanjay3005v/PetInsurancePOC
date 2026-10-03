using System;

namespace PetInsurance.DTOs;

public class CreateQuoteDto
{
    public string FirstName {get; set;} = string.Empty;
    public string LastName {get; set;} = string.Empty;
    public string Email {get; set;} = string.Empty;
    public string Phone {get; set;} = string.Empty;
    public string ZipCode {get; set;} = string.Empty;

    public string PetName {get; set;} = string.Empty;
    public string Species {get; set;} = string.Empty;
    public string Breed {get; set;} = string.Empty;
    public DateTime DateOfBirth {get; set;} 
    public string Gender {get; set;} = string.Empty;
    public bool HasPreExistingCondition {get; set;}

    public decimal AnnualLimit {get; set;}
    public decimal Deductible {get; set;}
    public decimal ReimbursementPct {get; set;}
    public bool Wellness {get; set;}

}
