using System;
using PetInsurance.Models;

namespace PetInsurance.Repositories.Interfaces;

public interface IUserRepository
{
    Task<AppUser?> GetByUserNameAsync(string userName);
    Task AddAsync(AppUser user);
    Task SaveChangesAsync();
}
