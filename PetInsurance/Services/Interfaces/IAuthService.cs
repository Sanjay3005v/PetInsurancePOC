using System;
using PetInsurance.DTOs;

namespace PetInsurance.Services.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(RegisterDto dto);
    Task<LoginResponseDto?> LoginAsync(LoginDto dto);
}
