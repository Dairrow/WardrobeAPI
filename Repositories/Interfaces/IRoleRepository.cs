using Wardrobe.Data.Entities;

namespace Wardrobe.Repositories.Interfaces;

public interface IRoleRepository
    : IBaseRepository<Role>
{
    Task<Role?> GetByNameAsync(string name);
}