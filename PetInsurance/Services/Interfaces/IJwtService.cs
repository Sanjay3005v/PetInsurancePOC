using System;
using PetInsurance.Models;

namespace PetInsurance.Services.Interfaces;

public interface IJwtService
{
    string GenerateToken(AppUser user);
}
