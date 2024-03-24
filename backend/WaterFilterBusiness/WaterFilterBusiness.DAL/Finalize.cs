using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL;

public static class Finalize
{
    public static async Task SeedAdmin(this IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateAsyncScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var workUnit = scope.ServiceProvider.GetRequiredService<IWorkUnit>();

            await Seeder.SeedAdmin(userManager, workUnit);
        }
    }
}
