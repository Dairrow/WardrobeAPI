using Microsoft.EntityFrameworkCore;
using Wardrobe.Data.Entities;
using Wardrobe.Repositories.Context;
using Wardrobe.Repositories.Interfaces;

namespace Wardrobe.Repositories.Implementations;

public class OutfitRepository : BaseRepository<Outfit>, IOutfitRepository
{
	public OutfitRepository(AppDbContext context) : base(context)
	{
	}

	public async Task<IEnumerable<Outfit>> GetByUserIdAsync(int userId)
	{
		return await Context.Set<Outfit>()
			.Include(o => o.OutfitItems)
				.ThenInclude(oi => oi.ClothingItem)
			.Where(o => o.UserId == userId)
			.ToListAsync();
	}

	public async Task<Outfit?> GetByIdWithDetailsAsync(int id, int userId)
	{
		return await Context.Set<Outfit>()
			.Include(o => o.OutfitItems)
				.ThenInclude(oi => oi.ClothingItem)
			.FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
	}
}