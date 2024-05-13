using FluentResults;
using Microsoft.IdentityModel.Tokens;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.ViewModels;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.ErrorHandling.Errors;
using WaterFilterBusiness.Common.Utilities;
using WaterFilterBusiness.DAL;
using WaterFilterBusiness.DAL.DAOs;

namespace WaterFilterBusiness.BLL.Services;

public interface IClientMeetingsService
{
    Task<Result<ClientMeeting>> CreateAsync(ClientMeeting_AddRequestModel clientMeeting);
    Task<Result<ClientMeeting>> UpdateAsync(int id, ClientMeeting_UpdateRequestModel meeting);
    Task<Result<ClientMeeting>> GetByIdAsync(int id);

    Task<OffsetPaginatedList<ClientMeeting>> GetAllAsync(
        int page,
        int pageSize,
        DateOnly? from = null,
        DateOnly? to = null,
        int? filterByOutcome = null,
        bool? filterExpressMeetings = true);

    Task<Result<CursorPaginatedList<ClientMeeting, int>>> GetAllByDayForWorkerAsync(
        int workerId,
        DateOnly date, 
        int paginationCursor, 
        int pageSize,
        bool? filterByCompleted = null,
        bool? filterExpressMeetings = true);
    
    Task<Result<CursorPaginatedList<ClientMeeting, int>>> GetAllByWeekForWorkerAsync(
        int workerId,
        DateOnly date, 
        int paginationCursor, 
        int pageSize,
        bool? filterByCompleted = null,
        bool? filterExpressMeetings = true);

    Task<Result<Dictionary<int, Dictionary<int, int>>>> GetYearlyTotalMonthlyMeetingsSetupForPhoneAgentAsync(
        int phoneAgentId,
        DateOnly? from = null,
        DateOnly? to = null);

    Task<Result<Dictionary<DateOnly, int>>> GetLastestXWeeklyMeetingsSetupForPhoneAgentAsync(
        int phoneAgentId,
        int nrWeeks);

    Task<Result<int>> GetTotalNrMeetingsBySalesAgentAsync(int salesAgentId, MeetingOutcome? filterByOutcome = null);
}

internal class ClientMeetingsService : Service, IClientMeetingsService
{
    public ClientMeetingsService(IWorkUnit workUnit, IUtilityService utilityService) : base(workUnit, utilityService)
    {
    }

    public async Task<Result<int>> GetTotalNrMeetingsBySalesAgentAsync(int salesAgentId, MeetingOutcome? filterByOutcome = null)
    {
        if (!await _utilityService.IsUserInRoleAsync(salesAgentId, Role.SalesAgent))
            return new Error(nameof(salesAgentId), new Error("Only sales agent can be queried"));

        return await _workUnit.ClientMeetingsRepository
                              .GetTotalNrMeetingsBySalesAgentAsync(salesAgentId, filterByOutcome);
    }

    public async Task<Result<Dictionary<DateOnly, int>>> GetLastestXWeeklyMeetingsSetupForPhoneAgentAsync(int phoneAgentId, int nrWeeks)
    {
        if (!await _utilityService.IsUserInRoleAsync(phoneAgentId, Role.PhoneOperator))
            return new Error(nameof(phoneAgentId), new Error("Only phone agents can be used"));

        var meetings = await _workUnit.ClientMeetingsRepository
                                      .GetLastestXWeeklyMeetingsSetupForPhoneAgentAsync(phoneAgentId, nrWeeks);

        var groupedMeetings = meetings.GroupBy(meeting => meeting.ScheduledAt.StartOfWeek())
                                      .ToDictionary(
                                        weekKey => DateOnly.FromDateTime(weekKey.Key),
                                        weekMeetings => weekMeetings.Count());

        return groupedMeetings;
    }

