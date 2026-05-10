using System.ComponentModel.DataAnnotations;

namespace Wardrobe.API.DTOs.Users;

public class UpdateUserDto
{
	[Required]
	[StringLength(100)]
	public string Username { get; set; } = null!;


	[Required]
	[EmailAddress]
	public string Email { get; set; } = null!;


	[Required]
	public int RoleId { get; set; }
}