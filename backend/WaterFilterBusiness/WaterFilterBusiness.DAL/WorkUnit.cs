using Microsoft.EntityFrameworkCore.Storage;

namespace WaterFilterBusiness.DAL;

public interface IWorkUnit
{
    Task SaveChangesAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
}

internal sealed class WorkUnit
{
    private readonly AppDbContext _dbContext;

    public WorkUnit(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _dbContext.Database.BeginTransactionAsync();
    }
}
