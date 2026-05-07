using System.ComponentModel.DataAnnotations;

namespace Wardrobe.API.DTOs.OutfitItems;

public class CreateOutfitItemDto
{
    [Required]
    public int OutfitId { get; set; }

    [Required]
    public int ClothingItemId { get; set; }
}