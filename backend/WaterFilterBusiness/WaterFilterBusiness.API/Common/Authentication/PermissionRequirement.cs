using Microsoft.AspNetCore.Authorization;

namespace WaterFilterBusiness.API.Common.Authentication;

public class PermissionRequirement : IAuthorizationRequirement
{
    public PermissionRequirement(string permission)
        => Permission = permission;

    public string Permission { get; }
}