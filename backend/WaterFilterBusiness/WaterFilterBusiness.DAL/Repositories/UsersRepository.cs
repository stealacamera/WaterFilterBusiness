using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WaterFilterBusiness.DAL.DAOs;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL.Repository;

public interface IUsersRepository
{
    Task<OffsetPaginatedEnumerable<User>> GetAllAsync(int page, int pageSize, bool excludeDeleted = true);
    Task<User?> GetByIdAsync(int id);
    Task<IdentityResult> AddAsync(User user, string password);
    Task<IdentityResult> AddToRoleAsync(User user, Common.Enums.Role role);
    Task<Common.Enums.Role> GetRoleAsync(User user);
}

internal sealed class UsersRepository : IUsersRepository
{
    private readonly UserManager<User> _userManager;
    private readonly IQueryable<User> _untrackedSet;

    public UsersRepository(UserManager<User> userManager)
    {
        _userManager = userManager;
        _untrackedSet = _userManager.Users.AsNoTracking();
    }

    public async Task<OffsetPaginatedEnumerable<User>> GetAllAsync(int page, int pageSize, bool excludeDeleted = true)
    {
        var query = _untrackedSet;

        if (excludeDeleted)
            query = query.Where(e => e.DeletedAt == null);

        return await OffsetPaginatedEnumerable<User>.CreateAsync(query, page, pageSize);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _userManager.FindByIdAsync(id.ToString());
    }

    public async Task<IdentityResult> AddAsync(User user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    public async Task<IdentityResult> AddToRoleAsync(User user, Common.Enums.Role role)
    {
        return await _userManager.AddToRoleAsync(user, role.Name);
    }

    public async Task<Common.Enums.Role> GetRoleAsync(User user)
    {
        string roleName = (await _userManager.GetRolesAsync(user)).First();
        return Common.Enums.Role.FromName(roleName);
    }
}