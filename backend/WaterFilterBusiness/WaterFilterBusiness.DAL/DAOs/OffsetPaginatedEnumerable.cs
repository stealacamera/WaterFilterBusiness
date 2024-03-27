using Microsoft.EntityFrameworkCore;

namespace WaterFilterBusiness.DAL.DAOs;

public class OffsetPaginatedEnumerable<T>
{
    public IEnumerable<T> Values { get; set; }
    public int TotalCount { get; set; }

    private OffsetPaginatedEnumerable(IEnumerable<T> values, int totalCount)
    {
        Values = values;
        TotalCount= totalCount;
    }

    public static async Task<OffsetPaginatedEnumerable<T>> CreateAsync(IQueryable<T> query, int page, int pageSize)
    {
        int totalCount = await query.CountAsync();
        var values = await query.Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

        return new(values, totalCount);
    }
}
