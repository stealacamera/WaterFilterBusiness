using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.DAL.DAOs;
using WaterFilterBusiness.DAL.Entities.Inventory;
using WaterFilterBusiness.DAL.Repository;

namespace WaterFilterBusiness.DAL.Repositories.Inventory.Requests;

public interface ITechnicianInventoryRequestsRepository :
    ISimpleRepository<TechnicianInventoryRequest, int>
{
    Task<OffsetPaginatedEnumerable<TechnicianInventoryRequest>> GetAllAsync(
        int page,
        int pageSize,
        InventoryRequestStatus? filterByStatus = null);
}

internal class TechnicianInventoryRequestsRepository :
    SimpleRepository<TechnicianInventoryRequest, int>,
    ITechnicianInventoryRequestsRepository
{
    public TechnicianInventoryRequestsRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<OffsetPaginatedEnumerable<TechnicianInventoryRequest>> GetAllAsync(
        int page,
        int pageSize,
        InventoryRequestStatus? filterByStatus = null)
    {
        IQueryable<TechnicianInventoryRequest> query = _untrackedSet;

        if (filterByStatus != null)
            query = query.Where(e => e.InventoryRequest.StatusId == filterByStatus.Value);

        return await OffsetPaginatedEnumerable<TechnicianInventoryRequest>.CreateAsync(query, page, pageSize);
    }
}
