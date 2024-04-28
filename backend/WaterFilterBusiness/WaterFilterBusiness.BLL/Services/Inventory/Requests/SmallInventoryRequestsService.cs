using Azure.Core;
using FluentResults;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.Inventory;
using WaterFilterBusiness.Common.DTOs.ViewModels;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.ErrorHandling.Errors;
using WaterFilterBusiness.Common.ErrorHandling.Errors.Inventory;
using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL.Services.Inventory.Requests;

public interface ISmallInventoryRequestsService
{
    Task<Result<SmallInventoryRequest>> CreateAsync(int requesterId, InventoryRequest_AddRequestModel request);
    Task<Result<SmallInventoryRequest>> UpdateAsync(int requestId, InventoryRequest_PatchRequestModel update);
    Task<OffsetPaginatedList<SmallInventoryRequest>> GetAllAsync(int page, int pageSize);
}

internal class SmallInventoryRequestsService : BaseInventoryRequestsService, ISmallInventoryRequestsService
{
    public SmallInventoryRequestsService(IWorkUnit workUnit, IUtilityService utilityService) : base(workUnit, utilityService)
    {
    }

    public async Task<OffsetPaginatedList<SmallInventoryRequest>> GetAllAsync(int page, int pageSize)
    {
        var result = await _workUnit.SmallInventoryRequestsRepository
                                    .GetAllAsync(page, pageSize);

        return new OffsetPaginatedList<SmallInventoryRequest>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = result.TotalCount,
            Values = result.Values.Select(ConvertEntityToModel).Select(e => e.Result).ToList()
        };
    }

    public async Task<Result<SmallInventoryRequest>> UpdateAsync(int requestId, InventoryRequest_PatchRequestModel update)
    {
        var dbModel = await _workUnit.SmallInventoryRequestsRepository.GetByIdAsync(requestId);

        if (dbModel == null)
            return InventoryRequestErrors.NotFound;

        var baseRequestModel = await _workUnit.InventoryRequestsRepository.GetByIdAsync(requestId);
        var updateValidation = await IsUpdateValid(baseRequestModel, update);

        if (updateValidation.IsFailed)
            return Result.Fail(updateValidation.Errors);

        baseRequestModel.StatusId = update.Status.Value;
        baseRequestModel.ConclusionNote = update.ConclusionNote;

        if (update.Status == InventoryRequestStatus.Cancelled 
            || update.Status == InventoryRequestStatus.Completed)
            baseRequestModel.ConcludedAt = DateTime.Now;

        await _workUnit.SaveChangesAsync();
        return ConvertEntityToModel(dbModel.RequesterId, baseRequestModel);
    }

    private async Task<Result> IsUpdateValid(
        DAL.Entities.Inventory.InventoryRequest entity,
        InventoryRequest_PatchRequestModel update)
    {
        if (!IsStatusChangeValid(entity, update.Status))
            return InventoryRequestErrors.CannotChangeStatus(
                InventoryRequestStatus.FromValue(entity.StatusId).Name,
                update.Status.Name);

        // Check if there's enough quantity in small inventory to complete request
        int bigInventoryStock = await _utilityService.GetBigInventoryItemQuantityAsync(entity.ToolId);

        if (update.Status == InventoryRequestStatus.Completed && bigInventoryStock < entity.Quantity)
            return InventoryItemErrors.NotEnoughStock;

        return Result.Ok();
    }

    public async Task<Result<SmallInventoryRequest>> CreateAsync(int requesterId, InventoryRequest_AddRequestModel request)
    {
        if (!await _utilityService.DoesUserExistAsync(requesterId))
            return UserErrors.NotFound;

        if(!await _utilityService.IsUserInRoleAsync(requesterId, Role.OperationsChief))
            return GeneralErrors.Unauthorized;

        using var transaction = await _workUnit.BeginTransactionAsync();

        try
        {
            var createRequestResult = await CreateAsync(request);

            if (createRequestResult.IsFailed)
                return Result.Fail(createRequestResult.Errors);

            await _workUnit.SaveChangesAsync();
            await _workUnit.SmallInventoryRequestsRepository
                       .AddAsync(new DAL.Entities.Inventory.SmallInventoryRequest
                       {
                           InventoryRequestId = createRequestResult.Value.Id
                       });

            await _workUnit.SaveChangesAsync();
            await transaction.CommitAsync();

            return ConvertEntityToModel(requesterId, createRequestResult.Value);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task<SmallInventoryRequest> ConvertEntityToModel(DAL.Entities.Inventory.SmallInventoryRequest entity)
    {
        var baseRequestEntity = await _workUnit.InventoryRequestsRepository
                                               .GetByIdAsync(entity.InventoryRequestId);

        return ConvertEntityToModel(entity.RequesterId, baseRequestEntity);
    }

    private SmallInventoryRequest ConvertEntityToModel(int requesterId, DAL.Entities.Inventory.InventoryRequest entity)
    {
        return new SmallInventoryRequest
        {
            ConcludedAt = entity.ConcludedAt,
            ConclusionNote = entity.ConclusionNote,
            CreatedAt = entity.CreatedAt,
            Id = entity.Id,
            Quantity = entity.Quantity,
            RequestNote = entity.RequestNote,
            Status = InventoryRequestStatus.FromValue(entity.StatusId),
            Tool = new InventoryItem { Id = entity.ToolId },
            Requester = new User_BriefDescription { Id = requesterId }
        };
    }
}
