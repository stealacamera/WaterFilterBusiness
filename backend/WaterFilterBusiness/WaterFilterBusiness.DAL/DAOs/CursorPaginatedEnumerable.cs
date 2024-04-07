using Microsoft.EntityFrameworkCore;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL.DAOs;

public sealed class CursorPaginatedEnumerable<TEntity, TKey> where TEntity : BaseEntity<TKey> where TKey : IComparable<TKey>
{
    public TKey Cursor { get; private set; }
    public IEnumerable<TEntity> Values { get; private set; } = new List<TEntity>();

    private CursorPaginatedEnumerable()
    {
    }

    public static async Task<CursorPaginatedEnumerable<TEntity, TKey>> CreateFromStrongEntityAsync(
        IQueryable<TEntity> query,
        TKey paginationCursor, int pageSize)
    {
        return await CreateAsync(query, "Id", paginationCursor, pageSize);
    }

    public static async Task<CursorPaginatedEnumerable<TEntity, TKey>> CreateAsync(
        IQueryable<TEntity> query, 
        string primaryKeyName, 
        TKey paginationCursor, int pageSize)
    {
        query = query.Where(e => EF.Property<TKey>(e, primaryKeyName).CompareTo(paginationCursor) >= 0);
        query = query.Take(pageSize + 1);

        var values = await query.ToListAsync();
        var paginatedResult = new CursorPaginatedEnumerable<TEntity, TKey>();

        if (!values.Any())
            return paginatedResult;

        paginatedResult.Cursor = (TKey)values[^1].GetType().GetProperty(primaryKeyName).GetValue(values[^1]);
        paginatedResult.Values = values.Take(pageSize).ToList();
        
        return paginatedResult;
    }
}
