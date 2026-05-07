using Microsoft.EntityFrameworkCore;
using Wardrobe.Data.Entities;
using Wardrobe.Repositories.Context;
using Wardrobe.Repositories.Interfaces;

namespace Wardrobe.Repositories.Implementations;

public class UserRepository
    : BaseRepository<User>,
      IUserRepository
{
    public UserRepository(AppDbContext context)
        : base(context)
    {
    }


    public async Task<User?> GetByEmailAsync(string email)
    {
        return await Context.Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(
                x => x.Email == email);
    }
}