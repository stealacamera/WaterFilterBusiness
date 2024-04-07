using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using WaterFilterBusiness.DAL.Entities;
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
    IUsersRepository UsersRepository { get; }
    ISalesAgentSchedulesRepository SalesAgentSchedulesRepository { get; }
    ISalesAgentScheduleChangesRepository SalesAgentScheduleChangesRepository { get; }
    ICustomersRepository CustomersRepository { get; }
    ICustomerChangesRepository CustomerChangesRepository { get; }
    IClientMeetingsRepository ClientMeetingsRepository { get; }
    ICustomerCallsRepository CustomerCallsRepository { get; }
    IScheduledCallsRepository ScheduledCallsRepository { get; }
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

    private IUsersRepository _usersRepository;
    public IUsersRepository UsersRepository
    {
        get
        {
            _usersRepository ??= new UsersRepository(_userManager);
            return _usersRepository;
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
}
