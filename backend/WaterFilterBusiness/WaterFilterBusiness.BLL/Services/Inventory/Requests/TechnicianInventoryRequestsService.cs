using FluentResults;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.Inventory;
using WaterFilterBusiness.Common.DTOs.ViewModels;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.ErrorHandling.Errors;
using WaterFilterBusiness.Common.ErrorHandling.Errors.Inventory;
using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL.Services.Inventory.Requests;

public interface ITechnicianInventoryRequestsService
{
    Task<OffsetPaginatedList<TechnicianInventoryRequest>> GetAllAsync(
        int page,
        int pageSize,
        InventoryRequestStatus? filterByStatus = null);

    Task<Result<TechnicianInventoryRequest>> CreateAsync(int technicianId, InventoryRequest_AddRequestModel request);
    Task<Result<TechnicianInventoryRequest>> UpdateAsync(int requestId, InventoryRequest_PatchRequestModel request);
}

internal class TechnicianInventoryRequestsService : BaseInventoryRequestsService, ITechnicianInventoryRequestsService
{
    public TechnicianInventoryRequestsService(
        IWorkUnit workUnit,
        IUtilityService utilityService) : base(workUnit, utilityService)
    {
    }

    public async Task<Result<TechnicianInventoryRequest>> CreateAsync(int technicianId, InventoryRequest_AddRequestModel request)
    {
        if (!await _utilityService.DoesUserExistAsync(technicianId))
            return UserErrors.NotFound;

        if (!await _utilityService.IsUserInRoleAsync(technicianId, Role.Technician))
            return GeneralErrors.Unauthorized;

        // Transaction
        using var transaction = await _workUnit.BeginTransactionAsync();

        try
        {
            var createRequestResult = await base.CreateAsync(request);

            if (createRequestResult.IsFailed)
                return Result.Fail(createRequestResult.Errors);

            await _workUnit.SaveChangesAsync();
            await _workUnit.TechnicianInventoryRequestsRepository
                       .AddAsync(new DAL.Entities.Inventory.TechnicianInventoryRequest
                       {
                           InventoryRequestId = createRequestResult.Value.Id,
                           TechnicianId = technicianId
                       });

            await _workUnit.SaveChangesAsync();
            await transaction.CommitAsync();

            return ConvertEntityToModel(technicianId, createRequestResult.Value);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<OffsetPaginatedList<TechnicianInventoryRequest>> GetAllAsync(
        int page,
        int pageSize,
        InventoryRequestStatus? filterByStatus = null)
    {
        var result = await _workUnit.TechnicianInventoryRequestsRepository
                                    .GetAllAsync(page, pageSize, filterByStatus);

        return new OffsetPaginatedList<TechnicianInventoryRequest>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = result.TotalCount,
            Values = result.Values
                           .Select(ConvertEntityToModel)
                           .Select(e => e.Result)
                           .ToList()
        };
    }

    public async Task<Result<TechnicianInventoryRequest>> UpdateAsync(int requestId, InventoryRequest_PatchRequestModel request)
    {
        var dbRequest = await _workUnit.TechnicianInventoryRequestsRepository.GetByIdAsync(requestId);

        if (dbRequest == null)
            return InventoryRequestErrors.NotFound;

        var dbBaseRequest = await _workUnit.InventoryRequestsRepository.GetByIdAsync(requestId);
        var updateValidation = await IsUpdateValid(dbBaseRequest, request);

        if (updateValidation.IsFailed)
            return Result.Fail(updateValidation.Errors);

        dbBaseRequest.StatusId = request.Status.Value;
        dbBaseRequest.ConclusionNote = request.ConclusionNote;

        if (request.Status == InventoryRequestStatus.Cancelled 
            || request.Status == InventoryRequestStatus.Completed)
            dbBaseRequest.ConcludedAt = DateTime.Now;

        await _workUnit.SaveChangesAsync();
        return ConvertEntityToModel(dbRequest.TechnicianId, dbBaseRequest);
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
        int smallInventoryStock = await _utilityService.GetSmallInventoryItemQuantityAsync(entity.ToolId);

        if (update.Status == InventoryRequestStatus.Completed && smallInventoryStock < entity.Quantity)
            return InventoryItemErrors.NotEnoughStock;

        return Result.Ok();
    }

    private async Task<TechnicianInventoryRequest> ConvertEntityToModel(DAL.Entities.Inventory.TechnicianInventoryRequest entity)
    {
        var baseRequest = await _workUnit.InventoryRequestsRepository
                                         .GetByIdAsync(entity.InventoryRequestId);

        return ConvertEntityToModel(entity.TechnicianId, baseRequest);
    }

    private TechnicianInventoryRequest ConvertEntityToModel(int technicianId, DAL.Entities.Inventory.InventoryRequest entity)
    {
        return new TechnicianInventoryRequest
        {
            Technician = new User_BriefDescription { Id = technicianId },
            Id = entity.Id,
            ConcludedAt = entity.ConcludedAt,
            ConclusionNote = entity.ConclusionNote,
            CreatedAt = entity.CreatedAt,
            Quantity = entity.Quantity,
            RequestNote = entity.RequestNote,
            Status = InventoryRequestStatus.FromValue(entity.StatusId),
            Tool = new InventoryItem { Id = entity.ToolId }
        };
    }
}
