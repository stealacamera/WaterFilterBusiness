using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using WaterFilterBusiness.BLL.Services.Identity;
using WaterFilterBusiness.Common.Enums;

namespace WaterFilterBusiness.API.Common.Authentication;

public class PermissionAuthorizationHandler
    : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        string? userId = context.User
                                .Claims
                                .FirstOrDefault(e => e.Type == JwtRegisteredClaimNames.Sub)?.Value;

        if (!int.TryParse(userId, out int parsedUserId))
            return;

        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        IPermissionsService permissionsService = scope.ServiceProvider
                                                      .GetRequiredService<IPermissionsService>();

        var permissions = await permissionsService.GetAllForUserAsync(parsedUserId);

        if (permissions.IsSuccess)
        {
            Permission requestPermission = (Permission)Enum.Parse(typeof(Permission), requirement.Permission);

            if (permissions.Value.Contains(requestPermission))
            {
                context.Succeed(requirement);
                return;
            }
        }

        context.Fail();
    }
}
