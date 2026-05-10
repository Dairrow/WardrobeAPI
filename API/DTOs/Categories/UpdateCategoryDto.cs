using System.ComponentModel.DataAnnotations;

namespace Wardrobe.API.DTOs.Categories;

public class UpdateCategoryDto
{
	[Required]
	[StringLength(100, MinimumLength = 2)]
	public string Name { get; set; } = null!;
}