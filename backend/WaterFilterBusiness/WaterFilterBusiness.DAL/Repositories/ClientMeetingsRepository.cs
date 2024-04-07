using Microsoft.EntityFrameworkCore;
using System.Linq;
using WaterFilterBusiness.Common;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.DAL.DAOs;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL.Repository;

public interface IClientMeetingsRepository : IRepository<ClientMeeting, int>
{
    Task<CursorPaginatedEnumerable<ClientMeeting, int>> GetAllAsync(
        int paginationCursor,
        int pageSize,
        DateOnly? from = null,
        DateOnly? to = null,
        bool onlyCompleted = false, bool onlyUpcoming = false,
        bool onlyExpressMeetings = false);

    Task<CursorPaginatedEnumerable<ClientMeeting, int>> GetAllByDayForWorkerAsync(
        DateOnly date,
        int paginationCursor, int pageSize,
        int? salesAgentId = null, int? phoneOperatorId = null,
        bool onlyExpressMeetings = false);

    Task<CursorPaginatedEnumerable<ClientMeeting, int>> GetAllByWeekForWorkerAsync(
        DateOnly date,
        int paginationCursor, int pageSize,
        int? salesAgentId = null, int? phoneOperatorId = null,
        bool onlyExpressMeetings = false);

    Task<bool> DoesCustomerHaveSuccessfulMeetingsAsync(int customerId);
    Task<bool> IsCustomerAlreadyScheduledAsync(int customerId, DateTime date);
    Task<bool> DoesSalesAgentHaveMeetingsInTimespan(int salesAgentId, DateTime date);
}

internal class ClientMeetingsRepository : Repository<ClientMeeting, int>, IClientMeetingsRepository
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

    public async Task<bool> IsCustomerAlreadyScheduledAsync(int customerId, DateTime date)
    {
        IQueryable<ClientMeeting> query = _untrackedSet.Where(e => e.CustomerId == customerId);
        query = query.Where(e => e.ScheduledAt >= date);

        return await query.AnyAsync();
    }

    public async Task<bool> DoesCustomerHaveSuccessfulMeetingsAsync(int customerId)
    {
        IQueryable<ClientMeeting> query = _untrackedSet.Where(e => e.CustomerId == customerId);
        query = query.Where(e => e.MeetingOutcomeId == MeetingOutcome.Successful);

        return await query.AnyAsync();
    }

    public async Task<CursorPaginatedEnumerable<ClientMeeting, int>> GetAllByDayForWorkerAsync(
        DateOnly date,
        int paginationCursor, int pageSize,
        int? salesAgentId = null, int? phoneOperatorId = null,
        bool onlyExpressMeetings = false)
    {
        IQueryable<ClientMeeting> query = QueryAllForWorkerAsync(salesAgentId, phoneOperatorId, onlyExpressMeetings);
        query = query.Where(e => DateOnly.FromDateTime(e.ScheduledAt).Equals(date));

        return await CursorPaginatedEnumerable<ClientMeeting, int>.CreateFromStrongEntityAsync(query, paginationCursor, pageSize);
    }

    public async Task<CursorPaginatedEnumerable<ClientMeeting, int>> GetAllByWeekForWorkerAsync(
        DateOnly date,
        int paginationCursor, int pageSize,
        int? salesAgentId = null, int? phoneOperatorId = null,
        bool onlyExpressMeetings = false)
    {  
        IQueryable<ClientMeeting> query = QueryAllForWorkerAsync(salesAgentId, phoneOperatorId, onlyExpressMeetings);

        DateTime datetime = date.ToDateTime(TimeOnly.MinValue),
                 weekStartDate = datetime.StartOfWeek(),
                 weekEndDate = datetime.EndOfWeek();
        
        query = query.Where(e => e.ScheduledAt >= weekStartDate && e.ScheduledAt <= weekEndDate);

        return await CursorPaginatedEnumerable<ClientMeeting, int>.CreateFromStrongEntityAsync(query, paginationCursor, pageSize);
    }

    private IQueryable<ClientMeeting> QueryAllForWorkerAsync(int? salesAgentId = null, int? phoneOperatorId = null, bool onlyExpressMeetings = false)
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

        if (onlyExpressMeetings)
            query = query.Where(e => e.PhoneOperatorId == null);

        return query;
    }

    public async Task<CursorPaginatedEnumerable<ClientMeeting, int>> GetAllAsync(
        int paginationCursor, int pageSize, 
        DateOnly? from = null, DateOnly? to = null,
        bool onlyCompleted = false, bool onlyUpcoming = false,
        bool onlyExpressMeetings = false)
    {
        if (onlyCompleted && onlyUpcoming)
            throw new InvalidCastException("Cannot query only completed and only uncompleted meetings simultaneously");

        IQueryable<ClientMeeting> query = _untrackedSet.OrderByDescending(e => e.ScheduledAt);

        if (from.HasValue)
            query = query.Where(e => DateOnly.FromDateTime(e.ScheduledAt) >= from);

        if (to.HasValue)
            query = query.Where(e => DateOnly.FromDateTime(e.ScheduledAt) <= to);

        if (onlyExpressMeetings)
            query = query.Where(e => e.PhoneOperatorId == null);

        if (onlyCompleted)
            query = query.Where(e => e.MeetingOutcomeId != null);
        else if (onlyUpcoming)
            query = query.Where(e => e.MeetingOutcomeId == null);

        return await CursorPaginatedEnumerable<ClientMeeting, int>.CreateFromStrongEntityAsync(query, paginationCursor, pageSize);
    }
}
