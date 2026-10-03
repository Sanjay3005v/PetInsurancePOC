using System;

namespace PetInsurance.DTOs;

public class RegisterDto
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
