using Wardrobe.Data.Entities;

namespace Wardrobe.Services.Interfaces;

public interface IUserService
{
    Task<User?> GetByIdAsync(int id);

    Task<User?> GetByEmailAsync(string email);

    Task<User> CreateAsync(User user);

    Task<IEnumerable<User>> GetAllAsync();
}