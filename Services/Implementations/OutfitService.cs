using Wardrobe.Data.Entities;
using Wardrobe.Repositories.Interfaces;
using Wardrobe.Services.Exceptions;
using Wardrobe.Services.Interfaces;

namespace Wardrobe.Services.Implementations;

public class OutfitService : IOutfitService
{
    private readonly IOutfitRepository _repository;

    public OutfitService(IOutfitRepository repository)
    {
        _repository = repository;
    }

    public async Task<
    IEnumerable<Outfit>>
    GetAllAsync()
    {
        return await _repository
            .GetAllAsync();
    }

    public async Task<
    Outfit?>
    GetByIdAsync(
        int id)
    {
        return await _repository
            .GetByIdAsync(id);
    }

    public async Task<IEnumerable<Outfit>> GetByUserIdAsync(int userId)
    {
        return await _repository.GetByUserIdAsync(userId);
    }

    public async Task<Outfit?> GetByIdAsync(int id, int userId)
    {
        return await _repository.GetByIdWithDetailsAsync(id, userId);
    }

    public async Task<Outfit> CreateAsync(Outfit outfit)
    {
        if (outfit.UserId <= 0)
        {
            throw new ArgumentException("UserId must be set before creating an outfit");
        }

        return await _repository.AddAsync(outfit);
    }

    public async Task<
    Outfit>
    UpdateAsync(
        int id,
        Outfit outfit)
    {
        var existing =
            await _repository
                .GetByIdAsync(id);


        if (existing is null)
        {
            throw new NotFoundException(
                "Outfit not found");
        }


        existing.Name =
            outfit.Name;


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
                "Outfit not found");
        }


        await _repository
            .DeleteAsync(
                existing);
    }
}