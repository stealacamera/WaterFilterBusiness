using FluentResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.ViewModels;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.ErrorHandling.Errors;
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
            return SalesAgentScheduleErrors.SalesAgentNotFound(nameof(salesAgentId));

        var dbModel = new DAL.Entities.SalesAgentSchedule
        {
            BeginHour = schedule.BeginHour,
            EndHour = GenerateEndHour(schedule.BeginHour),
            DayOfWeekId = schedule.DayOfWeek.Value,
            SalesAgentId = salesAgentId
        };

        if (await _workUnit.SalesAgentSchedulesRepository.IsScheduleTakenForSalesAgentAsync(dbModel))
            return SalesAgentScheduleErrors.TimespanTaken(nameof(schedule.DayOfWeek), schedule.DayOfWeek.Name);

        try
        {
            dbModel = await _workUnit.SalesAgentSchedulesRepository.AddAsync(dbModel);
            await _workUnit.SaveChangesAsync();

            return ConvertEntityToModel(dbModel);
        }
        catch (Exception ex)
        {
            if (ex is DbUpdateException dbUpdateEx)
            {
                if (dbUpdateEx.InnerException != null && dbUpdateEx.InnerException is SqlException sqlEx)
                {
                    if (sqlEx.Number == 2627 && sqlEx.Number == 2601)
                        return SalesAgentScheduleErrors.UniqueConstraintFailed;
                }
            }

            throw;
        }
    }

    public async Task<Result<IList<SalesAgentWeekSchedules>>> GetAllForSalesAgentAsync(int salesAgentId)
    {
        if (!await _utilityService.DoesUserExistAsync(salesAgentId))
            return SalesAgentScheduleErrors.SalesAgentNotFound(nameof(salesAgentId));

        var dbSchedules = await _workUnit.SalesAgentSchedulesRepository
                                         .GetAllForSalesAgentAsync(salesAgentId);

        return dbSchedules.GroupBy(e => e.DayOfWeekId)
                          .Select(group => new SalesAgentWeekSchedules
                              {
                                  DayOfWeek = Weekday.FromValue(group.Key),
                                  Schedules = group.Select(ConvertEntityToModel).ToList()
                              })
                          .ToList();
    }

    public async Task<Result<SalesAgentSchedule>> GetByIdAsync(int id)
    {
        var dbModel = await _workUnit.SalesAgentSchedulesRepository
                                     .GetByIdAsync(id);

        if (dbModel == null)
            return SalesAgentScheduleErrors.NotFound(nameof(id));

        return ConvertEntityToModel(dbModel);
    }

    public async Task<Result> RemoveAsync(int id)
    {
        var dbModel = await _workUnit.SalesAgentSchedulesRepository.GetByIdAsync(id);

        if (dbModel == null)
            return SalesAgentScheduleErrors.NotFound(nameof(id));

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
            return SalesAgentScheduleErrors.NotFound(nameof(id));

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
            return SalesAgentScheduleErrors.TimespanTaken(nameof(schedule.DayOfWeek), schedule.DayOfWeek.Name);

        try
        {
            await _workUnit.SaveChangesAsync();
            return ConvertEntityToModel(dbModel);
        }
        catch (Exception ex)
        {
            if (ex is DbUpdateException dbUpdateEx)
            {
                if (dbUpdateEx.InnerException != null && dbUpdateEx.InnerException is SqlException sqlEx)
                {
                    if (sqlEx.Number == 2627 && sqlEx.Number == 2601)
                        return SalesAgentScheduleErrors.UniqueConstraintFailed;
                }
            }

            throw;
        }
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
                               .GetAllFreeSchedulesByTimeAsync(dayOfWeek, time))
                               .Select(ConvertEntityToModel)
                               .ToList();
    }

    private SalesAgentSchedule ConvertEntityToModel(DAL.Entities.SalesAgentSchedule entity)
    {
        return new SalesAgentSchedule
        {
            BeginHour = entity.BeginHour,
            EndHour = entity.EndHour,
            Id = entity.Id,
            DayOfWeek = Weekday.FromValue(entity.DayOfWeekId)
        };
    }
}
