using Quartz;
using WaterFilterBusiness.BLL;

namespace WaterFilterBusiness.API.Common.Quartz.Jobs;

[DisallowConcurrentExecution]
public class RemoveCustomersFromRedlistedJob : IJob
{
    private readonly IServicesManager _servicesManager;

    public RemoveCustomersFromRedlistedJob(IServicesManager servicesManager)
    {
        _servicesManager = servicesManager;
    }

    public Task Execute(IJobExecutionContext context)
    {
        _servicesManager.CustomersService.RemoveFromRedlistedAsync();
        return Task.CompletedTask;
    }
}
