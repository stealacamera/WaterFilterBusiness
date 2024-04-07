using Microsoft.Extensions.DependencyInjection;
using WaterFilterBusiness.BLL.Services;

namespace WaterFilterBusiness.BLL;

public static class Startup
{
    public static void RegisterBLLServices(this IServiceCollection services)
    {
        services.AddScoped<IServicesManager, ServicesManager>();
        
        services.AddProblemDetails();
    }
}
