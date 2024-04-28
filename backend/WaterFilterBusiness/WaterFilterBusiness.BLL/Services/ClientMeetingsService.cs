using FluentResults;
using Microsoft.IdentityModel.Tokens;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.ViewModels;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.ErrorHandling.Errors;
using WaterFilterBusiness.DAL;
using WaterFilterBusiness.DAL.DAOs;

namespace WaterFilterBusiness.BLL.Services;

public interface IClientMeetingsService
{
    Task<Result<ClientMeeting>> CreateAsync(ClientMeeting_AddRequestModel clientMeeting);
    Task<Result<ClientMeeting>> UpdateAsync(ClientMeeting_UpdateRequestModel meeting);

    Task<CursorPaginatedList<ClientMeeting, int>> GetAllAsync(
        int paginationCursor,
        int pageSize,
        DateOnly? from = null,
        DateOnly? to = null,
        bool onlyCompleted = false, bool onlyUpcoming = false,
        bool? filterExpressMeetings = false);
    
    Task<CursorPaginatedList<ClientMeeting, int>> GetAllByDayForWorkerAsync(
        int workerId,
        DateOnly date, 
        int paginationCursor, int pageSize, 
        bool? filterExpressMeetings = false);
    
    Task<CursorPaginatedList<ClientMeeting, int>> GetAllByWeekForWorkerAsync(
        int workerId,
        DateOnly date, 
        int paginationCursor, int pageSize, 
        bool? filterExpressMeetings = false);
}

internal class ClientMeetingsService : Service, IClientMeetingsService
{
    public ClientMeetingsService(IWorkUnit workUnit, IUtilityService utilityService) : base(workUnit, utilityService)
    {
    }

    public async Task<Result<ClientMeeting>> CreateAsync(ClientMeeting_AddRequestModel clientMeeting)
    {
        if (!await _utilityService.DoesCustomerExistAsync(clientMeeting.CustomerId))
            return CustomerErrors.NotFound;
        else if (!await _utilityService.DoesUserExistAsync(clientMeeting.SalesAgentId))
            return GeneralErrors.NotFoundError("Sales agent");
        else if (clientMeeting.PhoneOperatorId.HasValue &&
                 !await _utilityService.DoesUserExistAsync(clientMeeting.PhoneOperatorId.Value))
            return GeneralErrors.NotFoundError("Phone agent");

        var validationResult = await IsNewMeetingValidAsync(clientMeeting);

        if (validationResult.IsFailed)
            return Result.Fail(validationResult.Errors);

        var dbModel = await _workUnit.ClientMeetingsRepository
                                     .AddAsync(new DAL.Entities.Clients.ClientMeeting
                                     {
                                         CustomerId = clientMeeting.CustomerId,
                                         InitialNotes = clientMeeting.InitialNotes,
                                         PhoneOperatorId = clientMeeting.PhoneOperatorId,
                                         SalesAgentId = clientMeeting.SalesAgentId,
                                         ScheduledAt = clientMeeting.ScheduledAt
                                     });

        await _workUnit.SaveChangesAsync();

        return ConvertEntityToModel(dbModel);
    }

    private async Task<Result> IsNewMeetingValidAsync(ClientMeeting_AddRequestModel meeting)
    {
        if (await _workUnit.ClientMeetingsRepository
                           .DoesCustomerHaveSuccessfulMeetingsAsync(meeting.CustomerId))
            return ClientMeetingErrors.CustomerIsBuyer;

        if (await _workUnit.ClientMeetingsRepository
                           .IsCustomerAlreadyScheduledAsync(meeting.CustomerId, meeting.ScheduledAt))
            return ClientMeetingErrors.CustomerIsAlreadyScheduled;

        if (await _utilityService.IsTimespanWithinSalesAgentScheduleAsync(meeting.SalesAgentId, meeting.ScheduledAt))
            return ClientMeetingErrors.MeetingOutsideSalesAgentSchedule;

        if (await _workUnit.ClientMeetingsRepository
                           .DoesSalesAgentHaveMeetingsInTimespan(meeting.SalesAgentId, meeting.ScheduledAt))
            return ClientMeetingErrors.SalesAgentBusy_CannotAssignMeeting;

        return Result.Ok();
    }

    public async Task<CursorPaginatedList<ClientMeeting, int>> GetAllByDayForWorkerAsync(
        int workerId,
        DateOnly date,
        int paginationCursor,
        int pageSize,
        bool? filterExpressMeetings = false)
    {
        CursorPaginatedEnumerable<DAL.Entities.Clients.ClientMeeting, int> result;

        if (await _utilityService.IsUserInRoleAsync(workerId, Role.PhoneOperator))
            result = await _workUnit.ClientMeetingsRepository
                                    .GetAllByDayForWorkerAsync(
                                        date,
                                        paginationCursor, pageSize,
                                        phoneOperatorId: workerId,
                                        filterExpressMeetings: filterExpressMeetings);
        else if (await _utilityService.IsUserInRoleAsync(workerId, Role.SalesAgent))
            result = await _workUnit.ClientMeetingsRepository
                                    .GetAllByDayForWorkerAsync(
                                        date,
                                        paginationCursor, pageSize,
                                        salesAgentId: workerId,
                                        filterExpressMeetings: filterExpressMeetings);
        else
            throw new InvalidOperationException("Only sales agents and phone operators can view meetings");

        return new CursorPaginatedList<ClientMeeting, int>
        {
            Cursor = result.Cursor,
            Values = result.Values.Select(ConvertEntityToModel).ToList()
        };
    }

