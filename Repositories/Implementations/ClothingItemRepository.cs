using Wardrobe.Data.Entities;
using Wardrobe.Repositories.Context;
using Wardrobe.Repositories.Interfaces;

namespace Wardrobe.Repositories.Implementations;

public class ClothingItemRepository
    : BaseRepository<ClothingItem>,
      IClothingItemRepository
{
    public ClothingItemRepository(AppDbContext context)
        : base(context)
    {
    }
}