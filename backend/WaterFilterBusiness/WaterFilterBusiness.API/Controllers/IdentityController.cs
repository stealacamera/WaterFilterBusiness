using Microsoft.AspNetCore.Mvc;
using WaterFilterBusiness.API.Common.Authentication;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.DTOs;

namespace WaterFilterBusiness.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IdentityController : Controller
{
    private readonly IJwtProvider _jwtProvider;

    public IdentityController(
        IServicesManager servicesManager,
        IJwtProvider jwtProvider) 
        : base(servicesManager)
    {
        _jwtProvider = jwtProvider;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCredentials credentials)
    {
        var userResult = await _servicesManager.UsersService
                                               .GetByCredentials(credentials);

        return userResult.IsFailed 
            ? BadRequest() 
            : Ok(_jwtProvider.Generate(userResult.Value));
    }
}
