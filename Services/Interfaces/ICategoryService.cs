using Wardrobe.Data.Entities;

namespace Wardrobe.Services.Interfaces;

public interface ICategoryService
{
	Task<IEnumerable<Category>> GetAllAsync();

	Task<Category?> GetByIdAsync(int id);

	Task<Category> CreateAsync(Category category);

	Task<Category> UpdateAsync(
	int id,
	Category category);


	Task DeleteAsync(
		int id);
}