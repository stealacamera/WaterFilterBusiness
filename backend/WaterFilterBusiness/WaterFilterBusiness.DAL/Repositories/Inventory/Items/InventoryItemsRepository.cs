using WaterFilterBusiness.DAL.DAOs;
using WaterFilterBusiness.DAL.Entities.Inventory;
using WaterFilterBusiness.DAL.Repository;

namespace WaterFilterBusiness.DAL.Repositories.Inventory.Items;

public interface IInventoryItemsRepository : ISimpleRepository<InventoryItem, int>
{
    Task<OffsetPaginatedEnumerable<InventoryItem>> GetAllAsync(int page, int pageSize, bool excludeDeleted = true);
}

internal class InventoryItemsRepository : SimpleRepository<InventoryItem, int>, IInventoryItemsRepository
{
    public InventoryItemsRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<OffsetPaginatedEnumerable<InventoryItem>> GetAllAsync(int page, int pageSize, bool excludeDeleted = true)
    {
        IQueryable<InventoryItem> query = _untrackedSet;

        if (excludeDeleted)
            query = query.Where(e => e.DeletedAt == null);

        return await OffsetPaginatedEnumerable<InventoryItem>.CreateAsync(query, page, pageSize);
    }
}
