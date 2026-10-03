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
    
    public async Task RegisterAsync(RegisterDto dto)
    {
    var user = await _userRepository.GetByUserNameAsync(dto.UserName);
    
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
    
    await _userRepository.AddAsync(user);
    
    await _userRepository.SaveChangesAsync();
    }
    
    public async Task<LoginResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByUserNameAsync(dto.UserName);
    
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
