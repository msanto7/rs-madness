using System.ComponentModel.DataAnnotations;

namespace RSMadnessEngine.Api.DTOs.Auth;

public class RegisterRequest
{
    [Required]
    public string DisplayName { get; set; } = string.Empty; 

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required, MinLength(12)]
    public string Password { get; set; } = string.Empty;
}