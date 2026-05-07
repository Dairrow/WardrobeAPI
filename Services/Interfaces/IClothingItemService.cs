using Wardrobe.Data.Entities;

namespace Wardrobe.Services.Interfaces;

public interface IClothingItemService
{
    Task<IEnumerable<ClothingItem>> GetAllAsync();

    Task<ClothingItem?> GetByIdAsync(int id, int userId);
    Task<IEnumerable<ClothingItem>> GetByUserIdAsync(int userId);

    Task<ClothingItem> CreateAsync(
        ClothingItem item);

    Task<ClothingItem> UpdateAsync(
    int id,
    ClothingItem item);


    Task DeleteAsync(
        int id);
}