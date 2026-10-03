using System;
using PetInsurance.Models;

namespace PetInsurance.Repositories.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(int customerId);
    Task<Customer?> GetByEmailAsync(string email);
    Task AddAsync(Customer customer);
    Task SaveChangesAsync();
}
