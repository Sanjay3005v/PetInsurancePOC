using System;
using PetInsurance.Models;

namespace PetInsurance.Repositories.Interfaces;

public interface IPetRepository
{
    Task<Pet?> GetByIdAsync(int petId);
    Task<Pet?> GetByCustomerAndNameAsync(int customerId, string petName);
    Task AddAsync(Pet pet);
    Task SaveChangesAsync();
}
