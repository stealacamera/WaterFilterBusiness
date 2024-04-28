using WaterFilterBusiness.DAL.DAOs;
using WaterFilterBusiness.DAL.Entities.Inventory;
using WaterFilterBusiness.DAL.Repository;

namespace WaterFilterBusiness.DAL.Repositories.Inventory.Items;

public interface IBaseInventoryItemsRepository<TInventoryTypeItem> :
    ISimpleRepository<TInventoryTypeItem, int>
    where TInventoryTypeItem : InventoryTypeItem
{
    Task<OffsetPaginatedEnumerable<TInventoryTypeItem>> GetAllAsync(
        int page,
        int pageSize,
        bool excludeDeleted = false,
        bool? orderByQuantity = null);
}

internal abstract class BaseInventoryItemsRepository<TInventoryTypeItem> :
    SimpleRepository<TInventoryTypeItem, int>,
    IBaseInventoryItemsRepository<TInventoryTypeItem>
    where TInventoryTypeItem : InventoryTypeItem
{
    protected BaseInventoryItemsRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<OffsetPaginatedEnumerable<TInventoryTypeItem>> GetAllAsync(
        int page,
        int pageSize,
        bool excludeDeleted = false,
        bool? orderByQuantity = null)
    {
        IQueryable<TInventoryTypeItem> query = _untrackedSet;

        if (excludeDeleted)
            query = query.Where(e => e.Tool.DeletedAt == null);

        if (orderByQuantity.HasValue)
            query = orderByQuantity.Value 
                    ? query.OrderByDescending(e => e.Quantity) 
                    : query.OrderBy(e => e.Quantity);

        return await OffsetPaginatedEnumerable<TInventoryTypeItem>.CreateAsync(query, page, pageSize);
    }
}