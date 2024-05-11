using Microsoft.EntityFrameworkCore;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL.Repository;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity> AddAsync(TEntity entity);
}

public interface ISimpleRepository<TEntity, TKey> : IRepository<TEntity> where TEntity : BaseEntity<TKey>
{
    Task<TEntity?> GetByIdAsync(TKey id);
}

public interface ICompositeRepository<TEntity, TKey1, TKey2> : IRepository<TEntity> where TEntity : CompositeEntity<TKey1, TKey2>
{
    Task<TEntity?> GetByIdsAsync(TKey1 key1, TKey2 key2);
}


internal abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
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
}

internal abstract class SimpleRepository<TEntity, TKey> : 
    Repository<TEntity>, 
    ISimpleRepository<TEntity, TKey> 
    where TEntity : BaseEntity<TKey>
{
    protected SimpleRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<TEntity?> GetByIdAsync(TKey id) => await _set.FindAsync(id);
}