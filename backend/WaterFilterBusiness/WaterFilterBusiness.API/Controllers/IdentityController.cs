using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using WaterFilterBusiness.API.Common.Authentication;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.DTOs;

namespace WaterFilterBusiness.API.Controllers;

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

    [HttpPost("/login")]
    public async Task<IActionResult> Login(LoginCredentials credentials)
    {
        var userResult = await _servicesManager.UsersService
                                               .GetByCredentials(credentials);

        return userResult.IsFailed 
            ? Unauthorized() 
            : Ok(await _jwtProvider.GenerateAsync(userResult.Value));
    }
}
