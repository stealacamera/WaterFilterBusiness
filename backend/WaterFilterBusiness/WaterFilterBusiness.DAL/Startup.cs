using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WaterFilterBusiness.DAL.Entities;
using WaterFilterBusiness.DAL.IdentityConfigurations;

namespace WaterFilterBusiness.DAL;

public static class Startup
{
    public static void RegisterDALServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserStore<User>, AppUserStore>();
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
                                                                    configuration.GetConnectionString("DbConnectionString"),
                                                                    e => e.UseDateOnlyTimeOnly()));

        services.AddIdentityCore<User>(options =>
                    {
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequireUppercase = false;

                        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/ ";
                        options.User.RequireUniqueEmail = true;

                        options.SignIn.RequireConfirmedAccount = false;
                        options.Lockout.AllowedForNewUsers = false;
                        options.SignIn.RequireConfirmedAccount = false;
                    })
                .AddRoles<Role>()
                .AddEntityFrameworkStores<AppDbContext>();

        services.AddScoped<IWorkUnit, WorkUnit>();
    }
}
