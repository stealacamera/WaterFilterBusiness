using Microsoft.AspNetCore.Mvc;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.DTOs.Calls;
using WaterFilterBusiness.Common.Enums;

namespace WaterFilterBusiness.API.Controllers.Calls;

[Route("api/[controller]")]
[ApiController]
public class CustomerCallsController : Controller
{
    public CustomerCallsController(IServicesManager servicesManager) : base(servicesManager)
    {
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        int page, int pageSize,
        DateOnly? from = null, DateOnly? to = null,
        [FromQuery] CallOutcome? filterByOutcome = null)
    {
        var calls = await _servicesManager.CustomerCallsService
                                          .GetAllAsync(page, pageSize, from, to, filterByOutcome);

        foreach (var call in calls.Values)
        {
            var phoneAgent = (await _servicesManager.UsersService
                                                    .GetByIdAsync(call.PhoneAgent.Id))
                                                    .Value;

            call.PhoneAgent.Surname = phoneAgent.Surname;
            call.PhoneAgent.Username = phoneAgent.Username;
            call.PhoneAgent.Name = phoneAgent.Name;
        }

        return Ok(calls);
    }

    [HttpGet("customers/{customerId:int}")]
    public async Task<IActionResult> GetHistoryForCustomer(int customerId, int pageSize, int page = 1)
    {
        var result = await _servicesManager.CustomerCallsService
                                           .GetCallHistoryForCustomerAsync(customerId, page, pageSize);

        return result.IsFailed ? BadRequest(result) : Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CustomerCall_AddRequestModel customerCall)
    {
        var result = await _servicesManager.CustomerCallsService
                                           .CreateAsync(customerCall);

        return result.IsFailed ? BadRequest(result) : Created(string.Empty, result.Value);
    }
}
