using System;

namespace PetInsurance.Models;

public class Pet
{
    public int PetId {get; set;}
    public int CustomerId {get; set;}
    public string PetName {get; set;} = string.Empty;
    public string Species {get; set;} =string.Empty;
    public string Breed {get; set;} = string.Empty;
    public DateTime DateOfBirth {get; set;} 
    public string Gender {get; set;} = string.Empty;
    public bool HasPreExistingCondition {get; set;} 
    public Customer? Customer {get; set;}
    public ICollection<Quote> Quotes {get; set;} =[];
}