    public async Task<CursorPaginatedList<ClientMeeting, int>> GetAllByWeekForWorkerAsync(
        int workerId, 
        DateOnly date, 
        int paginationCursor, 
        int pageSize, 
        bool? filterExpressMeetings = false)
    {
        CursorPaginatedEnumerable<DAL.Entities.Clients.ClientMeeting, int> result;

        if (await _utilityService.IsUserInRoleAsync(workerId, Role.PhoneOperator))
            result = await _workUnit.ClientMeetingsRepository
                                    .GetAllByWeekForWorkerAsync(
                                        date,
                                        paginationCursor, pageSize,
                                        phoneOperatorId: workerId,
                                        filterExpressMeetings: filterExpressMeetings);
        else if (await _utilityService.IsUserInRoleAsync(workerId, Role.SalesAgent))
            result = await _workUnit.ClientMeetingsRepository
                                    .GetAllByWeekForWorkerAsync(
                                        date,
                                        paginationCursor, pageSize,
                                        salesAgentId: workerId,
                                        filterExpressMeetings: filterExpressMeetings);
        else
            throw new InvalidOperationException("Only sales agents and phone operators can view meetings");

        return new CursorPaginatedList<ClientMeeting, int>
        {
            Cursor = result.Cursor,
            Values = result.Values.Select(ConvertEntityToModel).ToList()
        };
    }

    public async Task<Result<ClientMeeting>> UpdateAsync(ClientMeeting_UpdateRequestModel updatedMeeting)
    {
        var dbModel = await _workUnit.ClientMeetingsRepository.GetByIdAsync(updatedMeeting.Id);

        if (dbModel == null)
            return GeneralErrors.NotFoundError("Client meeting");

        var validationResult = IsMeetingUpdateValid(dbModel, updatedMeeting);

        if (validationResult.IsFailed)
            return Result.Fail(validationResult.Errors);

        dbModel.MeetingOutcomeId = updatedMeeting.Outcome.Value;

        if (!updatedMeeting.Afternotes.IsNullOrEmpty())
            dbModel.Afternotes = updatedMeeting.Afternotes;

        await _workUnit.SaveChangesAsync();
        return ConvertEntityToModel(dbModel);
    }

    public async Task<CursorPaginatedList<ClientMeeting, int>> GetAllAsync(
        int paginationCursor,
        int pageSize,
        DateOnly? from = null,
        DateOnly? to = null,
        bool onlyCompleted = false, bool onlyUpcoming = false,
        bool? filterExpressMeetings = false)
    {
        var result = await _workUnit.ClientMeetingsRepository
                                    .GetAllAsync(
                                        paginationCursor, pageSize, 
                                        from, to, 
                                        onlyCompleted, onlyUpcoming, 
                                        filterExpressMeetings);

        return new CursorPaginatedList<ClientMeeting, int> 
        { 
            Cursor = result.Cursor,
            Values = result.Values.Select(ConvertEntityToModel).ToList()
        };
    }

    private ClientMeeting ConvertEntityToModel(DAL.Entities.Clients.ClientMeeting entity)
    {
        return new ClientMeeting
        {
            Afternotes = entity.Afternotes,
            Customer = new Customer_BriefDescription { Id = entity.CustomerId },
            Id = entity.Id,
            InitialNotes = entity.InitialNotes,
            Outcome = entity.MeetingOutcomeId.HasValue ? MeetingOutcome.FromValue(entity.MeetingOutcomeId.Value) : null,
            PhoneOperator = entity.PhoneOperatorId.HasValue ? new User_BriefDescription { Id = entity.PhoneOperatorId.Value } : null,
            SalesAgent = new User_BriefDescription { Id = entity.SalesAgentId },
            ScheduledAt = entity.ScheduledAt
        };
    }

    private Result IsMeetingUpdateValid(DAL.Entities.Clients.ClientMeeting meeting, ClientMeeting_UpdateRequestModel updatedMeeting)
    {
        if (meeting.MeetingOutcomeId != null)
            return ClientMeetingErrors.CannotUpdate_OutcomeAlreadySet;

        // Restrict possible outcomes for express meetings
        if (meeting.PhoneOperatorId == null)
        {
            if (updatedMeeting.Outcome.Equals(MeetingOutcome.ClientCancelled) ||
               updatedMeeting.Outcome.Equals(MeetingOutcome.AgentCancelled))
                return ClientMeetingErrors.CannotUpdate_ExpressMeetingOutcome;
        }

        return Result.Ok();
    }
}
