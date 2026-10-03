using System;

namespace PetInsurance.Models;

public class AppUser
{
    public int AppUserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
}
