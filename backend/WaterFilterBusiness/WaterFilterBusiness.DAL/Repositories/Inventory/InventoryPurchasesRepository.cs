using WaterFilterBusiness.DAL.DAOs;
using WaterFilterBusiness.DAL.Entities.Inventory;
using WaterFilterBusiness.DAL.Repository;

namespace WaterFilterBusiness.DAL.Repositories.Inventory;

public interface IInventoryPurchasesRepository : ISimpleRepository<InventoryPurchase, int>
{
    Task<OffsetPaginatedEnumerable<InventoryPurchase>> GetAllAsync(int page, int pageSize);
}

internal class InventoryPurchasesRepository : SimpleRepository<InventoryPurchase, int>, IInventoryPurchasesRepository
{
    public InventoryPurchasesRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<OffsetPaginatedEnumerable<InventoryPurchase>> GetAllAsync(int page, int pageSize)
    {
        IQueryable<InventoryPurchase> query = _untrackedSet.OrderByDescending(e => e.OccurredAt);
        return await OffsetPaginatedEnumerable<InventoryPurchase>.CreateAsync(query, page, pageSize);
    }
}
