using Wardrobe.Data.Entities;
using Wardrobe.Repositories.Interfaces;
using Wardrobe.Services.Exceptions;
using Wardrobe.Services.Interfaces;

namespace Wardrobe.Services.Implementations;

public class ClothingItemService
    : IClothingItemService
{
    private readonly IClothingItemRepository _repository;


    public ClothingItemService(
        IClothingItemRepository repository)
    {
        _repository = repository;
    }


    public async Task<IEnumerable<ClothingItem>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }


    public async Task<ClothingItem?> GetByIdAsync(int id, int userId)
    {
        return await _repository.GetByIdWithDetailsAsync(id, userId);
    }

    public async Task<IEnumerable<ClothingItem>> GetByUserIdAsync(int userId)
    {
        return await _repository.GetByUserIdAsync(userId);
    }


    public async Task<ClothingItem> CreateAsync(
        ClothingItem item)
    {
        return await _repository.AddAsync(
            item);
    }

    public async Task<
    ClothingItem>
    UpdateAsync(
        int id,
        ClothingItem item)
    {
        var existing =
            await _repository
                .GetByIdAsync(id);


        if (existing is null)
        {
            throw new NotFoundException(
                "Clothing item not found");
        }


        existing.Name =
            item.Name;

        existing.Price =
            item.Price;

        existing.CategoryId =
            item.CategoryId;

        existing.BrandId =
            item.BrandId;

        existing.ImagePath =
            item.ImagePath;


        await _repository
            .UpdateAsync(
                existing);


        return existing;
    }


    public async Task DeleteAsync(
        int id)
    {
        var existing =
            await _repository
                .GetByIdAsync(id);


        if (existing is null)
        {
            throw new NotFoundException(
                "Clothing item not found");
        }


        await _repository
            .DeleteAsync(
                existing);
    }
}