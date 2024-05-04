using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.Attributes;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.Utilities;

namespace WaterFilterBusiness.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : Controller
{
    public UsersController(IServicesManager servicesManager) : base(servicesManager) { }

    [HasPermission(Permission.ReadUsers)]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [Required, Range(1, int.MaxValue)] int page,
        [Required, Range(1, int.MaxValue)] int pageSize)
    {
        var users = await _servicesManager.UsersService.GetAllAsync(page, pageSize);
        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> Create(User_AddRequestModel model)
    {
        var transactionResult = await _servicesManager.WrapInTransactionAsync(async () =>
        {
            var result = await _servicesManager.UsersService.AddAsync(model);

            if (result.IsFailed)
                return result;

            var newUser = result.Value;
            return await _servicesManager.UsersService.AddUserToRole(newUser.Id, model.Role);
        });

        return transactionResult.IsSuccess
               ? Created(string.Empty, transactionResult.Value)
               : BadRequest(transactionResult.GetErrorsDictionary());
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _servicesManager.UsersService.RemoveAsync(id);
        return result.IsSuccess ? NoContent() : NotFound();
    }
}
