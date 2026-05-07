using Wardrobe.Data.Entities;

namespace Wardrobe.Services.Interfaces;

public interface IOutfitService
{
    Task<IEnumerable<Outfit>> GetByUserIdAsync(int userId);
    Task<Outfit?> GetByIdAsync(int id, int userId);
    Task<Outfit> CreateAsync(Outfit outfit);
}