using Microsoft.EntityFrameworkCore;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL.Repository.Schedules;

public interface ISalesAgentSchedulesRepository : ISimpleRepository<SalesAgentSchedule, int>
{
    Task<IEnumerable<SalesAgentSchedule>> GetAllFreeSchedulesByTimeAsync(Weekday? dayOfWeek, TimeOnly? time);
    Task<bool> IsScheduleTakenForSalesAgentAsync(SalesAgentSchedule schedule);
    Task<IEnumerable<SalesAgentSchedule>> GetAllForSalesAgentAsync(int salesAgentId);
    void Remove(SalesAgentSchedule schedule);
}

internal class SalesAgentSchedulesRepository : SimpleRepository<SalesAgentSchedule, int>, ISalesAgentSchedulesRepository
{
    public SalesAgentSchedulesRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<SalesAgentSchedule>> GetAllForSalesAgentAsync(int salesAgentId)
    {
        IQueryable<SalesAgentSchedule> query = _untrackedSet.Where(e => e.SalesAgentId == salesAgentId);

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<SalesAgentSchedule>> GetAllFreeSchedulesByTimeAsync(Weekday? dayOfWeek, TimeOnly? time)
    {
        IQueryable<SalesAgentSchedule> query = _untrackedSet;

        // Get all in the given date
        if (dayOfWeek != null)
            query = query.Where(e => e.DayOfWeekId == dayOfWeek.Value);

        // Get the schedules that fit the given time
        if (time.HasValue)
            query = query.Where(e => e.BeginHour >= time && e.EndHour <= time);

        // Get all the sales agents that are free in this given period
        query = query.Where(e => e.SalesAgent
                                  .ClientMeetings
                                  .Any(e => e.MeetingOutcomeId == null));

        return await query.ToListAsync();
    }

    public async Task<bool> IsScheduleTakenForSalesAgentAsync(SalesAgentSchedule schedule)
    {
        IQueryable<SalesAgentSchedule> query = _untrackedSet.Where(e => e.SalesAgentId == schedule.SalesAgentId);
        query = query.Where(e => e.DayOfWeekId == schedule.DayOfWeekId);

        if (schedule.Id != 0)
            query = query.Where(e => e.Id != schedule.Id);

        // Checks if timespan is taken:
        // Checks if this schedule falls inside an existing one
        // or encapsulates an existing one
        return await query.AnyAsync(e => (schedule.BeginHour >= e.BeginHour && schedule.BeginHour <= e.EndHour) ||
                                         (schedule.EndHour >= e.BeginHour && schedule.EndHour <= e.EndHour) ||
                                         schedule.BeginHour <= e.BeginHour && schedule.EndHour >= e.EndHour);
    }

    public void Remove(SalesAgentSchedule schedule)
    {
        _set.Remove(schedule);
    }
}
