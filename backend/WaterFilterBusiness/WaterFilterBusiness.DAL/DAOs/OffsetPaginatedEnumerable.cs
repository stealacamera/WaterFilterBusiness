using Microsoft.EntityFrameworkCore;

namespace WaterFilterBusiness.DAL.DAOs;

public sealed class OffsetPaginatedEnumerable<T>
{
    public IEnumerable<T> Values { get; set; }
    public int TotalCount { get; }

    private OffsetPaginatedEnumerable(IEnumerable<T> values, int totalCount)
    {
        Values = values;
        TotalCount= totalCount;
    }

    public static async Task<OffsetPaginatedEnumerable<T>> CreateAsync(IQueryable<T> query, int page, int pageSize)
    {
        if (page <= 0)
            throw new ArgumentOutOfRangeException(nameof(page));
        else if(pageSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        int totalCount = await query.CountAsync();
        
        var values = await query.Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

        return new(values, totalCount);
    }
}
