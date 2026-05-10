using System.ComponentModel.DataAnnotations;

namespace Wardrobe.API.DTOs.ClothingItems;

public class CreateClothingItemDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;


    [Range(0.01, 100000)]
    public decimal Price { get; set; }


    [Required]
    public int CategoryId { get; set; }


    [Required]
    public int BrandId { get; set; }


    public IFormFile? Image { get; set; }
}