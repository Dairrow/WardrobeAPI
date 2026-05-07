using Microsoft.EntityFrameworkCore;
using Wardrobe.Data.Entities;
using Wardrobe.Repositories.Context;
using Wardrobe.Repositories.Interfaces;

namespace Wardrobe.Repositories.Implementations;

public class RoleRepository
    : BaseRepository<Role>,
      IRoleRepository
{
    public RoleRepository(AppDbContext context)
        : base(context)
    {
    }


    public async Task<Role?> GetByNameAsync(string name)
    {
        return await Context.Roles
            .FirstOrDefaultAsync(
                x => x.Name == name);
    }
}