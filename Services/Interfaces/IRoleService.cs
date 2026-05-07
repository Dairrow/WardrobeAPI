using Wardrobe.Data.Entities;

namespace Wardrobe.Services.Interfaces;

public interface IRoleService
{
    Task<Role?> GetByIdAsync(int id);

    Task<Role?> GetByNameAsync(string name);

    Task<IEnumerable<Role>> GetAllAsync();
}