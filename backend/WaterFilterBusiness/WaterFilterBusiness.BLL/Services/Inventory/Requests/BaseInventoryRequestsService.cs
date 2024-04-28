using FluentResults;
using WaterFilterBusiness.Common.DTOs.Inventory;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.ErrorHandling.Errors.Inventory;
using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL.Services.Inventory.Requests;

internal class BaseInventoryRequestsService : Service
{
    public BaseInventoryRequestsService(
        IWorkUnit workUnit,
        IUtilityService utilityService)
        : base(workUnit, utilityService)
    {
    }

    protected async Task<Result<DAL.Entities.Inventory.InventoryRequest>> CreateAsync(InventoryRequest_AddRequestModel request)
    {
        if (!await _utilityService.DoesInventoryItemExistAsync(request.ToolId))
            return InventoryItemErrors.NotFound;

        return await _workUnit.InventoryRequestsRepository
                              .AddAsync(new DAL.Entities.Inventory.InventoryRequest
                              {
                                  CreatedAt = DateTime.Now,
                                  Quantity = request.Quantity,
                                  RequestNote = request.RequestNote,
                                  ToolId = request.ToolId,
                                  StatusId = InventoryRequestStatus.Pending
                              });

    }

    protected bool IsStatusChangeValid(DAL.Entities.Inventory.InventoryRequest request, InventoryRequestStatus newStatus)
    {
        bool isRequestFinalized = request.StatusId == InventoryRequestStatus.Cancelled.Value
                                  || request.StatusId == InventoryRequestStatus.Completed.Value;

        bool isRequestInProgress = request.StatusId == InventoryRequestStatus.InProgress.Value;

        return !isRequestFinalized || !(isRequestInProgress && newStatus == InventoryRequestStatus.Pending);
    }
}
