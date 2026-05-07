using System.ComponentModel.DataAnnotations;

namespace Wardrobe.API.DTOs.ClothingItems;

public class UpdateClothingItemDto
{
    [Required]
    public string Name { get; set; } = null!;


    [Range(0.01, 100000)]
    public decimal Price { get; set; }


    public int CategoryId { get; set; }

    public int BrandId { get; set; }


    public string? ImagePath { get; set; }
}