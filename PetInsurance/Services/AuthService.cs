using System;
using PetInsurance.DTOs;
using PetInsurance.Models;
using PetInsurance.Repositories.Interfaces;
using PetInsurance.Services.Interfaces;

namespace PetInsurance.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    public AuthService(IUserRepository userRepository,IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }
    
    public async Task RegisterUserAsync(RegisterDto dto)
    {
    var user = await _userRepository.GetUserByUserNameAsync(dto.UserName);
    
    if (user is not null)
    {
        throw new Exception("User already exists.");
    }
    
    user = new AppUser
    {
        UserName = dto.UserName,
        PasswordHash =
        BCrypt.Net.BCrypt.HashPassword(
        dto.Password)
    };
    
    await _userRepository.AddUserAsync(user);
    
    await _userRepository.SaveUserChangesAsync();
    }
    
    public async Task<LoginResponseDto?> LoginUserAsync(LoginDto dto)
    {
        var user = await _userRepository.GetUserByUserNameAsync(dto.UserName);
    
        if (user is null)
        {
            return null;
        }
    
        bool valid = BCrypt.Net.BCrypt.Verify(dto.Password,user.PasswordHash);
    
        if (!valid)
        {
            return null;
        }
        return new LoginResponseDto
        {
            Token = _jwtService.GenerateToken(user)
        };
    }
}
