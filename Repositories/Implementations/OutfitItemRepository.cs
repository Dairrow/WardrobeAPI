using Microsoft.EntityFrameworkCore;
using Wardrobe.Data.Entities;
using Wardrobe.Repositories.Context;
using Wardrobe.Repositories.Interfaces;

namespace Wardrobe.Repositories.Implementations;

public class OutfitItemRepository : IOutfitItemRepository
{
    private readonly AppDbContext _context;

    public OutfitItemRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OutfitItem>> GetByOutfitIdAsync(int outfitId, int userId)
    {
        return await _context.Set<OutfitItem>()
            .Include(x => x.ClothingItem)
                .ThenInclude(ci => ci.Category)
            .Include(x => x.ClothingItem)
                .ThenInclude(ci => ci.Brand)
            .Include(x => x.Outfit)
            .Where(x => x.OutfitId == outfitId && x.Outfit.UserId == userId)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(int outfitId, int clothingItemId)
    {
        return await _context.Set<OutfitItem>()
            .AnyAsync(x => x.OutfitId == outfitId && x.ClothingItemId == clothingItemId);
    }

    public async Task AddAsync(OutfitItem outfitItem)
    {
        await _context.Set<OutfitItem>().AddAsync(outfitItem);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int outfitId, int clothingItemId, int userId)
    {
        var item = await _context.Set<OutfitItem>()
            .Include(x => x.Outfit)
            .FirstOrDefaultAsync(x => x.OutfitId == outfitId
                                    && x.ClothingItemId == clothingItemId
                                    && x.Outfit.UserId == userId);

        if (item != null)
        {
            _context.Set<OutfitItem>().Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}