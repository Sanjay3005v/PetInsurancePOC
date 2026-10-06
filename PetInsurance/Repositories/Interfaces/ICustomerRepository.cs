using System;
using PetInsurance.Models;

namespace PetInsurance.Repositories.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetCustomerByIdAsync(int customerId);
    Task<Customer?> GetCustomerByEmailAsync(string email);
    Task AddCustomerAsync(Customer customer);
    Task SaveCustomerChangesAsync();
}
