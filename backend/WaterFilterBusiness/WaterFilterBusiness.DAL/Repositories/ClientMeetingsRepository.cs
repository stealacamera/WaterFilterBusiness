using Microsoft.EntityFrameworkCore;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.Utilities;
using WaterFilterBusiness.DAL.DAOs;
using WaterFilterBusiness.DAL.Entities.Clients;

namespace WaterFilterBusiness.DAL.Repository;

public interface IClientMeetingsRepository : ISimpleRepository<ClientMeeting, int>
{
    Task<OffsetPaginatedEnumerable<ClientMeeting>> GetAllAsync(
        int page,
        int pageSize,
        DateOnly? from = null,
        DateOnly? to = null,
        int? filterByOutcome = null,
        bool? filterExpressMeetings = true);

    Task<CursorPaginatedEnumerable<ClientMeeting, int>> GetAllByDayForWorkerAsync(
        DateOnly date,
        int paginationCursor, 
        int pageSize,
        int? salesAgentId = null, 
        int? phoneOperatorId = null,
        bool? filterByCompleted = null,
        bool? filterExpressMeetings = true);

    Task<CursorPaginatedEnumerable<ClientMeeting, int>> GetAllByWeekForWorkerAsync(
        DateOnly date,
        int paginationCursor, 
        int pageSize,
        int? salesAgentId = null, 
        int? phoneOperatorId = null,
        bool? filterByCompleted = null,
        bool? filterExpressMeetings = true);

    Task<IEnumerable<ClientMeeting>> GetYearlyTotalMonthlyMeetingsSetupForPhoneAgentAsync(
        int phoneAgentId,
        DateOnly? from = null,
        DateOnly? to = null);

    Task<IEnumerable<ClientMeeting>> GetLastestXWeeklyMeetingsSetupForPhoneAgentAsync(
        int phoneAgentId,
        int nrWeeks);

    Task<bool> IsCustomerAlreadyScheduledAsync(int customerId);
    Task<bool> DoesSalesAgentHaveMeetingsInTimespan(int salesAgentId, DateTime date);
}

internal class ClientMeetingsRepository : SimpleRepository<ClientMeeting, int>, IClientMeetingsRepository
{
    public ClientMeetingsRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<bool> DoesSalesAgentHaveMeetingsInTimespan(int salesAgentId, DateTime date)
    {
        IQueryable<ClientMeeting> query = _untrackedSet.Where(e => e.SalesAgentId == salesAgentId);
        query = query.Where(e => e.ScheduledAt >= date && e.ScheduledAt <= date.AddMinutes(40));

        return await query.AnyAsync();
    }

    public async Task<bool> IsCustomerAlreadyScheduledAsync(int customerId)
    {
        IQueryable<ClientMeeting> query = _untrackedSet.Where(e => e.CustomerId == customerId);
        return await query.AnyAsync();
    }

    public async Task<IEnumerable<ClientMeeting>> GetYearlyTotalMonthlyMeetingsSetupForPhoneAgentAsync(
        int phoneAgentId, 
        DateOnly? from = null, 
        DateOnly? to = null)
    {
        IQueryable<ClientMeeting> query = _untrackedSet.Where(e => e.PhoneOperatorId == phoneAgentId);
        query = query.OrderByDescending(e => e.ScheduledAt);

        if (from.HasValue)
            query = query.Where(e => e.ScheduledAt >= from.Value.ToDateTime(TimeOnly.MinValue));

        if (to.HasValue)
            query = query.Where(e => e.ScheduledAt <= to.Value.ToDateTime(TimeOnly.MinValue));

        return await query.ToListAsync();
    }

    public async Task<CursorPaginatedEnumerable<ClientMeeting, int>> GetAllByDayForWorkerAsync(
        DateOnly date,
        int paginationCursor, 
        int pageSize,
        int? salesAgentId = null, 
        int? phoneOperatorId = null,
        bool? filterByCompleted = null,
        bool? filterExpressMeetings = true)
    {
        IQueryable<ClientMeeting> query = QueryAllForWorkerAsync(salesAgentId, phoneOperatorId, filterByCompleted, filterExpressMeetings);
        query = query.Where(e => DateOnly.FromDateTime(e.ScheduledAt).Equals(date));

        return await CursorPaginatedEnumerable<ClientMeeting, int>
            .CreateFromStrongEntityAsync(query, paginationCursor, pageSize);
    }

