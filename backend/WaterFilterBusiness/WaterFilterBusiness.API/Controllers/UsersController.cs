using FluentResults;
using Microsoft.AspNetCore.Mvc;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.Results;

namespace WaterFilterBusiness.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // TODO [authorized only to admins]
    public class UsersController : Controller
    {
        public UsersController(IServicesManager servicesManager) : base(servicesManager)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int pageSize, int page = 1)
        {
            var result = await _servicesManager.UsersService.GetAllAsync(page, pageSize);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserAddRequestModel model)
        {
            // Check if role given exists
            Role? role = null;

            if (!Role.TryFromName(model.Role, ignoreCase: true, out role))
                ModelState.AddModelError("Role", "Invalid value");

            if (!ModelState.IsValid)
                return BadRequest(new ValidationProblemDetails(ModelState));

            var transactionResult = await _servicesManager.WrapInTransactionAsync(async () =>
            {
                var result = await _servicesManager.UsersService.AddAsync(model);

                if (result.IsFailed)
                    return result;

                var newUser = result.Value;
                await _servicesManager.UsersService.AddUserToRole(newUser.Id, role);

                newUser.Role = role.Name;
                return Result.Ok(newUser);
            });

            if (transactionResult.IsSuccess)
                return Created(string.Empty, transactionResult.Value);
            else
            {
                if (transactionResult is IdentityErrorsResult)
                {
                    var errors = ((IdentityErrorsResult)transactionResult).BaseErrors
                                                                          .ToDictionary(
                                                                                e => e.Key, 
                                                                                e => e.Value.ToArray());

                    return BadRequest(new ValidationProblemDetails(errors));
                }

                return BadRequest();
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _servicesManager.UsersService.RemoveAsync(id);

            return result.IsSuccess ? NoContent() : NotFound();
        }
    }
}
