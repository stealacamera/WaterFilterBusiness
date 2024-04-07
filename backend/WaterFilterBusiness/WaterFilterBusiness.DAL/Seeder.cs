using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL;

internal static class Seeder
{
    public static async Task SeedAdmin(UserManager<User> userManager, IWorkUnit workUnit)
    {
        var admin = new User
        {
            Name = "Base",
            Surname = "Admin",
            UserName = "base_admin",
            Email = "admin@gmail.com",
            EmailConfirmed = true
        };

        var doAdminsExistInSystem = (await userManager.GetUsersInRoleAsync(Common.Enums.Role.Admin.Name)).Any();

        if(!doAdminsExistInSystem)
        {
            await userManager.CreateAsync(admin, "@Password123");
            await workUnit.SaveChangesAsync();

            await userManager.AddToRoleAsync(admin, Common.Enums.Role.Admin.Name);
            await workUnit.SaveChangesAsync();
        }
    }
}
