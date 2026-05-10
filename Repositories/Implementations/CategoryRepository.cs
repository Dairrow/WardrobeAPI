using Wardrobe.Data.Entities;
using Wardrobe.Repositories.Context;
using Wardrobe.Repositories.Interfaces;

namespace Wardrobe.Repositories.Implementations;

public class CategoryRepository
	: BaseRepository<Category>,
	  ICategoryRepository
{
	public CategoryRepository(AppDbContext context)
		: base(context)
	{
	}
}