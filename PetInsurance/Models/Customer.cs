using System;

namespace PetInsurance.Models;

public class Customer
{
    public int CustomerId {get; set;}
    public string FirstName {get; set;} = string.Empty;
    public string LastName {get; set;} = string.Empty;
    public string Email {get; set;} = string.Empty;
    public string Phone {get; set;} = string.Empty;
    public string ZipCode {get; set;} = string.Empty;

    public ICollection<Pet> Pets {get; set;} = [];
    public ICollection<Quote> Quotes {get; set;} = [];
}
