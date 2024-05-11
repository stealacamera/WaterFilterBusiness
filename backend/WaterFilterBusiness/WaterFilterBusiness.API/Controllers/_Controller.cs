using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using WaterFilterBusiness.BLL;

namespace WaterFilterBusiness.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public abstract class Controller : ControllerBase
{
    protected readonly IServicesManager _servicesManager;

    public Controller(IServicesManager servicesManager) 
          => _servicesManager = servicesManager;

    protected int GetCurrentUserId()
    {
        string? userId = User.Claims
                             .FirstOrDefault(e => e.Type == JwtRegisteredClaimNames.Sub)?
                             .Value;

        if (!int.TryParse(userId, out int parsedUserId))
            return 0;
            
        return parsedUserId;
    }
    
    protected IActionResult GetResult<T>(Result<T> result, IActionResult onSuccess)
        {
            return result.IsFailed ? BadRequest(result) : onSuccess;
        }
}