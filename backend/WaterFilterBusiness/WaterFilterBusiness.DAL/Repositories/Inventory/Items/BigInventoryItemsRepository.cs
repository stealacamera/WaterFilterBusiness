using Microsoft.EntityFrameworkCore;
using WaterFilterBusiness.DAL.Entities.Inventory;

namespace WaterFilterBusiness.DAL.Repositories.Inventory.Items;

public interface IBigInventoryItemsRepository : IBaseInventoryItemsRepository<BigInventoryItem>
{
    Task<IEnumerable<BigInventoryItem>> GetAllLowStockAsync(int minStock);
}

internal class BigInventoryItemsRepository :
    BaseInventoryItemsRepository<BigInventoryItem>,
    IBigInventoryItemsRepository
{
    public BigInventoryItemsRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<BigInventoryItem>> GetAllLowStockAsync(int minStock)
    {
        IQueryable<BigInventoryItem> query = _untrackedSet.Where(e => e.Quantity < minStock);
        return await query.ToListAsync();
    }
}
