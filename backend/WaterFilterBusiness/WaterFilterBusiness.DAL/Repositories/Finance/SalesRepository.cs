using WaterFilterBusiness.DAL.DAOs;
using WaterFilterBusiness.DAL.Entities;
using WaterFilterBusiness.DAL.Repository;

namespace WaterFilterBusiness.DAL.Repositories.Finance;

public interface ISalesRepository : ISimpleRepository<Sale, int>
{
    Task<OffsetPaginatedEnumerable<Sale>> GetAllAsync(int page, int pageSize);
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
}
