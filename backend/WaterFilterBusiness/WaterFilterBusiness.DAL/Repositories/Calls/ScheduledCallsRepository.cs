using Microsoft.EntityFrameworkCore;
using WaterFilterBusiness.DAL.DAOs;
using WaterFilterBusiness.DAL.Entities.Clients;

namespace WaterFilterBusiness.DAL.Repository.Calls;

public interface IScheduledCallsRepository : ISimpleRepository<ScheduledCall, int>
{
    Task<bool> DoesCustomerHaveNonCompletedCallsAsync(int customerId);
    Task<bool> IsPhoneAgentBusyForTimespanAsync(int phoneAgentId, DateTime schedule, int withinMinutes);
    void Remove(ScheduledCall call);

    Task<CursorPaginatedEnumerable<ScheduledCall, int>> GetAllForPhoneOperatorAsync(
        int phoneOperatorId,
        int paginationCursor,
        int pageSize,
        DateOnly? scheduledDate = null,
        bool? filterByCompletionStatus = true);
}

internal class ScheduledCallsRepository : SimpleRepository<ScheduledCall, int>, IScheduledCallsRepository
{
    public ScheduledCallsRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<bool> IsPhoneAgentBusyForTimespanAsync(int phoneAgentId, DateTime schedule, int withinMinutes)
    {
        IQueryable<ScheduledCall> query = _untrackedSet.Where(e => e.PhoneAgentId == phoneAgentId);
        query = query.Where(e => e.CompletedAt == null);

        query = query.Where(e => e.ScheduledAt >= schedule
                                 && e.ScheduledAt <= schedule.AddMinutes(withinMinutes));

        return await query.AnyAsync();
    }

    public async Task<bool> DoesCustomerHaveNonCompletedCallsAsync(int customerId)
    {
        IQueryable<ScheduledCall> query = _untrackedSet.Where(e => e.CustomerId == customerId);
        query = query.Where(e => e.CompletedAt == null);

        return await query.AnyAsync();
    }

    public async Task<CursorPaginatedEnumerable<ScheduledCall, int>> GetAllForPhoneOperatorAsync(
        int phoneOperatorId,
        int paginationCursor,
        int pageSize,
        DateOnly? scheduledDate = null,
        bool? filterByCompletionStatus = true)
    {
        IQueryable<ScheduledCall> query = _untrackedSet.OrderBy(e => e.ScheduledAt);
        query = query.Where(e => e.PhoneAgentId == phoneOperatorId);

        if (scheduledDate.HasValue)
        {
            DateTime scheduledDatetime = scheduledDate.Value.ToDateTime(TimeOnly.MinValue);
            query = query.Where(e => e.ScheduledAt.Date == scheduledDatetime);
        }

        // Truthy value gets all completed calls
        // False value gets all non-completed calls
        if(filterByCompletionStatus.HasValue)
            query = query.Where(e => filterByCompletionStatus.Value 
                                     ? e.CompletedAt != null
                                     : e.CompletedAt == null);

        return await CursorPaginatedEnumerable<ScheduledCall, int>
            .CreateFromStrongEntityAsync(query, paginationCursor, pageSize);
    }

    public void Remove(ScheduledCall call)
    {
        _set.Remove(call);
    }
}
