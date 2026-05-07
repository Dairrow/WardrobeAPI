using Wardrobe.Data.Entities;
using Wardrobe.Repositories.Context;
using Wardrobe.Repositories.Interfaces;

namespace Wardrobe.Repositories.Implementations;

public class OutfitRepository
    : BaseRepository<Outfit>,
      IOutfitRepository
{
    public OutfitRepository(AppDbContext context)
        : base(context)
    {
    }
}