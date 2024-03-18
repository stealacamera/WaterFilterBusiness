using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL;

public static class Startup
{
    public static void RegisterDALServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DbConnectionString")));

        services.AddIdentityCore<User>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.User.RequireUniqueEmail = true;
            options.Lockout.AllowedForNewUsers = false;
        })
                .AddRoles<Role>()
                .AddEntityFrameworkStores<AppDbContext>();

        services.AddScoped<IWorkUnit, WorkUnit>();
    }
}
