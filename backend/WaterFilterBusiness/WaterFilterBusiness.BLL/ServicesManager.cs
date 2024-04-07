using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using WaterFilterBusiness.BLL.Services;
using WaterFilterBusiness.BLL.Services.Calls;
using WaterFilterBusiness.BLL.Services.Customers;
using WaterFilterBusiness.BLL.Services.Schedules;
using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL;

public interface IServicesManager
{
    Task<Result<T>> WrapInTransactionAsync<T>(Func<Task<Result<T>>> asyncFunc);

    #region Services
    IUsersService UsersService { get; }
    ISalesAgentSchedulesService SalesAgentSchedulesService { get; }
    ISalesAgentScheduleChangesService SalesAgentScheduleChangesService { get; }
    ICustomersService CustomersService { get; }
    ICustomerChangesService CustomerChangesService { get; }
    ICustomerCallsService CustomerCallsService { get; }
    IScheduledCallsService ScheduledCallsService { get; }
    IClientMeetingsService ClientMeetings { get; }
    #endregion
}

public sealed class ServicesManager : IServicesManager
{
    private static readonly SemaphoreSlim _asyncLock = new SemaphoreSlim(1, 1);

    private readonly IServiceProvider _serviceProvider;
    private readonly IWorkUnit _workUnit;
    private readonly IUtilityService _utilityService;

    public ServicesManager(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _workUnit = _serviceProvider.GetRequiredService<IWorkUnit>();

        _utilityService = new UtilityService(_workUnit);
    }

    public async Task<Result<T>> WrapInTransactionAsync<T>(Func<Task<Result<T>>> asyncFunc)
    {
        await _asyncLock.WaitAsync();

        using var transaction = await _workUnit.BeginTransactionAsync();
        Result<T> result;

        try
        {
            result = await asyncFunc();

            if (result.IsFailed)
                await transaction.RollbackAsync();
            else
                await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
        finally
        {
            _asyncLock.Release();
        }

        return result;
    }

    private IUsersService _usersService;
    public IUsersService UsersService
    {
        get
        {
            _usersService ??= new UsersService(_workUnit, _utilityService, _serviceProvider);
            return _usersService;
        }
    }

    private ISalesAgentSchedulesService _salesAgentSchedulesService;
    public ISalesAgentSchedulesService SalesAgentSchedulesService
    {
        get
        {
            _salesAgentSchedulesService ??= new SalesAgentSchedulesService(_workUnit, _utilityService);
            return _salesAgentSchedulesService;
        }
    }

    private ISalesAgentScheduleChangesService _salesAgentScheduleChangesService;
    public ISalesAgentScheduleChangesService SalesAgentScheduleChangesService
    {
        get
        {
            _salesAgentScheduleChangesService ??= new SalesAgentScheduleChangesService(_workUnit, _utilityService);
            return _salesAgentScheduleChangesService;
        }
    }

    private ICustomersService _customersService;
    public ICustomersService CustomersService
    {
        get
        {
            _customersService ??= new CustomersService(_workUnit, _utilityService);
            return _customersService;
        }
    }

    private ICustomerChangesService _customerChangesService;
    public ICustomerChangesService CustomerChangesService
    {
        get
        {
            _customerChangesService ??= new CustomerChangesService(_workUnit, _utilityService);
            return _customerChangesService;
        }
    }

    private ICustomerCallsService _customerCallsService;
    public ICustomerCallsService CustomerCallsService
    {
        get
        {
            _customerCallsService ??= new CustomerCallsService(_workUnit, _utilityService);
            return _customerCallsService;
        }
    }

    private IScheduledCallsService _scheduledCallsService;
    public IScheduledCallsService ScheduledCallsService
    {
        get
        {
            _scheduledCallsService ??= new ScheduledCallsService(_workUnit, _utilityService);
            return _scheduledCallsService;
        }
    }

    private IClientMeetingsService _clientMeetings;
    public IClientMeetingsService ClientMeetings
    {
        get
        {
            _clientMeetings ??= new ClientMeetingsService(_workUnit, _utilityService);
            return _clientMeetings;
        }
    }
}
