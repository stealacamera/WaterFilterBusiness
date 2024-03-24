using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using WaterFilterBusiness.DAL.Entities;
using WaterFilterBusiness.DAL.Repositories;

namespace WaterFilterBusiness.DAL;

public interface IWorkUnit
{
    #region Repositories
    IUsersRepository UsersRepository { get; }
    ICustomersRepository CustomersRepository { get; }
    #endregion

    Task SaveChangesAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
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

    private ICustomersRepository _customersRepository;
    public ICustomersRepository CustomersRepository
    {
        get
        {
            _customersRepository ??= new CustomersRepository(_dbContext);
            return _customersRepository;
        }
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
}
