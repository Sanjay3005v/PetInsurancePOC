using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetInsurance.DTOs;
using PetInsurance.Services.Interfaces;

namespace PetInsurance.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
    
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(RegisterDto dto)
        {
            await _authService.RegisterUserAsync(dto);
            
            return Ok("User registered successfully.");
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(LoginDto dto)
        {
            var response = await _authService.LoginUserAsync(dto);
            
            if (response is null)
            {
                return Unauthorized("Invalid username or password.");
            }
            
            return Ok(response);
        }
    }
}
