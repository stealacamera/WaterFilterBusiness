using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using WaterFilterBusiness.BLL.Services;
using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL;

public interface IServicesManager
{
    Task<Result<T>> WrapInTransactionAsync<T>(Func<Task<Result<T>>> asyncFunc);
}

public sealed class ServicesManager : IServicesManager
{
    private static readonly SemaphoreSlim _asyncLock = new SemaphoreSlim(1, 1);

    private readonly IServiceProvider _serviceProvider;
    private readonly IWorkUnit _workUnit;

    public ServicesManager(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _workUnit = _serviceProvider.GetRequiredService<IWorkUnit>();
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
}
