using Wardrobe.Data.Entities;

namespace Wardrobe.Services.Interfaces;

public interface IOutfitItemService
{
	Task<IEnumerable<OutfitItem>> GetByOutfitIdAsync(int outfitId, int userId);
	Task<OutfitItem> AddAsync(int outfitId, int clothingItemId, int userId);
	Task DeleteAsync(int outfitId, int clothingItemId, int userId);
}