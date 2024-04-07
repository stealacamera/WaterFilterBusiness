using Microsoft.EntityFrameworkCore;
using WaterFilterBusiness.DAL.DAOs;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL.Repository.Calls;

public interface IScheduledCallsRepository : IRepository<ScheduledCall, int>
{
    Task<bool> DoesCustomerHaveAny(int customerId);
    void Remove(ScheduledCall call);
    Task<CursorPaginatedEnumerable<ScheduledCall, int>> GetAllForPhoneOperator(
        int phoneOperatorId, 
        int paginationCursor, 
        int pageSize, 
        DateOnly? scheduledFor = null);
}

internal class ScheduledCallsRepository : Repository<ScheduledCall, int>, IScheduledCallsRepository
{
    public ScheduledCallsRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<bool> DoesCustomerHaveAny(int customerId)
    {
        IQueryable<ScheduledCall> query = _untrackedSet.Where(e => e.CustomerId == customerId);
        return await query.AnyAsync();
    }

    public async Task<CursorPaginatedEnumerable<ScheduledCall, int>> GetAllForPhoneOperator(
        int phoneOperatorId, 
        int paginationCursor, 
        int pageSize, 
        DateOnly? scheduledFor = null)
    {
        IQueryable<ScheduledCall> query = _untrackedSet.OrderBy(e => e.ScheduledAt);          
        query = query.Where(e => e.PhoneAgentId == phoneOperatorId);

        if (scheduledFor.HasValue)
        {
            DateTime scheduledDatetime = scheduledFor.Value.ToDateTime(TimeOnly.MinValue);
            query = query.Where(e => e.ScheduledAt.Date == scheduledDatetime);
        }

        return await CursorPaginatedEnumerable<ScheduledCall, int>.CreateFromStrongEntityAsync(query, paginationCursor, pageSize);
    }

    public void Remove(ScheduledCall call)
    {
        _set.Remove(call);
    }
}
