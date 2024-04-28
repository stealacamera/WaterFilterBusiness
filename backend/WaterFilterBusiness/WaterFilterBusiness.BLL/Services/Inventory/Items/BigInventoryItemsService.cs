using FluentResults;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.ViewModels;
using WaterFilterBusiness.Common.Errors.Inventory;
using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL.Services.Inventory.Items;

public interface IBigInventoryItemsService : IBaseInventoryItemsService
{
}

internal class BigInventoryItemsService : Service, IBigInventoryItemsService
{
    public BigInventoryItemsService(IWorkUnit workUnit, IUtilityService utilityService) : base(workUnit, utilityService)
    {
    }

    public async Task<Result<InventoryTypeItem>> DecreaseQuantityAsync(int tooldId, int decreaseBy)
    {
        if (!await _utilityService.DoesInventoryItemExistAsync(tooldId))
            return InventoryItemErrors.NotFound;

        var dbModel = await _workUnit.BigInventoryItemsRepository.GetByIdAsync(tooldId);

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
        var result = await _workUnit.BigInventoryItemsRepository
                                    .GetAllAsync(
                                        page, pageSize, 
                                        excludeDeleted: true, 
                                        orderByQuantity);

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
        if (!await _utilityService.DoesInventoryItemExistAsync(toolId))
            return InventoryItemErrors.NotFound;

        var dbModel = await _workUnit.BigInventoryItemsRepository
                                     .GetByIdAsync(toolId);

        if (dbModel == null)
            dbModel = await _workUnit.BigInventoryItemsRepository
                                     .AddAsync(new DAL.Entities.Inventory.BigInventoryItem
                                     {
                                         ToolId = toolId,
                                         Quantity = quantity
                                     });
        else
            dbModel.Quantity += Math.Abs(quantity);

        await _workUnit.SaveChangesAsync();
        return ConvertEntityToModel(dbModel);
    }

    private InventoryTypeItem ConvertEntityToModel(DAL.Entities.Inventory.BigInventoryItem entity)
    {
        return new InventoryTypeItem
        {
            Item = new InventoryItem { Id = entity.ToolId },
            Quantity = entity.Quantity
        };
    }
}
