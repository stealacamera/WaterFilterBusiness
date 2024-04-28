using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using WaterFilterBusiness.DAL.Entities;
using WaterFilterBusiness.DAL.Repositories;
using WaterFilterBusiness.DAL.Repositories.Finance;
using WaterFilterBusiness.DAL.Repositories.Inventory;
using WaterFilterBusiness.DAL.Repositories.Inventory.Items;
using WaterFilterBusiness.DAL.Repositories.Inventory.Requests;
using WaterFilterBusiness.DAL.Repository;
using WaterFilterBusiness.DAL.Repository.Calls;
using WaterFilterBusiness.DAL.Repository.Customers;
using WaterFilterBusiness.DAL.Repository.Schedules;

namespace WaterFilterBusiness.DAL;

public interface IWorkUnit
{
    Task SaveChangesAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();

    #region Repositories
    #region Identity
    IUsersRepository UsersRepository { get; }
    IRolePermissionsRepository RolePermissionsRepository { get; }
    #endregion
    ISalesRepository SalesRepository { get; }
    ISalesAgentSchedulesRepository SalesAgentSchedulesRepository { get; }
    ISalesAgentScheduleChangesRepository SalesAgentScheduleChangesRepository { get; }
    ICustomerCallsRepository CustomerCallsRepository { get; }
    IScheduledCallsRepository ScheduledCallsRepository { get; }

    #region Clients
    ICustomersRepository CustomersRepository { get; }
    ICustomerChangesRepository CustomerChangesRepository { get; }
    IClientMeetingsRepository ClientMeetingsRepository { get; }
    IClientDebtsRepository ClientDebtsRepository { get; }
    #endregion

    #region Inventory
    IInventoryPurchasesRepository InventoryPurchasesRepository { get; }
    IInventoryMovementsRepository InventoryMovementsRepository { get; }

    #region Items
    IInventoryItemsRepository InventoryItemsRepository { get; }
    IBigInventoryItemsRepository BigInventoryItemsRepository { get; }
    ISmallInventoryItemsRepository SmallInventoryItemsRepository { get; }
    ITechnicianInventoryItemsRepository TechnicianInventoryItemsRepository { get; }
    #endregion
    #endregion

    #region Requests
    IInventoryRequestsRepository InventoryRequestsRepository { get; }
    ISmallInventoryRequestsRepository SmallInventoryRequestsRepository { get; }
    ITechnicianInventoryRequestsRepository TechnicianInventoryRequestsRepository { get; }
    #endregion
    #endregion
}

