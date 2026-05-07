using Wardrobe.Data.Entities;
using Wardrobe.Repositories.Interfaces;
using Wardrobe.Services.Interfaces;

namespace Wardrobe.Services.Implementations;

public class OutfitService
    : IOutfitService
{
    private readonly IOutfitRepository _repository;


    public OutfitService(
        IOutfitRepository repository)
    {
        _repository = repository;
    }


    public async Task<IEnumerable<Outfit>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }


    public async Task<Outfit?> GetByIdAsync(
        int id)
    {
        return await _repository.GetByIdAsync(
            id);
    }


    public async Task<Outfit> CreateAsync(
        Outfit outfit)
    {
        return await _repository.AddAsync(
            outfit);
    }
}