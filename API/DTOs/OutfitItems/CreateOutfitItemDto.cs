using System.ComponentModel.DataAnnotations;

namespace Wardrobe.API.DTOs.OutfitItems;

public class CreateOutfitItemDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "OutfitId must be greater than 0")]
    public int OutfitId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "ClothingItemId must be greater than 0")]
    public int ClothingItemId { get; set; }
}