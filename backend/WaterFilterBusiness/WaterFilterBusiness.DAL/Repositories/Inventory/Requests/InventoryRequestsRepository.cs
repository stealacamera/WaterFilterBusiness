using WaterFilterBusiness.DAL.Entities.Inventory;
using WaterFilterBusiness.DAL.Repository;

namespace WaterFilterBusiness.DAL.Repositories.Inventory.Requests;

public interface IInventoryRequestsRepository : ISimpleRepository<InventoryRequest, int>
{
}

internal class InventoryRequestsRepository : SimpleRepository<InventoryRequest, int>, IInventoryRequestsRepository
{
    public InventoryRequestsRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
