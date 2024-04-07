using Microsoft.EntityFrameworkCore;
using WaterFilterBusiness.DAL.DAOs;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL.Repository.Schedules;

public interface ISalesAgentScheduleChangesRepository : IRepository<SalesAgentScheduleChange, int>
{
    Task<CursorPaginatedEnumerable<SalesAgentScheduleChange, int>> GetAllForSalesAgentAsync(int salesAgentId, int paginationCursor, int pageSize);
    Task RemoveAllForScheduleAsync(int scheduleId);
}

internal class SalesAgentScheduleChangesRepository : Repository<SalesAgentScheduleChange, int>, ISalesAgentScheduleChangesRepository
{
    public SalesAgentScheduleChangesRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<CursorPaginatedEnumerable<SalesAgentScheduleChange, int>> GetAllForSalesAgentAsync(int salesAgentId, int paginationCursor, int pageSize)
    {
        IQueryable<SalesAgentScheduleChange> query = _untrackedSet.OrderByDescending(e => e.ChangedAt);
        query = query.Where(e => e.Schedule.SalesAgentId == salesAgentId);

        return await CursorPaginatedEnumerable<SalesAgentScheduleChange, int>.CreateFromStrongEntityAsync(query, paginationCursor, pageSize);
    }

    public async Task RemoveAllForScheduleAsync(int scheduleId)
    {
        IQueryable<SalesAgentScheduleChange> query = _untrackedSet.Where(e => e.ScheduleId == scheduleId);
        await query.ExecuteDeleteAsync();
    }
}