    public async Task<CursorPaginatedEnumerable<ClientMeeting, int>> GetAllByWeekForWorkerAsync(
        DateOnly date,
        int paginationCursor, 
        int pageSize,
        int? salesAgentId = null, 
        int? phoneOperatorId = null,
        bool? filterByCompleted = null,
        bool? filterExpressMeetings = true)
    {  
        IQueryable<ClientMeeting> query = QueryAllForWorkerAsync(salesAgentId, phoneOperatorId, filterByCompleted, filterExpressMeetings);

        DateTime datetime = date.ToDateTime(TimeOnly.MinValue),
                 weekStartDate = datetime.StartOfWeek(),
                 weekEndDate = datetime.EndOfWeek();
        
        query = query.Where(e => e.ScheduledAt >= weekStartDate && e.ScheduledAt <= weekEndDate);

        return await CursorPaginatedEnumerable<ClientMeeting, int>
            .CreateFromStrongEntityAsync(query, paginationCursor, pageSize);
    }

    private IQueryable<ClientMeeting> QueryAllForWorkerAsync(
        int? salesAgentId = null, 
        int? phoneOperatorId = null,
        bool? filterByCompleted = null,
        bool? filterExpressMeetings = true)
    {
        if (salesAgentId.HasValue && phoneOperatorId.HasValue)
            throw new InvalidOperationException("Querying can be performed only for sales agent or phone operator");
        else if (!salesAgentId.HasValue && !phoneOperatorId.HasValue)
            throw new InvalidOperationException("Querying requires a worker's ID");

        IQueryable<ClientMeeting> query = _untrackedSet.OrderBy(e => e.ScheduledAt)
                                                       .ThenBy(e => e.MeetingOutcomeId);

        if (salesAgentId.HasValue)
            query = query.Where(e => e.SalesAgentId == salesAgentId);
        else
            query = query.Where(e => e.PhoneOperatorId == phoneOperatorId);

        // Truthy value gets only completed meetings
        // Flase value gets only uncompleted meetings
        if (filterByCompleted.HasValue)
            query = query.Where(e => filterByCompleted.Value
                                     ? e.MeetingOutcomeId != null
                                     : e.MeetingOutcomeId == null);

        // Truthy value gets only non-express meetings
        // False value gets only express meetings
        if (filterExpressMeetings.HasValue)
            query = query.Where(e => filterExpressMeetings.Value 
                                     ? e.PhoneOperatorId != null
                                     : e.PhoneOperatorId == null);

        return query;
    }

    public async Task<IEnumerable<ClientMeeting>> GetLastestXWeeklyMeetingsSetupForPhoneAgentAsync(
        int phoneAgentId, 
        int nrWeeks)
    {
        IQueryable<ClientMeeting> query = _untrackedSet.Where(e => e.PhoneOperatorId == phoneAgentId);
        query = query.OrderByDescending(e => e.ScheduledAt);

        var beginDate = DateTime.Now
                                .AddDays(-1 * (nrWeeks * 7))
                                .StartOfWeek();

        query = query.Where(e => e.ScheduledAt <= beginDate);
        return await query.ToListAsync();
    }

    public Task<OffsetPaginatedEnumerable<ClientMeeting>> GetAllAsync(
        int page, 
        int pageSize,
        DateOnly? from = null,
        DateOnly? to = null,
        int? filterByOutcome = null, 
        bool? filterExpressMeetings = true)
    {
        IQueryable<ClientMeeting> query = _untrackedSet.OrderByDescending(e => e.Id);

        if (from.HasValue)
            query = query.Where(e => e.ScheduledAt >= from.Value.ToDateTime(TimeOnly.MinValue));

        if(to.HasValue)
            query = query.Where(e => e.ScheduledAt <= to.Value.ToDateTime(TimeOnly.MaxValue));

        if (filterByOutcome.HasValue)
            query = query.Where(e => filterByOutcome.Value <= 0
                                     ? e.MeetingOutcomeId == null
                                     : e.MeetingOutcomeId == filterByOutcome.Value);

        // Truthy value gets only non-express meetings
        // False value gets only express meetings
        if (filterExpressMeetings.HasValue)
            query = query.Where(e => filterExpressMeetings.Value
                                     ? e.PhoneOperatorId != null
                                     : e.PhoneOperatorId == null);

        return OffsetPaginatedEnumerable<ClientMeeting>.CreateAsync(query, page, pageSize);
    }
}
