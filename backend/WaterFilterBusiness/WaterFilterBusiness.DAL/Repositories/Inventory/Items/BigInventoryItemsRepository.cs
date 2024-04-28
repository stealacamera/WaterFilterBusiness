using WaterFilterBusiness.DAL.Entities.Inventory;

namespace WaterFilterBusiness.DAL.Repositories.Inventory.Items;

public interface IBigInventoryItemsRepository : IBaseInventoryItemsRepository<BigInventoryItem>
{
}

internal class BigInventoryItemsRepository :
    BaseInventoryItemsRepository<BigInventoryItem>,
    IBigInventoryItemsRepository
{
    public BigInventoryItemsRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
