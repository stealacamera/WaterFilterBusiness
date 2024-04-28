using WaterFilterBusiness.DAL.DAOs;
using WaterFilterBusiness.DAL.Entities.Inventory;
using WaterFilterBusiness.DAL.Repository;

namespace WaterFilterBusiness.DAL.Repositories.Inventory.Requests;

public interface ISmallInventoryRequestsRepository : ISimpleRepository<SmallInventoryRequest, int>
{
    Task<OffsetPaginatedEnumerable<SmallInventoryRequest>> GetAllAsync(int page, int pageSize);
}

internal class SmallInventoryRequestsRepository : SimpleRepository<SmallInventoryRequest, int>, ISmallInventoryRequestsRepository
{
    public SmallInventoryRequestsRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<OffsetPaginatedEnumerable<SmallInventoryRequest>> GetAllAsync(int page, int pageSize)
    {
        IQueryable<SmallInventoryRequest> query = _untrackedSet.OrderBy(e => e.InventoryRequest.CreatedAt);
        return await OffsetPaginatedEnumerable<SmallInventoryRequest>.CreateAsync(query, page, pageSize);
    }
}
