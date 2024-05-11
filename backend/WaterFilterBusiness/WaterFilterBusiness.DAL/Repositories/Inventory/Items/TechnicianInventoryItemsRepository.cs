using WaterFilterBusiness.DAL.DAOs;
using WaterFilterBusiness.DAL.Entities.Inventory;
using WaterFilterBusiness.DAL.Repository;

namespace WaterFilterBusiness.DAL.Repositories.Inventory.Items;

public interface ITechnicianInventoryItemsRepository : ICompositeRepository<TechnicianInventoryItem, int, int>
{
    Task<OffsetPaginatedEnumerable<TechnicianInventoryItem>> GetAllAsync(
        int technicianId,
        int page,
        int pageSize,
        bool? orderByQuantity = null);
}

internal class TechnicianInventoryItemsRepository : Repository<TechnicianInventoryItem>, ITechnicianInventoryItemsRepository
{
    public TechnicianInventoryItemsRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<OffsetPaginatedEnumerable<TechnicianInventoryItem>> GetAllAsync(
        int technicianId,
        int page, int pageSize,
        bool? orderByQuantity = null)
    {
        IQueryable<TechnicianInventoryItem> query = _untrackedSet.Where(e => e.TechnicianId == technicianId);

        if(orderByQuantity.HasValue)
            query = orderByQuantity.Value 
                    ? query.OrderByDescending(e => e.Quantity) 
                    : query.OrderBy(e => e.Quantity);

        return await OffsetPaginatedEnumerable<TechnicianInventoryItem>.CreateAsync(query, page, pageSize);
    }

    public async Task<TechnicianInventoryItem?> GetByIdsAsync(int technicianId, int toolId)
    {
        return await _set.FindAsync(technicianId, toolId);
    }
}
