using Wardrobe.Data.Entities;

namespace Wardrobe.Repositories.Interfaces;

public interface IClothingItemRepository : IBaseRepository<ClothingItem>
{
    Task<IEnumerable<ClothingItem>> GetByUserIdAsync(int userId);
    Task<ClothingItem?> GetByIdWithDetailsAsync(int id, int userId);
}