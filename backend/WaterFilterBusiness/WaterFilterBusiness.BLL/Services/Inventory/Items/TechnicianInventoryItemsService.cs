using FluentResults;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.ViewModels;
using WaterFilterBusiness.Common.Errors;
using WaterFilterBusiness.Common.Errors.Inventory;
using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL.Services.Inventory.Items;

public interface ITechnicianInventoryItemsService
{
    Task<Result<InventoryTypeItem>> UpsertAsync(int technicianId, int toolId, int quantity);
    Task<Result<InventoryTypeItem>> DecreaseQuantityAsync(int technicianId, int tooldId, int decreaseBy);

    Task<Result<OffsetPaginatedList<InventoryTypeItem>>> GetAllAsync(
        int technicianId,
        int page, int pageSize,
        bool? orderByQuantity = null);
}

internal class TechnicianInventoryItemsService : Service, ITechnicianInventoryItemsService
{
    public TechnicianInventoryItemsService(IWorkUnit workUnit, IUtilityService utilityService) : base(workUnit, utilityService)
    {
    }

    public async Task<Result<InventoryTypeItem>> DecreaseQuantityAsync(int technicianId, int tooldId, int decreaseBy)
    {
        if (!await _utilityService.DoesUserExistAsync(technicianId))
            return UserErrors.NotFound;

        if (!await _utilityService.DoesInventoryItemExistAsync(tooldId))
            return InventoryItemErrors.NotFound;

        var dbModel = await _workUnit.TechnicianInventoryItemsRepository.GetByIdsAsync(technicianId, tooldId);

        if (dbModel == null)
            return InventoryItemErrors.NotFound;

        if (dbModel.Quantity - Math.Abs(decreaseBy) < 0)
            return InventoryItemErrors.NotEnoughStock;

        dbModel.Quantity -= Math.Abs(decreaseBy);
        await _workUnit.SaveChangesAsync();

        return ConvertEntityToModel(dbModel);
    }

    public async Task<Result<OffsetPaginatedList<InventoryTypeItem>>> GetAllAsync(
        int technicianId,
        int page, int pageSize,
        bool? orderByQuantity = null)
    {
        if (!await _utilityService.DoesUserExistAsync(technicianId))
            return UserErrors.NotFound;

        var result = await _workUnit.TechnicianInventoryItemsRepository
                                    .GetAllAsync(technicianId, page, pageSize, orderByQuantity);

        return new OffsetPaginatedList<InventoryTypeItem>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = result.TotalCount,
            Values = result.Values.Select(ConvertEntityToModel).ToList()
        };
    }

    public async Task<Result<InventoryTypeItem>> UpsertAsync(int technicianId, int toolId, int quantity)
    {
        if (!await _utilityService.DoesUserExistAsync(technicianId))
            return UserErrors.NotFound;

        if (!await _utilityService.DoesInventoryItemExistAsync(toolId))
            return InventoryItemErrors.NotFound;

        var dbModel = await _workUnit.TechnicianInventoryItemsRepository.GetByIdsAsync(technicianId, toolId);

        // If tool doesn't exist in their inventory, then it is created
        if (dbModel == null)
            dbModel = await _workUnit.TechnicianInventoryItemsRepository
                                     .AddAsync(new DAL.Entities.Inventory.TechnicianInventoryItem
                                     {
                                         Quantity = Math.Abs(quantity),
                                         TechnicianId = technicianId,
                                         ToolId = toolId
                                     });
        else
            dbModel.Quantity += Math.Abs(quantity);

        await _workUnit.SaveChangesAsync();
        return ConvertEntityToModel(dbModel);
    }

    private InventoryTypeItem ConvertEntityToModel(DAL.Entities.Inventory.TechnicianInventoryItem entity)
    {
        return new InventoryTypeItem
        {
            Item = new InventoryItem { Id = entity.ToolId },
            Quantity = entity.Quantity
        };
    }
}
