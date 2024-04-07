using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL.Services;

internal interface IUtilityService
{
    Task<bool> DoesUserExistAsync(int id);
    Task<bool> IsUserInRoleAsync(int id, Role role);

    Task<bool> DoesScheduleExistAsync(int id);
    
    Task<bool> DoesCustomerExistAsync(int id);
    Task<bool> DoesCustomerHaveAScheduledCallAsync(int customerId);
    Task<bool> IsCustomerRedListedAsync(int customerId);
    Task<bool> IsTimespanWithinSalesAgentScheduleAsync(int salesAgentId, DateTime scheduledAt);
}

internal sealed class UtilityService : IUtilityService
{
    private readonly IWorkUnit _workUnit;

    public UtilityService(IWorkUnit workUnit) => _workUnit = workUnit;

    public async Task<bool> DoesCustomerExistAsync(int id) => 
        (await _workUnit.CustomersRepository.GetByIdAsync(id)) != null;
    
    public async Task<bool> DoesCustomerHaveAScheduledCallAsync(int customerId)
    {
        if (!await DoesCustomerExistAsync(customerId))
            return false;

        return await _workUnit.ScheduledCallsRepository
                              .DoesCustomerHaveAny(customerId);
    }

    public async Task<bool> DoesScheduleExistAsync(int id) => 
        (await _workUnit.SalesAgentSchedulesRepository.GetByIdAsync(id)) != null;

    public async Task<bool> DoesUserExistAsync(int id) =>
        (await _workUnit.UsersRepository.GetByIdAsync(id)) != null;

    public async Task<bool> IsCustomerRedListedAsync(int customerId)
    {
        var dbModel = await _workUnit.CustomersRepository.GetByIdAsync(customerId);
        return dbModel?.RedListedAt != null;
    }

    public async Task<bool> IsTimespanWithinSalesAgentScheduleAsync(int salesAgentId, DateTime scheduledAt)
    {
        return await _workUnit.SalesAgentSchedulesRepository
                              .IsScheduleTakenForSalesAgentAsync(new DAL.Entities.SalesAgentSchedule
                              {
                                  SalesAgentId = salesAgentId,
                                  BeginHour = TimeOnly.FromDateTime(scheduledAt),
                                  EndHour = TimeOnly.FromDateTime(scheduledAt).AddMinutes(40),
                                  DayOfWeekId = Weekday.FromName(scheduledAt.DayOfWeek.ToString(), ignoreCase: true)
                              });
    }

    public async Task<bool> IsUserInRoleAsync(int id, Role role)
    {
        var dbModel = await _workUnit.UsersRepository.GetByIdAsync(id);

        if (dbModel == null)
            return false;

        return (await _workUnit.UsersRepository
                               .GetRoleAsync(dbModel))
                               .Equals(role);
    }
}
