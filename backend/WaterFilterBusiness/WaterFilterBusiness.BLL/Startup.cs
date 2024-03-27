using Microsoft.Extensions.DependencyInjection;

namespace WaterFilterBusiness.BLL;

public static class Startup
{
    public static void RegisterBLLService(this IServiceCollection services)
    {
        services.AddScoped<IServicesManager, ServicesManager>();
    }
}
