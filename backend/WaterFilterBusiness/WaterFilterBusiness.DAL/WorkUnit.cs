using Microsoft.EntityFrameworkCore.Storage;

namespace WaterFilterBusiness.DAL;

public interface IWorkUnit
{
    Task SaveChanges();
    Task<IDbContextTransaction> BeginTransactionAsync();
}

internal sealed class WorkUnit : IWorkUnit
{
    private readonly AppDbContext _dbContext;

    public WorkUnit(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveChanges()
    {
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _dbContext.Database.BeginTransactionAsync();
    }
}
