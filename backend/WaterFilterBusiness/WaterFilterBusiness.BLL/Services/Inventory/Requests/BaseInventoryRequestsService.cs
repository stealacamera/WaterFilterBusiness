using FluentResults;
using WaterFilterBusiness.Common.DTOs.Inventory;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.ErrorHandling.Errors.Inventory;
using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL.Services.Inventory.Requests;

public interface IBaseInventoryRequestsService
{
    Task<Result<int>> CreateAsync(InventoryRequest_AddRequestModel request);
}

internal class BaseInventoryRequestsService : Service, IBaseInventoryRequestsService
{
    public BaseInventoryRequestsService(IWorkUnit workUnit, IUtilityService utilityService) : base(workUnit, utilityService)
    {
    }

    public async Task<Result<int>> CreateAsync(InventoryRequest_AddRequestModel request)
    {
        if (!await _utilityService.DoesInventoryItemExistAsync(request.ToolId))
            return InventoryItemErrors.NotFound(nameof(request.ToolId));

        var dbModel = await _workUnit.InventoryRequestsRepository
                                     .AddAsync(new DAL.Entities.Inventory.InventoryRequest
                                     {
                                         CreatedAt = DateTime.Now,
                                         Quantity = request.Quantity,
                                         RequestNote = request.RequestNote,
                                         ToolId = request.ToolId,
                                         StatusId = InventoryRequestStatus.Pending
                                     });

        await _workUnit.SaveChangesAsync();
        return dbModel.Id;
    }
}
