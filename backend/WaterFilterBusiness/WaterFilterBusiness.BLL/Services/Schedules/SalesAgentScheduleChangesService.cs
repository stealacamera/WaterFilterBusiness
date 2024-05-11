using FluentResults;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.ViewModels;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.ErrorHandling.Errors;
using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL.Services.Schedules;

public interface ISalesAgentScheduleChangesService
{
    Task<Result<SalesAgentScheduleChange>> CreateAsync(int scheduleId, SalesAgentSchedule newSchedule, SalesAgentSchedule oldSchedule);
    Task<Result<CursorPaginatedList<SalesAgentScheduleChange, int>>> GetAllForSalesAgentAsync(int salesAgentId, int paginationCursor, int pageSize);
    Task<Result> RemoveAllForScheduleAsync(int scheduleId);
}

internal class SalesAgentScheduleChangesService : Service, ISalesAgentScheduleChangesService
{
    public SalesAgentScheduleChangesService(
        IWorkUnit workUnit,
        IUtilityService utilityService) : base(workUnit, utilityService)
    {
    }

    public async Task<Result<SalesAgentScheduleChange>> CreateAsync(
        int scheduleId,
        SalesAgentSchedule newSchedule,
        SalesAgentSchedule oldSchedule)
    {
        if (!await _utilityService.DoesScheduleExistAsync(scheduleId))
            return SalesAgentScheduleErrors.NotFound(nameof(scheduleId));

        if (oldSchedule.Equals(newSchedule))
            return GeneralErrors.UnchangedUpdate;

        var dbModel = await _workUnit.SalesAgentScheduleChangesRepository
                                     .AddAsync(new DAL.Entities.SalesAgentScheduleChange
                                     {
                                         OldBeginHour = oldSchedule.BeginHour,
                                         OldEndHour = oldSchedule.EndHour,
                                         ScheduleId = scheduleId,
                                         ChangedAt = DateTime.Now,
                                         OldDayOfWeekId = oldSchedule.DayOfWeek.Value,
                                     });

        await _workUnit.SaveChangesAsync();

        return new SalesAgentScheduleChange
        {
            Id = dbModel.Id,
            ChangedAt = DateTime.Now,
            OldBeginHour = dbModel.OldBeginHour,
            OldEndHour = dbModel.OldEndHour,
            OldDayOfWeek = oldSchedule.DayOfWeek
        };
    }

    public async Task<Result<CursorPaginatedList<SalesAgentScheduleChange, int>>> GetAllForSalesAgentAsync(int salesAgentId, int paginationCursor, int pageSize)
    {
        if (!await _utilityService.DoesUserExistAsync(salesAgentId))
            return SalesAgentScheduleErrors.SalesAgentNotFound(nameof(salesAgentId));

        var dbValues = await _workUnit.SalesAgentScheduleChangesRepository
                               .GetAllForSalesAgentAsync(salesAgentId, paginationCursor, pageSize);

        return new CursorPaginatedList<SalesAgentScheduleChange, int>
        {
            Cursor = dbValues.Cursor,
            Values = dbValues.Values
                             .Select(e => new SalesAgentScheduleChange
                             {
                                 Id = e.Id,
                                 ChangedAt = e.ChangedAt,
                                 OldBeginHour = e.OldBeginHour,
                                 OldEndHour = e.OldEndHour,
                                 OldDayOfWeek = e.OldDayOfWeekId == null ? null : Weekday.FromValue(e.OldDayOfWeekId.Value)
                             })
                             .ToList()
        };
    }

    public async Task<Result> RemoveAllForScheduleAsync(int scheduleId)
    {
        if (!await _utilityService.DoesScheduleExistAsync(scheduleId))
            return SalesAgentScheduleErrors.NotFound(nameof(scheduleId));

        await _workUnit.SalesAgentScheduleChangesRepository.RemoveAllForScheduleAsync(scheduleId);
        await _workUnit.SaveChangesAsync();

        return Result.Ok();
    }
}
