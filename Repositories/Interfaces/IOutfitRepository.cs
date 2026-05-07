using Wardrobe.Data.Entities;

namespace Wardrobe.Repositories.Interfaces;

public interface IOutfitRepository : IBaseRepository<Outfit>
{
    Task<IEnumerable<Outfit>> GetByUserIdAsync(int userId);
    Task<Outfit?> GetByIdWithDetailsAsync(int id, int userId);
}