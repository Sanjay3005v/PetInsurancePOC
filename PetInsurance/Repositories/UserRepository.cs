using System;
using Microsoft.EntityFrameworkCore;
using PetInsurance.Data;
using PetInsurance.Models;
using PetInsurance.Repositories.Interfaces;

namespace PetInsurance.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<AppUser?> GetByUserNameAsync(string userName)
    {
        return await _context.Users.FirstOrDefaultAsync(u =>u.UserName == userName);
    }
     
    public async Task AddAsync(AppUser user)
    {
        await _context.Users.AddAsync(user);
    }
     
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
