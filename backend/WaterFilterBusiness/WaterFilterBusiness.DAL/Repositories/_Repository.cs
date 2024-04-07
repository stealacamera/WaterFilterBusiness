using Microsoft.EntityFrameworkCore;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL.Repository;

public interface IRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
{
    Task<TEntity?> GetByIdAsync(TKey id);
    Task<TEntity> AddAsync(TEntity entity);
}

internal abstract class Repository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
{
    protected readonly DbSet<TEntity> _set;
    protected readonly IQueryable<TEntity> _untrackedSet;

    public Repository(AppDbContext dbContext)
    {
        _set = dbContext.Set<TEntity>();
        _untrackedSet = _set.AsNoTracking();
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        await _set.AddAsync(entity);
        return entity;
    }

    public async Task<TEntity?> GetByIdAsync(TKey id)
    {
        return await _set.FindAsync(id);
    }
}
