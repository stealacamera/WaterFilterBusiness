using FluentResults;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.ViewModels;
using WaterFilterBusiness.Common.ErrorHandling.Errors.Inventory;
using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL.Services.Inventory.Items;

public interface ISmallInventoryItemsService : IBaseInventoryItemsService
{
}

internal class SmallInventoryItemsService : Service, ISmallInventoryItemsService
{
    public SmallInventoryItemsService(IWorkUnit workUnit, IUtilityService utilityService) : base(workUnit, utilityService)
    {
    }

    public async Task<Result<InventoryTypeItem>> DecreaseQuantityAsync(int tooldId, int decreaseBy)
    {
        if (!await _utilityService.DoesInventoryItemExistAsync(tooldId))
            return InventoryItemErrors.NotFound;

        var dbModel = await _workUnit.SmallInventoryItemsRepository.GetByIdAsync(tooldId);

        if (dbModel == null)
            return InventoryItemErrors.NotFound;

        if (dbModel.Quantity - Math.Abs(decreaseBy) < 0)
            return InventoryItemErrors.NotEnoughStock;

        dbModel.Quantity -= Math.Abs(decreaseBy);
        await _workUnit.SaveChangesAsync();

        return ConvertEntityToModel(dbModel);
    }

    public async Task<OffsetPaginatedList<InventoryTypeItem>> GetAllAsync(int page, int pageSize, bool? orderByQuantity = null)
    {
        var result = await _workUnit.SmallInventoryItemsRepository
                                    .GetAllAsync(page, pageSize, excludeDeleted: false, orderByQuantity);

        return new OffsetPaginatedList<InventoryTypeItem>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = result.TotalCount,
            Values = result.Values.Select(ConvertEntityToModel).ToList()
        };
    }

    public async Task<Result<InventoryTypeItem>> UpsertAsync(int toolId, int quantity)
    {
        var dbModel = await _workUnit.SmallInventoryItemsRepository
                                     .GetByIdAsync(toolId);

        if (dbModel == null)
            dbModel = await _workUnit.SmallInventoryItemsRepository
                                     .AddAsync(new DAL.Entities.Inventory.SmallInventoryItem
                                     {
                                         Quantity = quantity,
                                         ToolId = toolId
                                     });
        else
            dbModel.Quantity += quantity;

        await _workUnit.SaveChangesAsync();
        return ConvertEntityToModel(dbModel);
    }

    private InventoryTypeItem ConvertEntityToModel(DAL.Entities.Inventory.SmallInventoryItem entity)
    {
        return new InventoryTypeItem
        {
            Item = new InventoryItem { Id = entity.ToolId },
            Quantity = entity.Quantity
        };
    }
}
