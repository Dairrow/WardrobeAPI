using Microsoft.EntityFrameworkCore;
using Wardrobe.Data.Common;
using Wardrobe.Repositories.Context;
using Wardrobe.Repositories.Interfaces;

namespace Wardrobe.Repositories.Implementations;

public class BaseRepository<TEntity>
    : IBaseRepository<TEntity>
    where TEntity : BaseEntity
{
    protected readonly AppDbContext Context;

    protected readonly DbSet<TEntity> DbSet;


    public BaseRepository(AppDbContext context)
    {
        Context = context;

        DbSet = context.Set<TEntity>();
    }


    public virtual async Task<TEntity?> GetByIdAsync(int id)
    {
        return await DbSet.FindAsync(id);
    }


    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await DbSet.ToListAsync();
    }


    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        await DbSet.AddAsync(entity);

        await SaveChangesAsync();

        return entity;
    }


    public virtual async Task UpdateAsync(TEntity entity)
    {
        DbSet.Update(entity);

        await SaveChangesAsync();
    }


    public virtual async Task DeleteAsync(TEntity entity)
    {
        DbSet.Remove(entity);

        await SaveChangesAsync();
    }


    public virtual async Task<bool> ExistsAsync(int id)
    {
        return await DbSet.AnyAsync(x => x.Id == id);
    }


    public virtual async Task SaveChangesAsync()
    {
        await Context.SaveChangesAsync();
    }
}