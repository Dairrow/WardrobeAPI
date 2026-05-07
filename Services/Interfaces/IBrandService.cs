using Wardrobe.Data.Entities;

namespace Wardrobe.Services.Interfaces;

public interface IBrandService
{
    Task<IEnumerable<Brand>> GetAllAsync();

    Task<Brand?> GetByIdAsync(int id);

    Task<Brand> CreateAsync(Brand brand);

    Task<Brand> UpdateAsync(
    int id,
    Brand brand);


    Task DeleteAsync(int id);
}