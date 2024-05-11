using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using WaterFilterBusiness.BLL.Services;
using WaterFilterBusiness.BLL.Services.Calls;
using WaterFilterBusiness.BLL.Services.Customers;
using WaterFilterBusiness.BLL.Services.Finance;
using WaterFilterBusiness.BLL.Services.Identity;
using WaterFilterBusiness.BLL.Services.Inventory;
using WaterFilterBusiness.BLL.Services.Inventory.Items;
using WaterFilterBusiness.BLL.Services.Inventory.Requests;
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
    ICustomerCallsService CustomerCallsService { get; }
    IScheduledCallsService ScheduledCallsService { get; }

    #region Finance
    ISalesService SalesService { get; }
    IClientDebtsService ClientDebtsService { get; }
    ICommissionsService CommissionsService { get; }
    ICommissionRequestsService CommissionRequestsService { get; }
    #endregion

    #region Clients
    ICustomersService CustomersService { get; }
    ICustomerChangesService CustomerChangesService { get; }
    IClientMeetingsService ClientMeetings { get; }
    #endregion

    #region Inventory services
    IInventoryMovementsService InventoryMovementsService { get; }
    IInventoryPurchasesService InventoryPurchasesService { get; }

    #region Items
    IInventoryItemsService InventoryItemsService { get; }
    ITechnicianInventoryItemsService TechnicianInventoryItemsService { get; }
    ISmallInventoryItemsService SmallInventoryItemsService { get; }
    IBigInventoryItemsService BigInventoryItemsService { get; }
    #endregion

    #region Requests
    ITechnicianInventoryRequestsService TechnicianInventoryRequestsService { get; }
    ISmallInventoryRequestsService SmallInventoryRequestsService { get; }
    #endregion
    #endregion
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
        _utilityService = _serviceProvider.GetRequiredService<IUtilityService>();
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

    #region Finance
    private ISalesService _salesService;
    public ISalesService SalesService
    {
        get
        {
            _salesService ??= new SalesService(_workUnit, _utilityService);
            return _salesService;
        }
    }

    private ICommissionsService _commissionsService;
    public ICommissionsService CommissionsService
    {
        get
        {
            _commissionsService ??= new CommissionsService(_workUnit, _utilityService);
            return _commissionsService;
        }
    }

    private ICommissionRequestsService _commissionRequestsService;
    public ICommissionRequestsService CommissionRequestsService
    {
        get
        {
            _commissionRequestsService ??= new CommissionRequestsService(_workUnit, _utilityService);
            return _commissionRequestsService;
        }
    }
    #endregion

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

    #region Clients
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

    private IClientDebtsService _clientDebtsService;
    public IClientDebtsService ClientDebtsService
    {
        get
        {
            _clientDebtsService ??= new ClientDebtsService(_workUnit, _utilityService);
            return _clientDebtsService;
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
    #endregion

    private IScheduledCallsService _scheduledCallsService;
    public IScheduledCallsService ScheduledCallsService
    {
        get
        {
            _scheduledCallsService ??= new ScheduledCallsService(_workUnit, _utilityService);
            return _scheduledCallsService;
        }
    }

    #region Inventory
    private IInventoryMovementsService _inventoryMovementsService;
    public IInventoryMovementsService InventoryMovementsService
    {
        get
        {
            _inventoryMovementsService ??= new InventoryMovementsService(_workUnit, _utilityService);
            return _inventoryMovementsService;
        }
    }

    #region Items
    private IInventoryItemsService _inventoryItemsService;
    public IInventoryItemsService InventoryItemsService
    {
        get
        {
            _inventoryItemsService ??= new InventoryItemsService(_workUnit, _utilityService);
            return _inventoryItemsService;
        }
    }

    private ITechnicianInventoryItemsService _technicianInventoryItemsService;
    public ITechnicianInventoryItemsService TechnicianInventoryItemsService
    {
        get
        {
            _technicianInventoryItemsService ??= new TechnicianInventoryItemsService(_workUnit, _utilityService);
            return _technicianInventoryItemsService;
        }
    }

    private ISmallInventoryItemsService _smallInventoryItemsService;
    public ISmallInventoryItemsService SmallInventoryItemsService
    {
        get
        {
            _smallInventoryItemsService ??= new SmallInventoryItemsService(_workUnit, _utilityService);
            return _smallInventoryItemsService;
        }
    }

    private IBigInventoryItemsService _bigInventoryItemsService;
    public IBigInventoryItemsService BigInventoryItemsService
    {
        get
        {
            _bigInventoryItemsService ??= new BigInventoryItemsService(_workUnit, _utilityService);
            return _bigInventoryItemsService;
        }
    }

    private IInventoryPurchasesService _inventoryPurchasesService;
    public IInventoryPurchasesService InventoryPurchasesService
    {
        get
        {
            _inventoryPurchasesService ??= new InventoryPurchasesService(_workUnit, _utilityService);
            return _inventoryPurchasesService;
        }
    }
    #endregion

    #region Requests
    private ITechnicianInventoryRequestsService _technicianInventoryRequestsService;
    public ITechnicianInventoryRequestsService TechnicianInventoryRequestsService
    {
        get
        {
            _technicianInventoryRequestsService ??= new TechnicianInventoryRequestsService(_workUnit, _utilityService);
            return _technicianInventoryRequestsService;
        }
    }

    private ISmallInventoryRequestsService _smallInventoryRequestsService;
    public ISmallInventoryRequestsService SmallInventoryRequestsService
    {
        get
        {
            _smallInventoryRequestsService ??= new SmallInventoryRequestsService(_workUnit, _utilityService);
            return _smallInventoryRequestsService;
        }
    }

    #endregion
    #endregion
}
