using Microsoft.EntityFrameworkCore;
using WaterFilterBusiness.DAL.Entities;
using WaterFilterBusiness.DAL.Repository;

namespace WaterFilterBusiness.DAL.Repositories;

public interface IRolePermissionsRepository
{
    Task<IList<RolePermission>> GetAllForRoleAsync(int roleId);
}

internal class RolePermissionsRepository : Repository<RolePermission>, IRolePermissionsRepository
{
    public RolePermissionsRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IList<RolePermission>> GetAllForRoleAsync(int roleId)
    {
        IQueryable<RolePermission> query = _untrackedSet;
        query = query.Where(e => e.RoleId == roleId);

        return await query.ToListAsync();
    }
}
