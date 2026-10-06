using System;
using PetInsurance.Models;

namespace PetInsurance.Repositories.Interfaces;

public interface IUserRepository
{
    Task<AppUser?> GetUserByUserNameAsync(string userName);
    Task AddUserAsync(AppUser user);
    Task SaveUserChangesAsync();
}
