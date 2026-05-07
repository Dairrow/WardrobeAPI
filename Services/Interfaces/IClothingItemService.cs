using Wardrobe.Data.Entities;

namespace Wardrobe.Services.Interfaces;

public interface IClothingItemService
{
    Task<IEnumerable<ClothingItem>> GetAllAsync();

    Task<ClothingItem?> GetByIdAsync(int id);

    Task<ClothingItem> CreateAsync(
        ClothingItem item);
}