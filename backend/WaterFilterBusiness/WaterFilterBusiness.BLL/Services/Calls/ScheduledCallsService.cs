using FluentResults;
using WaterFilterBusiness.Common.DTOs.Calls;
using WaterFilterBusiness.Common.DTOs.ViewModels;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.ErrorHandling.Errors;
using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL.Services.Calls;

public interface IScheduledCallsService
{
    Task<Result<ScheduledCall>> CreateAsync(ScheduledCall_AddRequestModel call);
    Task<Result<CursorPaginatedList<ScheduledCall, int>>> GetAllForPhoneOperator(
        int phoneOperatorId,
        int paginationCursor,
        int pageSize,
        DateOnly? scheduledFor = null);
    Task<Result> RemoveAsync(int id);
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
        if (!await _utilityService.DoesCustomerExistAsync(call.CustomerId))
            return CustomerErrors.NotFound;
        else if (await _utilityService.IsCustomerRedListedAsync(call.CustomerId))
            return CallErrors.CannotCreate_RedlistedCustomer;
        else if (await _utilityService.DoesCustomerHaveAScheduledCallAsync(call.CustomerId))
            return CallErrors.CannotCreate_CustomerAlreadyScheduled;

        if (!await _utilityService.IsUserInRoleAsync(call.PhoneAgentId, Role.PhoneOperator))
            return CallErrors.NotPhoneAgent;

        var dbModel = await _workUnit.ScheduledCallsRepository
                                     .AddAsync(new DAL.Entities.Clients.ScheduledCall
                                     {
                                         CustomerId = call.CustomerId,
                                         PhoneAgentId = call.PhoneAgentId,
                                         ScheduledAt = call.ScheduledAt
                                     });

        await _workUnit.SaveChangesAsync();

        return new ScheduledCall
        {
            Id = dbModel.Id,
            CustomerId = dbModel.Id,
            PhoneAgentId = dbModel.PhoneAgentId,
            ScheduledAt = dbModel.ScheduledAt
        };
    }

    public async Task<Result<CursorPaginatedList<ScheduledCall, int>>> GetAllForPhoneOperator(
        int phoneOperatorId,
        int paginationCursor,
        int pageSize,
        DateOnly? scheduledFor = null)
    {
        if (!await _utilityService.IsUserInRoleAsync(phoneOperatorId, Role.PhoneOperator))
            return CallErrors.NotPhoneAgent;

        var result = await _workUnit.ScheduledCallsRepository
                                    .GetAllForPhoneOperator(
                                        phoneOperatorId,
                                        paginationCursor,
                                        pageSize,
                                        scheduledFor);

        return new CursorPaginatedList<ScheduledCall, int>
        {
            Cursor = result.Cursor,
            Values = result.Values
                           .Select(e => new ScheduledCall
                           {
                               CustomerId = e.CustomerId,
                               Id = e.Id,
                               PhoneAgentId = e.PhoneAgentId,
                               ScheduledAt = e.ScheduledAt
                           })
                           .ToList()
        };
    }

    public async Task<Result> RemoveAsync(int id)
    {
        var dbModel = await _workUnit.ScheduledCallsRepository.GetByIdAsync(id);

        if (dbModel == null)
            return GeneralErrors.NotFoundError("scheduled call");

        _workUnit.ScheduledCallsRepository.Remove(dbModel);
        await _workUnit.SaveChangesAsync();

        return Result.Ok();
    }
}