    public async Task<Result<Dictionary<int, Dictionary<int, int>>>> GetYearlyTotalMonthlyMeetingsSetupForPhoneAgentAsync(
        int phoneAgentId, 
        DateOnly? from = null, 
        DateOnly? to = null)
    {
        if (!await _utilityService.IsUserInRoleAsync(phoneAgentId, Role.PhoneOperator))
            return new Error(nameof(phoneAgentId), new Error("Only phone agents can be used"));

        var meetings = await _workUnit.ClientMeetingsRepository
                                      .GetYearlyTotalMonthlyMeetingsSetupForPhoneAgentAsync(phoneAgentId, from, to);

        var groupedMeetings = meetings.GroupBy(meeting => meeting.ScheduledAt.Year)
                                      .ToDictionary(
                                            yearKey => yearKey.Key,
                                            yearlyMeetings => yearlyMeetings.GroupBy(meeting => meeting.ScheduledAt.Month)
                                                                            .ToDictionary(
                                                                                monthKey => monthKey.Key,
                                                                                monthlyMeetings => monthlyMeetings.Count()));

        return groupedMeetings;
    }

    public async Task<Result<ClientMeeting>> CreateAsync(ClientMeeting_AddRequestModel clientMeeting)
    {
        if (!await _utilityService.DoesCustomerExistAsync(clientMeeting.CustomerId))
            return CustomerErrors.NotFound(nameof(clientMeeting.CustomerId));
        else if (!await _utilityService.DoesUserExistAsync(clientMeeting.SalesAgentId))
            return GeneralErrors.EntityNotFound(nameof(clientMeeting.SalesAgentId), "Sales agent");
        else if (clientMeeting.PhoneOperatorId.HasValue &&
                 !await _utilityService.DoesUserExistAsync(clientMeeting.PhoneOperatorId.Value))
            return GeneralErrors.EntityNotFound(nameof(clientMeeting.PhoneOperatorId), "Phone agent");

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
                           .IsCustomerAlreadyScheduledAsync(meeting.CustomerId))
            return ClientMeetingErrors.CustomerIsAlreadyScheduled(nameof(meeting.CustomerId));

        if (await _utilityService.IsTimespanWithinSalesAgentScheduleAsync(meeting.SalesAgentId, meeting.ScheduledAt))
            return ClientMeetingErrors.MeetingOutsideSalesAgentSchedule(nameof(meeting.SalesAgentId));

        if (await _workUnit.ClientMeetingsRepository
                           .DoesSalesAgentHaveMeetingsInTimespan(meeting.SalesAgentId, meeting.ScheduledAt))
            return ClientMeetingErrors.SalesAgentBusy_CannotAssignMeeting(nameof(meeting.SalesAgentId));

