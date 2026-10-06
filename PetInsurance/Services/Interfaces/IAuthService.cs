using System;
using PetInsurance.DTOs;

namespace PetInsurance.Services.Interfaces;

public interface IAuthService
{
    Task RegisterUserAsync(RegisterDto dto);
    Task<LoginResponseDto?> LoginUserAsync(LoginDto dto);
}
