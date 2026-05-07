using Wardrobe.Data.Entities;
using Wardrobe.Repositories.Context;
using Wardrobe.Repositories.Interfaces;

namespace Wardrobe.Repositories.Implementations;

public class BrandRepository
    : BaseRepository<Brand>,
      IBrandRepository
{
    public BrandRepository(AppDbContext context)
        : base(context)
    {
    }
}