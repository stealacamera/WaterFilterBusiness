using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL.Services;

internal interface IUtilityService
{
    Task<bool> DoesUserExistAsync(int id);
    Task<bool> IsUserInRoleAsync(int id, Role role);
    Task<Role?> GetUserRoleAsync(int userId);

    Task<bool> DoesScheduleExistAsync(int id);

    Task<bool> DoesCustomerExistAsync(int id);
    Task<bool> DoesCustomerHaveAScheduledCallAsync(int customerId);
    Task<bool> IsCustomerRedListedAsync(int customerId);
    Task<bool> IsTimespanWithinSalesAgentScheduleAsync(int salesAgentId, DateTime scheduledAt);

    Task<bool> DoesInventoryItemExistAsync(int id);
    Task<int> GetSmallInventoryItemQuantityAsync(int toolId);
    Task<int> GetBigInventoryItemQuantityAsync(int toolId);
    Task<decimal?> GetInventoryItemPriceAsync(int toolId);

    Task<bool> DoesMeetingExistAsync(int meetingId);

    Task<Sale?> GetSaleById(int saleId);
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

    public async Task<bool> DoesInventoryItemExistAsync(int id) =>
        (await _workUnit.InventoryItemsRepository.GetByIdAsync(id)) != null;

    public async Task<bool> DoesMeetingExistAsync(int meetingId)
        => (await _workUnit.SalesRepository.GetByIdAsync(meetingId)) != null;

    public async Task<bool> DoesScheduleExistAsync(int id) =>
        (await _workUnit.SalesAgentSchedulesRepository.GetByIdAsync(id)) != null;

    public async Task<bool> DoesUserExistAsync(int id) =>
        (await _workUnit.UsersRepository.GetByIdAsync(id)) != null;

    public async Task<int> GetBigInventoryItemQuantityAsync(int toolId)
    {
        var item = await _workUnit.BigInventoryItemsRepository.GetByIdAsync(toolId);
        return item?.Quantity ?? 0;
    }

    public async Task<decimal?> GetInventoryItemPriceAsync(int toolId)
    {
        var item = await _workUnit.InventoryItemsRepository.GetByIdAsync(toolId);
        return item?.Price ?? null;
    }

    public async Task<int> GetSmallInventoryItemQuantityAsync(int toolId)
    {
        var item = await _workUnit.SmallInventoryItemsRepository.GetByIdAsync(toolId);
        return item?.Quantity ?? 0;
    }

    public async Task<Role?> GetUserRoleAsync(int userId)
    {
        var user = await _workUnit.UsersRepository.GetByIdAsync(userId);

        if (user == null)
            return null;

        return await _workUnit.UsersRepository.GetRoleAsync(user);
    }

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

    public async Task<Sale?> GetSaleById(int saleId)
    {
        var sale = await _workUnit.SalesRepository.GetByIdAsync(saleId);

        return sale == null ? null : new Sale
        {
            CreatedAt = sale.CreatedAt,
            Meeting = new ClientMeeting_BriefDescription { Id = sale.MeetingId },
            PaymentType = PaymentType.FromValue(sale.PaymentTypeId),
            TotalAmount = sale.TotalAmount,
            UpfrontPaymentAmount = sale.UpfrontPaymentAmount,
            VerificationNote = sale.VerificationNote,
            VerifiedAt = sale.VerifiedAt
        };
    }
}
