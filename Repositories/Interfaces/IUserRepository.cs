using Wardrobe.Data.Entities;

namespace Wardrobe.Repositories.Interfaces;

public interface IUserRepository
	: IBaseRepository<User>
{
	Task<User?> GetByEmailAsync(string email);
}