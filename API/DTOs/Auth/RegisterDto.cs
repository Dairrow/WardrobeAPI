using System.ComponentModel.DataAnnotations;

namespace Wardrobe.API.DTOs.Auth;

public class RegisterDto
{
    [Required]
    public string Username { get; set; } = null!;


    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;


    [Required]
    [MinLength(6)]
    public string Password { get; set; } = null!;
}