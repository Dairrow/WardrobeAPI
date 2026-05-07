using Wardrobe.Data.Entities;

namespace Wardrobe.Repositories.Interfaces;

public interface IOutfitItemRepository
{
    Task<IEnumerable<OutfitItem>> GetByOutfitIdAsync(int outfitId, int userId);
    Task<bool> ExistsAsync(int outfitId, int clothingItemId);
    Task AddAsync(OutfitItem outfitItem);
    Task DeleteAsync(int outfitId, int clothingItemId, int userId);
}