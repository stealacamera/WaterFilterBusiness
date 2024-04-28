using Microsoft.AspNetCore.Authorization;
using WaterFilterBusiness.Common.Enums;

namespace WaterFilterBusiness.Common.Attributes;

public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(Permission permission) 
        : base(policy: permission.ToString())
    {
    }
}
