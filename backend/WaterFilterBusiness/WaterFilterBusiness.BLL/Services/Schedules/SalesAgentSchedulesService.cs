using FluentResults;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.ViewModels;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.Errors;
using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL.Services.Schedules;

public interface ISalesAgentSchedulesService
{
    Task<Result<IList<SalesAgentSchedule>>> GetAllByTimeAsync(Weekday? dayOfWeek = null, TimeOnly? time = null);
    Task<Result<SalesAgentSchedule>> GetByIdAsync(int id);
    Task<Result<SalesAgentSchedule>> CreateAsync(int salesAgentId, SalesAgentSchedule_AddRequestModel schedule);
    Task<Result<SalesAgentSchedule>> UpdateAsync(int id, SalesAgentSchedule_UpdateRequestModel schedule);
    Task<Result> RemoveAsync(int id);
    Task<Result<IList<SalesAgentWeekSchedules>>> GetAllForSalesAgentAsync(int salesAgentId);
}

internal class SalesAgentSchedulesService : Service, ISalesAgentSchedulesService
{
    public SalesAgentSchedulesService(
        IWorkUnit workUnit,
        IUtilityService utilityService) : base(workUnit, utilityService)
    {
    }

    public async Task<Result<SalesAgentSchedule>> CreateAsync(int salesAgentId, SalesAgentSchedule_AddRequestModel schedule)
    {
        if (!await _utilityService.DoesUserExistAsync(salesAgentId))
            return SalesAgentScheduleErrors.SalesAgentNotFound;

        var dbModel = new DAL.Entities.SalesAgentSchedule
        {
            BeginHour = schedule.BeginHour,
            EndHour = GenerateEndHour(schedule.BeginHour),
            DayOfWeekId = schedule.DayOfWeek.Value,
            SalesAgentId = salesAgentId
        };

        if (await _workUnit.SalesAgentSchedulesRepository.IsScheduleTakenForSalesAgentAsync(dbModel))
            return SalesAgentScheduleErrors.TimespanTaken(schedule.DayOfWeek.Name);

        dbModel = await _workUnit.SalesAgentSchedulesRepository.AddAsync(dbModel);
        await _workUnit.SaveChangesAsync();

        return new SalesAgentSchedule
        {
            Id = dbModel.Id,
            BeginHour = dbModel.BeginHour,
            EndHour = dbModel.EndHour,
            DayOfWeek = schedule.DayOfWeek
        };
    }

    public async Task<Result<IList<SalesAgentWeekSchedules>>> GetAllForSalesAgentAsync(int salesAgentId)
    {
        if (!await _utilityService.DoesUserExistAsync(salesAgentId))
            return SalesAgentScheduleErrors.SalesAgentNotFound;

        var dbSchedules = await _workUnit.SalesAgentSchedulesRepository
                                         .GetAllForSalesAgentAsync(salesAgentId);

        return dbSchedules.GroupBy(e => e.DayOfWeekId)
                          .Select(group =>
                          {
                              var weekDay = Weekday.FromValue(group.Key);

                              return new SalesAgentWeekSchedules
                              {
                                  DayOfWeek = weekDay,
                                  Schedules = group.Select(e => new SalesAgentSchedule
                                  {
                                      Id = e.Id,
                                      BeginHour = e.BeginHour,
                                      EndHour = e.EndHour,
                                      DayOfWeek = weekDay
                                  })
                                                   .ToList()
                              };
                          })
                          .ToList();
    }

    public async Task<Result<SalesAgentSchedule>> GetByIdAsync(int id)
    {
        var dbModel = await _workUnit.SalesAgentSchedulesRepository
                                     .GetByIdAsync(id);

        if (dbModel == null)
            return SalesAgentScheduleErrors.NotFound;

        return new SalesAgentSchedule
        {
            Id = dbModel.Id,
            BeginHour = dbModel.BeginHour,
            EndHour = dbModel.EndHour,
            DayOfWeek = Weekday.FromValue(dbModel.DayOfWeekId)
        };
    }

    public async Task<Result> RemoveAsync(int id)
    {
        var dbModel = await _workUnit.SalesAgentSchedulesRepository.GetByIdAsync(id);

        if (dbModel == null)
            return SalesAgentScheduleErrors.NotFound;

        _workUnit.SalesAgentSchedulesRepository.Remove(dbModel);
        await _workUnit.SaveChangesAsync();

        return Result.Ok();
    }

    public async Task<Result<SalesAgentSchedule>> UpdateAsync(int id, SalesAgentSchedule_UpdateRequestModel schedule)
    {
        if (schedule.BeginHour == null && schedule.DayOfWeek == null)
            return SalesAgentScheduleErrors.EmptyUpdate;

        var dbModel = await _workUnit.SalesAgentSchedulesRepository.GetByIdAsync(id);

        if (dbModel == null)
            return SalesAgentScheduleErrors.NotFound;

        if (IsChangedScheduleTheSameAsExisting(schedule, dbModel))
            return GeneralErrors.UnchangedUpdate;

        if (schedule.BeginHour.HasValue)
        {
            dbModel.BeginHour = schedule.BeginHour.Value;
            dbModel.EndHour = GenerateEndHour(schedule.BeginHour.Value);
        }

        if (schedule.DayOfWeek != null)
            dbModel.DayOfWeekId = schedule.DayOfWeek.Value;

        if (await _workUnit.SalesAgentSchedulesRepository.IsScheduleTakenForSalesAgentAsync(dbModel))
            return SalesAgentScheduleErrors.TimespanTaken(schedule.DayOfWeek.Name);

        await _workUnit.SaveChangesAsync();

        return new SalesAgentSchedule
        {
            Id = dbModel.Id,
            BeginHour = dbModel.BeginHour,
            EndHour = dbModel.EndHour,
            DayOfWeek = schedule.DayOfWeek
        };
    }

    private bool IsChangedScheduleTheSameAsExisting(SalesAgentSchedule_UpdateRequestModel updatedSchedule, DAL.Entities.SalesAgentSchedule originalSchedule)
    {
        return updatedSchedule.BeginHour != null &&
               updatedSchedule.BeginHour.Equals(originalSchedule.BeginHour) &&
               updatedSchedule.DayOfWeek != null &&
               updatedSchedule.DayOfWeek.Value.Equals(originalSchedule.DayOfWeekId);
    }

    private TimeOnly GenerateEndHour(TimeOnly beginHour)
    {
        return beginHour.AddHours(1).AddMinutes(30);
    }

    public async Task<Result<IList<SalesAgentSchedule>>> GetAllByTimeAsync(Weekday? dayOfWeek = null, TimeOnly? time = null)
    {
        if (dayOfWeek == null && !time.HasValue)
            return new Error("At least one parameter required");

        return (await _workUnit.SalesAgentSchedulesRepository
                               .GetAllByTimeAsync(dayOfWeek, time))
                               .Select(e => new SalesAgentSchedule
                               {
                                   BeginHour = e.BeginHour,
                                   DayOfWeek = dayOfWeek,
                                   EndHour = e.EndHour,
                                   Id = e.Id
                               })
                               .ToList();
    }
}
