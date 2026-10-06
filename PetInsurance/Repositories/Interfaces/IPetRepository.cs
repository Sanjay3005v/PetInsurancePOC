using System;
using PetInsurance.Models;

namespace PetInsurance.Repositories.Interfaces;

public interface IPetRepository
{
    Task<Pet?> GetPetByIdAsync(int petId);
    Task<Pet?> GetPetByCustomerAndNameAsync(int customerId, string petName);
    Task AddPetAsync(Pet pet);
    Task SavePetChangesAsync();
}
