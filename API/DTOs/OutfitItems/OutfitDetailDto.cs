using Wardrobe.API.DTOs.ClothingItems;

namespace Wardrobe.API.DTOs.OutfitItems;

public class OutfitDetailDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public List<ClothingItemDto> Items { get; set; } = new();
}