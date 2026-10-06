using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using PetInsurance.DTOs;
using PetInsurance.Models;
using PetInsurance.Repositories.Interfaces;
using PetInsurance.Services;
using PetInsurance.Services.Interfaces;

namespace PetInsuranceTests;

[TestFixture]
public class AuthServiceTests
{
    private Mock<IUserRepository> _mockUserRepo = null!;
    private Mock<IJwtService> _mockJwtService = null!;
    private AuthService _authService = null!;

    [SetUp]
    public void Setup()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockJwtService = new Mock<IJwtService>();
        _authService = new AuthService(_mockUserRepo.Object, _mockJwtService.Object);
    }

    [Test]
    public async Task RegisterAsync_NewUser_RegistersSuccessfully()
    {
        // Arrange
        var dto = new RegisterDto { UserName = "newuser", Password = "Password123!" };
        _mockUserRepo.Setup(r => r.GetUserByUserNameAsync("newuser")).ReturnsAsync((AppUser?)null);

        // Act
        await _authService.RegisterUserAsync(dto);

        // Assert
        _mockUserRepo.Verify(r => r.AddUserAsync(It.Is<AppUser>(u => u.UserName == "newuser")), Times.Once);
        _mockUserRepo.Verify(r => r.SaveUserChangesAsync(), Times.Once);
    }

    [Test]
    public async Task LoginAsync_ValidCredentials_ReturnsJwtToken()
    {
        // Arrange
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword("Password123!");
        var user = new AppUser { AppUserId = 1, UserName = "admin", PasswordHash = hashedPassword };

        _mockUserRepo.Setup(r => r.GetUserByUserNameAsync("admin")).ReturnsAsync(user);
        _mockJwtService.Setup(j => j.GenerateToken(user)).Returns("mocked-jwt-token");

        // Act
        var result = await _authService.LoginUserAsync(new LoginDto { UserName = "admin", Password = "Password123!" });

        // Assert
        result.Should().NotBeNull();
        result!.Token.Should().Be("mocked-jwt-token");
    }

    [Test]
    public async Task LoginAsync_InvalidPassword_ReturnsNull()
    {
        // Arrange
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword("Password123!");
        var user = new AppUser { AppUserId = 1, UserName = "admin", PasswordHash = hashedPassword };

        _mockUserRepo.Setup(r => r.GetUserByUserNameAsync("admin")).ReturnsAsync(user);

        // Act
        var result = await _authService.LoginUserAsync(new LoginDto { UserName = "admin", Password = "WrongPassword" });

        // Assert
        result.Should().BeNull();
    }
}
