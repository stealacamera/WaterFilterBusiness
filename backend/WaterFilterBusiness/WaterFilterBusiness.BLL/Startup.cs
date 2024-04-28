using Microsoft.Extensions.DependencyInjection;
using WaterFilterBusiness.BLL.Services;
using WaterFilterBusiness.BLL.Services.Identity;

namespace WaterFilterBusiness.BLL;

public static class Startup
{
    public static void RegisterBLLServices(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddScoped<IPermissionsService, PermissionsService>();

        services.AddScoped<IUtilityService, UtilityService>();
        services.AddScoped<IServicesManager, ServicesManager>();

        services.AddProblemDetails();
    }
}
