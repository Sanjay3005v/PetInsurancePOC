using System;
using Microsoft.EntityFrameworkCore;
using PetInsurance.Data;
using PetInsurance.Models;
using PetInsurance.Repositories.Interfaces;

namespace PetInsurance.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;
 
    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }
     
    public async Task<Customer?> GetCustomerByIdAsync(int customerId)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);
    }
     
    public async Task<Customer?> GetCustomerByEmailAsync(string email)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(c => c.Email == email);
    }
     
    public async Task AddCustomerAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
    }
     
    public async Task SaveCustomerChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
