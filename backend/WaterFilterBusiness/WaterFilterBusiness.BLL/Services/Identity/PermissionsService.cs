using FluentResults;
using Microsoft.Extensions.Caching.Memory;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.Errors;
using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL.Services.Identity;

public interface IPermissionsService
{
    Task<Result<HashSet<Permission>>> GetAllForUserAsync(int userId);
}

internal class PermissionsService : Service, IPermissionsService
{
    private readonly IMemoryCache _memoryCache;
    private const string PERMISSIONS_CACHE_KEY = "_permissions_";

    public PermissionsService(
        IWorkUnit workUnit,
        IUtilityService utilityService,
        IMemoryCache memoryCache)
        : base(workUnit, utilityService)
    {
        _memoryCache = memoryCache;
    }

    public async Task<Result<HashSet<Permission>>> GetAllForUserAsync(int userId)
    {
        if (!await _utilityService.DoesUserExistAsync(userId))
            return UserErrors.NotFound;

        var permissions = GetPermissionsFromCache();

        if (!permissions.ContainsKey(userId))
        {
            var role = await _utilityService.GetUserRoleAsync(userId);
            var userPermissions = await _workUnit.RolePermissionsRepository
                                                 .GetAllForRoleAsync(role.Value);

            permissions.Add(userId, userPermissions.Select(e => e.PermissionId).ToHashSet());
            _memoryCache.Set(PERMISSIONS_CACHE_KEY, permissions);
        }

        return permissions[userId].Select(e => (Permission)e).ToHashSet();
    }

    private Dictionary<int, HashSet<int>>? GetPermissionsFromCache()
    {
        return _memoryCache.GetOrCreate(
                    PERMISSIONS_CACHE_KEY,
                    (entry) =>
                    {
                        entry.Priority = CacheItemPriority.NeverRemove;
                        entry.SlidingExpiration = TimeSpan.FromHours(2);

                        return new Dictionary<int, HashSet<int>>();
                    });
    }
}
