using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL.IdentityConfigurations;

internal class AppUserStore : UserStore<User, Role, AppDbContext, int>
{
    public AppUserStore(AppDbContext context) : base(context)
    {
        AutoSaveChanges = false;
    }
}
