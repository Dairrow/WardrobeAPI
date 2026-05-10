namespace Wardrobe.API.DTOs.ClothingItems;

public class ClothingItemDto
{
	public int Id { get; set; }

	public string Name { get; set; } = null!;

	public decimal Price { get; set; }

	public string CategoryName { get; set; } = null!;

	public string BrandName { get; set; } = null!;

	public string? ImagePath { get; set; }
}