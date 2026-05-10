using System.ComponentModel.DataAnnotations;

namespace Wardrobe.API.DTOs.Brands;

public class CreateBrandDto
{
	[Required]
	[StringLength(
		100,
		MinimumLength = 2)]
	public string Name { get; set; } = null!;
}