        return Result.Ok();
    }

    public async Task<Result<CursorPaginatedList<ClientMeeting, int>>> GetAllByDayForWorkerAsync(
        int workerId,
        DateOnly date,
        int paginationCursor,
        int pageSize,
        bool? filterByCompleted = null,
        bool? filterExpressMeetings = true)
    {
        CursorPaginatedEnumerable<DAL.Entities.Clients.ClientMeeting, int> result;

        if (await _utilityService.IsUserInRoleAsync(workerId, Role.PhoneOperator))
            result = await _workUnit.ClientMeetingsRepository
                                    .GetAllByDayForWorkerAsync(
                                        date,
                                        paginationCursor, pageSize,
                                        phoneOperatorId: workerId,
                                        filterByCompleted: filterByCompleted,
                                        filterExpressMeetings: filterExpressMeetings);
        else if (await _utilityService.IsUserInRoleAsync(workerId, Role.SalesAgent))
            result = await _workUnit.ClientMeetingsRepository
                                    .GetAllByDayForWorkerAsync(
                                        date,
                                        paginationCursor, pageSize,
                                        salesAgentId: workerId,
                                        filterByCompleted: filterByCompleted,
                                        filterExpressMeetings: filterExpressMeetings);
        else
            return ClientMeetingErrors.InvalidWorker(nameof(workerId));

        return new CursorPaginatedList<ClientMeeting, int>
        {
            Cursor = result.Cursor,
            Values = result.Values.Select(ConvertEntityToModel).ToList()
        };
    }

    public async Task<Result<CursorPaginatedList<ClientMeeting, int>>> GetAllByWeekForWorkerAsync(
        int workerId, 
        DateOnly date, 
        int paginationCursor, 
        int pageSize,
        bool? filterByCompleted = null,
        bool? filterExpressMeetings = true)
    {
        CursorPaginatedEnumerable<DAL.Entities.Clients.ClientMeeting, int> result;

        if (await _utilityService.IsUserInRoleAsync(workerId, Role.PhoneOperator))
            result = await _workUnit.ClientMeetingsRepository
                                    .GetAllByWeekForWorkerAsync(
                                        date,
                                        paginationCursor, pageSize,
                                        phoneOperatorId: workerId,
                                        filterByCompleted: filterByCompleted,
                                        filterExpressMeetings: filterExpressMeetings);
        else if (await _utilityService.IsUserInRoleAsync(workerId, Role.SalesAgent))
            result = await _workUnit.ClientMeetingsRepository
                                    .GetAllByWeekForWorkerAsync(
                                        date,
                                        paginationCursor, pageSize,
                                        salesAgentId: workerId,
                                        filterByCompleted: filterByCompleted,
                                        filterExpressMeetings: filterExpressMeetings);
        else
            return ClientMeetingErrors.InvalidWorker(nameof(workerId));

        return new CursorPaginatedList<ClientMeeting, int>
        {
            Cursor = result.Cursor,
            Values = result.Values.Select(ConvertEntityToModel).ToList()
        };
    }

    public async Task<Result<ClientMeeting>> UpdateAsync(int id, ClientMeeting_UpdateRequestModel updatedMeeting)
    {
        var dbModel = await _workUnit.ClientMeetingsRepository.GetByIdAsync(id);

        if (dbModel == null)
            return GeneralErrors.EntityNotFound("Client meeting");

        var validationResult = IsMeetingUpdateValid(dbModel, updatedMeeting);

        if (validationResult.IsFailed)
            return Result.Fail(validationResult.Errors);

        dbModel.MeetingOutcomeId = updatedMeeting.Outcome.Value;

        if (!updatedMeeting.Afternotes.IsNullOrEmpty())
            dbModel.Afternotes = updatedMeeting.Afternotes;

        await _workUnit.SaveChangesAsync();
        return ConvertEntityToModel(dbModel);
    }

    public async Task<OffsetPaginatedList<ClientMeeting>> GetAllAsync(
        int page,
        int pageSize,
        DateOnly? from = null,
        DateOnly? to = null,
        int? filterByOutcome = null,
        bool? filterExpressMeetings = true)
    {
        var result = await _workUnit.ClientMeetingsRepository
                                    .GetAllAsync(page, pageSize, from, to, filterByOutcome, filterExpressMeetings);

        return new OffsetPaginatedList<ClientMeeting>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = result.TotalCount,
            Values = result.Values.Select(ConvertEntityToModel).ToList()
        };
    }

    public async Task<Result<ClientMeeting>> GetByIdAsync(int id)
    {
        var dbModel = await _workUnit.ClientMeetingsRepository.GetByIdAsync(id);

        return dbModel == null
               ? ClientMeetingErrors.NotFound(nameof(id))
               : ConvertEntityToModel(dbModel);
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
            return ClientMeetingErrors.CannotUpdate_OutcomeAlreadySet(nameof(updatedMeeting.Outcome));

        // Restrict possible outcomes for express meetings
        if (meeting.PhoneOperatorId == null)
        {
            if (updatedMeeting.Outcome.Equals(MeetingOutcome.ClientCancelled) ||
               updatedMeeting.Outcome.Equals(MeetingOutcome.AgentCancelled))
                return ClientMeetingErrors.CannotUpdate_ExpressMeetingOutcome(nameof(updatedMeeting.Outcome));
        }

        return Result.Ok();
    }
}
