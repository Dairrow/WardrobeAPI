using Wardrobe.Data.Entities;

namespace Wardrobe.Services.Interfaces;

public interface IOutfitService
{
    Task<IEnumerable<Outfit>> GetAllAsync();

    Task<Outfit?> GetByIdAsync(int id);

    Task<Outfit> CreateAsync(
        Outfit outfit);
}