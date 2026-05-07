using Wardrobe.Data.Entities;

namespace Wardrobe.Services.Interfaces;

public interface IClothingItemService
{
    Task<IEnumerable<ClothingItem>> GetByUserIdAsync(int userId);
    Task<ClothingItem?> GetByIdAsync(int id, int userId);
    Task<ClothingItem> CreateAsync(ClothingItem item);
}