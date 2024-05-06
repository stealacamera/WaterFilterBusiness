using Microsoft.EntityFrameworkCore;
using WaterFilterBusiness.DAL.DAOs;
using WaterFilterBusiness.DAL.Entities;
using WaterFilterBusiness.DAL.Repository;

namespace WaterFilterBusiness.DAL.Repositories.Finance;

public interface ISalesRepository : ISimpleRepository<Sale, int>
{
    Task<OffsetPaginatedEnumerable<Sale>> GetAllAsync(int page, int pageSize);
    Task<int> GetTotalSalesCreatedBySalesAgentAsync(int salesAgentId, DateOnly? filterByDate = null);
}

internal class SalesRepository : SimpleRepository<Sale, int>, ISalesRepository
{
    public SalesRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<OffsetPaginatedEnumerable<Sale>> GetAllAsync(int page, int pageSize)
    {
        IQueryable<Sale> query = _untrackedSet.OrderByDescending(e => e.CreatedAt);
        return await OffsetPaginatedEnumerable<Sale>.CreateAsync(query, page, pageSize);
    }

    public async Task<int> GetTotalSalesCreatedBySalesAgentAsync(int salesAgentId, DateOnly? filterByDate = null)
    {
        IQueryable<Sale> query = _untrackedSet.Where(e => e.Meeting.SalesAgentId == salesAgentId);
        
        if(filterByDate.HasValue)
            query = query.Where(e => e.CreatedAt >= filterByDate.Value.ToDateTime(TimeOnly.MinValue)
                                && e.CreatedAt <= filterByDate.Value.ToDateTime(TimeOnly.MaxValue));

        return await query.CountAsync();
    }
}
