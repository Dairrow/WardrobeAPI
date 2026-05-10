using Wardrobe.Data.Common;

namespace Wardrobe.Repositories.Interfaces;

public interface IBaseRepository<TEntity>
	where TEntity : BaseEntity
{
	Task<TEntity?> GetByIdAsync(int id);

	Task<IEnumerable<TEntity>> GetAllAsync();

	Task<TEntity> AddAsync(TEntity entity);

	Task UpdateAsync(TEntity entity);

	Task DeleteAsync(TEntity entity);

	Task<bool> ExistsAsync(int id);

	Task SaveChangesAsync();
}