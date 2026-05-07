using Wardrobe.Data.Entities;
using Wardrobe.Repositories.Interfaces;
using Wardrobe.Services.Interfaces;

namespace Wardrobe.Services.Implementations;

public class ClothingItemService : IClothingItemService
{
    private readonly IClothingItemRepository _repository;

    public ClothingItemService(IClothingItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ClothingItem>> GetByUserIdAsync(int userId)
    {
        return await _repository.GetByUserIdAsync(userId);
    }

    public async Task<ClothingItem?> GetByIdAsync(int id, int userId)
    {
        return await _repository.GetByIdWithDetailsAsync(id, userId);
    }

    public async Task<ClothingItem> CreateAsync(ClothingItem item)
    {
        return await _repository.AddAsync(item);
    }
}