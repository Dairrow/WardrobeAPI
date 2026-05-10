using Microsoft.EntityFrameworkCore;
using Wardrobe.Data.Entities;
using Wardrobe.Repositories.Context;
using Wardrobe.Repositories.Interfaces;

namespace Wardrobe.Repositories.Implementations;

public class ClothingItemRepository : BaseRepository<ClothingItem>, IClothingItemRepository
{
	public ClothingItemRepository(AppDbContext context) : base(context)
	{
	}

	public async Task<IEnumerable<ClothingItem>> GetByUserIdAsync(int userId)
	{
		return await Context.ClothingItems
			.Include(x => x.Category)
			.Include(x => x.Brand)
			.Where(x => x.UserId == userId)
			.ToListAsync();
	}

	public async Task<ClothingItem?> GetByIdWithDetailsAsync(int id, int userId)
	{
		return await Context.ClothingItems
			.Include(x => x.Category)
			.Include(x => x.Brand)
			.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
	}
}