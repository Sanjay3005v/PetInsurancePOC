using System;
using Microsoft.EntityFrameworkCore;
using PetInsurance.Data;
using PetInsurance.Models;
using PetInsurance.Repositories.Interfaces;

namespace PetInsurance.Repositories;

public class PetRepository : IPetRepository
{
    private readonly AppDbContext _context;
 
    public PetRepository(AppDbContext context)
    {
        _context = context;
    }
     
    public async Task<Pet?> GetByIdAsync(int petId)
    {
        return await _context.Pets
            .FirstOrDefaultAsync(p => p.PetId == petId);
    }
     
    public async Task<Pet?> GetByCustomerAndNameAsync(
        int customerId,
        string petName)
    {
        return await _context.Pets
            .FirstOrDefaultAsync(p =>
            p.CustomerId == customerId &&
            p.PetName == petName);
    }
     
    public async Task AddAsync(Pet pet)
    {
        await _context.Pets.AddAsync(pet);
    }
     
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
