using WaterFilterBusiness.DAL.Entities.Inventory;

namespace WaterFilterBusiness.DAL.Repositories.Inventory.Items;

public interface ISmallInventoryItemsRepository : IBaseInventoryItemsRepository<SmallInventoryItem>
{
}

internal class SmallInventoryItemsRepository :
    BaseInventoryItemsRepository<SmallInventoryItem>,
    ISmallInventoryItemsRepository
{
    public SmallInventoryItemsRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
