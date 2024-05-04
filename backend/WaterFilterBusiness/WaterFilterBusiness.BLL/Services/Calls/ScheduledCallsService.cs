using FluentResults;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.Calls;
using WaterFilterBusiness.Common.DTOs.ViewModels;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.ErrorHandling.Errors;
using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL.Services.Calls;

public interface IScheduledCallsService
{
    Task<Result<ScheduledCall>> CreateAsync(ScheduledCall_AddRequestModel call);
    Task<Result<ScheduledCall>> MarkCompleteAsync(int id);
    Task<Result> RemoveAsync(int id);
    Task<Result<CursorPaginatedList<ScheduledCall, int>>> GetAllForPhoneOperatorAsync(
        int phoneOperatorId,
        int paginationCursor,
        int pageSize,
        DateOnly? scheduledFor = null,
        bool? filterByCompletionStatus = false);
}

internal class ScheduledCallsService : Service, IScheduledCallsService
{
    public ScheduledCallsService(
        IWorkUnit workUnit,
        IUtilityService utilityService) : base(workUnit, utilityService)
    {
    }

    public async Task<Result<ScheduledCall>> CreateAsync(ScheduledCall_AddRequestModel call)
    {
        if (!await _utilityService.IsUserInRoleAsync(call.PhoneAgentId, Role.PhoneOperator))
            return CallErrors.NotPhoneAgent(nameof(call.PhoneAgentId));
        
        if (!await _utilityService.DoesCustomerExistAsync(call.CustomerId))
            return CustomerErrors.NotFound(nameof(call.CustomerId));
        else if (await _utilityService.IsCustomerRedListedAsync(call.CustomerId))
            return ScheduledCallErrors.CannotCreate_RedlistedCustomer(nameof(call.CustomerId));
        
        if (await _utilityService.DoesCustomerHaveAScheduledCallAsync(call.CustomerId))
            return ScheduledCallErrors.CannotCreate_CustomerAlreadyScheduled(nameof(call.CustomerId));
        else if (await _utilityService.DoesPhoneAgentHaveAScheduledCallInTimespanAsync(call.PhoneAgentId, call.ScheduledAt, withinMinutes: 30))
            return ScheduledCallErrors.CannotCreate_PhoneAgentBusy(nameof(call.ScheduledAt), 30);

        var dbModel = await _workUnit.ScheduledCallsRepository
                                     .AddAsync(new DAL.Entities.Clients.ScheduledCall
                                     {
                                         CustomerId = call.CustomerId,
                                         PhoneAgentId = call.PhoneAgentId,
                                         ScheduledAt = call.ScheduledAt
                                     });

        await _workUnit.SaveChangesAsync();
        return ConvertEntityToModel(dbModel);
    }

    public async Task<Result<CursorPaginatedList<ScheduledCall, int>>> GetAllForPhoneOperatorAsync(
        int phoneOperatorId,
        int paginationCursor,
        int pageSize,
        DateOnly? scheduledFor = null,
        bool? filterByCompletionStatus = false)
    {
        if (!await _utilityService.IsUserInRoleAsync(phoneOperatorId, Role.PhoneOperator))
            return CallErrors.NotPhoneAgent(nameof(phoneOperatorId));

        var result = await _workUnit.ScheduledCallsRepository
                                    .GetAllForPhoneOperatorAsync(
                                        phoneOperatorId,
                                        paginationCursor,
                                        pageSize,
                                        scheduledFor,
                                        filterByCompletionStatus);

        return new CursorPaginatedList<ScheduledCall, int>
        {
            Cursor = result.Cursor,
            Values = result.Values.Select(ConvertEntityToModel).ToList()
        };
    }

    public async Task<Result> RemoveAsync(int id)
    {
        var dbModel = await _workUnit.ScheduledCallsRepository.GetByIdAsync(id);

        if (dbModel == null)
            return GeneralErrors.EntityNotFound(nameof(id), "Scheduled call");

        _workUnit.ScheduledCallsRepository.Remove(dbModel);
        await _workUnit.SaveChangesAsync();

        return Result.Ok();
    }

    public async Task<Result<ScheduledCall>> MarkCompleteAsync(int id)
    {
        var dbModel = await _workUnit.ScheduledCallsRepository.GetByIdAsync(id);

        if (dbModel == null)
            return CallErrors.NotFound(nameof(id));
        else if (dbModel.CompletedAt != null)
            return ScheduledCallErrors.AlreadyCompleted(nameof(id));

        dbModel.CompletedAt = DateTime.Now;
        await _workUnit.SaveChangesAsync();

        return ConvertEntityToModel(dbModel);
    }

    private ScheduledCall ConvertEntityToModel(DAL.Entities.Clients.ScheduledCall entity)
    {
        return new ScheduledCall
        {
            Id = entity.Id,
            Customer = new Customer_BriefDescription { Id = entity.CustomerId },
            PhoneAgent = new User_BriefDescription { Id = entity.PhoneAgentId },
            ScheduledAt = entity.ScheduledAt,
            CompletedAt = entity.CompletedAt
        };
    }
}
