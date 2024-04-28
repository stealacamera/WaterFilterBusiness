using FluentResults;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.ViewModels;
using WaterFilterBusiness.Common.ErrorHandling.Errors.Inventory;
using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL.Services.Inventory;

public interface IInventoryPurchasesService
{
    Task<OffsetPaginatedList<InventoryPurchase>> GetAllAsync(int page, int pageSize);
    Task<Result<InventoryPurchase>> CreateAsync(int toolId, int quantity);
}

internal class InventoryPurchasesService : Service, IInventoryPurchasesService
{
    public InventoryPurchasesService(IWorkUnit workUnit, IUtilityService utilityService) : base(workUnit, utilityService)
    {
    }

    public async Task<Result<InventoryPurchase>> CreateAsync(int toolId, int quantity)
    {
        if (!await _utilityService.DoesInventoryItemExistAsync(toolId))
            return InventoryItemErrors.NotFound;
         
        var dbModel = await _workUnit.InventoryPurchasesRepository
                                     .AddAsync(new DAL.Entities.Inventory.InventoryPurchase
                                     {
                                         Price = (await _utilityService.GetInventoryItemPriceAsync(toolId)).Value,
                                         ToolId = toolId,
                                         Quantity = Math.Abs(quantity),
                                         OccurredAt = DateTime.Now
                                     });

        await _workUnit.SaveChangesAsync();
        return ConvertEntityToModel(dbModel);
    }

    public async Task<OffsetPaginatedList<InventoryPurchase>> GetAllAsync(int page, int pageSize)
    {
        var result = await _workUnit.InventoryPurchasesRepository.GetAllAsync(page, pageSize);

        return new OffsetPaginatedList<InventoryPurchase>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = result.TotalCount,
            Values = result.Values
                           .Select(ConvertEntityToModel)
                           .ToList()
        };
    }

    private InventoryPurchase ConvertEntityToModel(DAL.Entities.Inventory.InventoryPurchase entity)
    {
        return new InventoryPurchase
        {
            OccurredAt = entity.OccurredAt,
            Price = entity.Price,
            Quantity = entity.Quantity,
            Tool = new InventoryItem { Id = entity.ToolId }
        };
    }
}
