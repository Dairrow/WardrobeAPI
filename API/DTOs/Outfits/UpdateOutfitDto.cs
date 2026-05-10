using System.ComponentModel.DataAnnotations;

namespace Wardrobe.API.DTOs.Outfits;

public class UpdateOutfitDto
{
	[Required]
	public string Name { get; set; } = null!;
}