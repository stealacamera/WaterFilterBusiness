using WaterFilterBusiness.DAL.DAOs;
using WaterFilterBusiness.DAL.Entities.Inventory;
using WaterFilterBusiness.DAL.Repository;

namespace WaterFilterBusiness.DAL.Repositories.Inventory;

public interface IInventoryMovementsRepository : ISimpleRepository<InventoryItemMovement, int>
{
    Task<CursorPaginatedEnumerable<InventoryItemMovement, int>> GetAllAsync(int cursor, int pageSize);
}

internal class InventoryMovementsRepository : SimpleRepository<InventoryItemMovement, int>, IInventoryMovementsRepository
{
    public InventoryMovementsRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<CursorPaginatedEnumerable<InventoryItemMovement, int>> GetAllAsync(int cursor, int pageSize)
    {
        IQueryable<InventoryItemMovement> query = _untrackedSet.OrderByDescending(e => e.OccurredAt);
        return await CursorPaginatedEnumerable<InventoryItemMovement, int>.CreateFromStrongEntityAsync(query, cursor, pageSize);
    }
}
