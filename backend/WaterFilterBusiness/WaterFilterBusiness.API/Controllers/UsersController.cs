using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.Results;

namespace WaterFilterBusiness.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            var transactionResult = await _servicesManager.WrapInTransactionAsync(async () =>
            {
                var result = await _servicesManager.UsersService.AddAsync(model);

                if (result.IsFailed)
                    return result;

                var newUser = result.Value;
                return await _servicesManager.UsersService.AddUserToRole(newUser.Id, model.Role);
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

                return BadRequest(transactionResult);
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
