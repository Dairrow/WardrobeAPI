using System.ComponentModel.DataAnnotations;

namespace Wardrobe.API.DTOs.Outfits;

public class CreateOutfitDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;
}