internal sealed class WorkUnit : IWorkUnit
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<User> _userManager;

    public WorkUnit(IServiceProvider serviceProvider)
    {
        _dbContext = serviceProvider.GetRequiredService<AppDbContext>();
        _userManager = serviceProvider.GetRequiredService<UserManager<User>>();
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _dbContext.Database.BeginTransactionAsync();
    }

    #region Identity
    private IUsersRepository _usersRepository;
    public IUsersRepository UsersRepository
    {
        get
        {
            _usersRepository ??= new UsersRepository(_userManager);
            return _usersRepository;
        }
    }

    private IRolePermissionsRepository _rolePermissionsRepository;
    public IRolePermissionsRepository RolePermissionsRepository
    {
        get
        {
            _rolePermissionsRepository ??= new RolePermissionsRepository(_dbContext);
            return _rolePermissionsRepository;
        }
    }
    #endregion

    private ISalesRepository _salesRepository;
    public ISalesRepository SalesRepository
    {
        get
        {
            _salesRepository ??= new SalesRepository(_dbContext);
            return _salesRepository;
        }
    }

    private ISalesAgentSchedulesRepository _salesAgentSchedulesRepository;
    public ISalesAgentSchedulesRepository SalesAgentSchedulesRepository
    {
        get
        {
            _salesAgentSchedulesRepository ??= new SalesAgentSchedulesRepository(_dbContext);
            return _salesAgentSchedulesRepository;
        }
    }

    private ISalesAgentScheduleChangesRepository _salesAgentScheduleChangesRepository;
    public ISalesAgentScheduleChangesRepository SalesAgentScheduleChangesRepository
    {
        get
        {
            _salesAgentScheduleChangesRepository ??= new SalesAgentScheduleChangesRepository(_dbContext);
            return _salesAgentScheduleChangesRepository;
        }
    }

    private ICustomerCallsRepository _customerCallsRepository;
    public ICustomerCallsRepository CustomerCallsRepository
    {
        get
        {
            _customerCallsRepository ??= new CustomerCallsRepository(_dbContext);
            return _customerCallsRepository;
        }
    }

    private IScheduledCallsRepository _scheduledCallsRepository;
    public IScheduledCallsRepository ScheduledCallsRepository
    {
        get
        {
            _scheduledCallsRepository ??= new ScheduledCallsRepository(_dbContext);
            return _scheduledCallsRepository;
        }
    }

    #region Clients
    private ICustomersRepository _customersRepository;
    public ICustomersRepository CustomersRepository
    {
        get
        {
            _customersRepository ??= new CustomersRepository(_dbContext);
            return _customersRepository;
        }
    }

    private ICustomerChangesRepository _customerChangesRepository;
    public ICustomerChangesRepository CustomerChangesRepository
    {
        get
        {
            _customerChangesRepository ??= new CustomerChangesRepository(_dbContext);
            return _customerChangesRepository;
        }
    }

    private IClientMeetingsRepository _clientMeetingsRepository;
    public IClientMeetingsRepository ClientMeetingsRepository
    {
        get
        {
            _clientMeetingsRepository ??= new ClientMeetingsRepository(_dbContext);
            return _clientMeetingsRepository;
        }
    }

    private IClientDebtsRepository _clientDebtsRepository;
    public IClientDebtsRepository ClientDebtsRepository
    {
        get
        {
            _clientDebtsRepository ??= new ClientDebtsRepository(_dbContext);
            return _clientDebtsRepository;
        }
    }
    #endregion


    #region Inventory repositories
    private IInventoryMovementsRepository _inventoryMovementsRepository;
    public IInventoryMovementsRepository InventoryMovementsRepository
    {
        get
        {
            _inventoryMovementsRepository ??= new InventoryMovementsRepository(_dbContext);
            return _inventoryMovementsRepository;
        }
    }

    private IInventoryPurchasesRepository _inventoryPurchasesRepository;
    public IInventoryPurchasesRepository InventoryPurchasesRepository
    {
        get
        {
            _inventoryPurchasesRepository ??= new InventoryPurchasesRepository(_dbContext);
            return _inventoryPurchasesRepository;
        }
    }

    #region Item repositories
    private IInventoryItemsRepository _inventoryItemsRepository;
    public IInventoryItemsRepository InventoryItemsRepository
    {
        get
        {
            _inventoryItemsRepository ??= new InventoryItemsRepository(_dbContext);
            return _inventoryItemsRepository;
        }
    }

    private IBigInventoryItemsRepository _bigInventoryItemsRepository;
    public IBigInventoryItemsRepository BigInventoryItemsRepository
    {
        get
        {
            _bigInventoryItemsRepository ??= new BigInventoryItemsRepository(_dbContext);
            return _bigInventoryItemsRepository;
        }
    }

    private ISmallInventoryItemsRepository _smallInventoryItemsRepository;
    public ISmallInventoryItemsRepository SmallInventoryItemsRepository
    {
        get
        {
            _smallInventoryItemsRepository ??= new SmallInventoryItemsRepository(_dbContext);
            return _smallInventoryItemsRepository;
        }
    }

    private ITechnicianInventoryItemsRepository _technicianInventoryItemsRepository;
    public ITechnicianInventoryItemsRepository TechnicianInventoryItemsRepository
    {
        get
        {
            _technicianInventoryItemsRepository ??= new TechnicianInventoryItemsRepository(_dbContext);
            return _technicianInventoryItemsRepository;
        }
    }
    #endregion

    #region Requests
    private IInventoryRequestsRepository _inventoryRequestsRepository;
    public IInventoryRequestsRepository InventoryRequestsRepository
    {
        get
        {
            _inventoryRequestsRepository ??= new InventoryRequestsRepository(_dbContext);
            return _inventoryRequestsRepository;
        }
    }

    private ISmallInventoryRequestsRepository _smallInventoryRequestsRepository;
    public ISmallInventoryRequestsRepository SmallInventoryRequestsRepository
    {
        get
        {
            _smallInventoryRequestsRepository ??= new SmallInventoryRequestsRepository(_dbContext);
            return _smallInventoryRequestsRepository;
        }
    }

    private ITechnicianInventoryRequestsRepository _technicianInventoryRequestsRepository;
    public ITechnicianInventoryRequestsRepository TechnicianInventoryRequestsRepository
    {
        get
        {
            _technicianInventoryRequestsRepository ??= new TechnicianInventoryRequestsRepository(_dbContext);
            return _technicianInventoryRequestsRepository;
        }
    }
    #endregion
    #endregion
}