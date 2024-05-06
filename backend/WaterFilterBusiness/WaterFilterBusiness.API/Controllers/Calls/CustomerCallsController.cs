using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.Attributes;
using WaterFilterBusiness.Common.DTOs.Calls;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.Utilities;

namespace WaterFilterBusiness.API.Controllers.Calls;

[Route("api/[controller]")]
[ApiController]
public class CustomerCallsController : Controller
{
    public CustomerCallsController(IServicesManager servicesManager) : base(servicesManager)
    {
    }

    [HasPermission(Permission.ReadCustomerCalls)]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [Required, Range(1, int.MaxValue)] int page,
        [Required, Range(1, int.MaxValue)] int pageSize,
        DateOnly? from = null, DateOnly? to = null,
        [FromQuery] CallOutcome? filterByOutcome = null)
    {
        var calls = await _servicesManager.CustomerCallsService
                                          .GetAllAsync(page, pageSize, from, to, filterByOutcome);

        foreach (var call in calls.Values)
            await PopulateCall(call);

        return Ok(calls);
    }

    [HasPermission(Permission.ReadCustomerCalls)]
    [HttpGet("customers/{customerId:int}/history")]
    public async Task<IActionResult> GetHistoryForCustomer(
        int customerId,
        [Required, Range(1, int.MaxValue)] int pageSize,
        [Required, Range(1, int.MaxValue)] int page)
    {
        var result = await _servicesManager.CustomerCallsService
                                           .GetCallHistoryForCustomerAsync(customerId, page, pageSize);

        if (result.IsFailed)
            return BadRequest(result.GetErrorsDictionary());

        var calls = result.Value;

        foreach (var call in calls.Values)
            await PopulateCall(call);

        return Ok(calls);
    }

    [HasPermission(Permission.CreateCustomerCalls)]
    [HttpPost]
    public async Task<IActionResult> Create(CustomerCall_AddRequestModel customerCall)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicesManager.CustomerCallsService
                                           .CreateAsync(customerCall);

        if (result.IsFailed)
            return BadRequest(result.GetErrorsDictionary());

        await PopulateCall(result.Value);
        return Created(string.Empty, result.Value);
    }

    private async Task PopulateCall(CustomerCall call)
    {
        var phoneAgent = (await _servicesManager.UsersService
                                                .GetByIdAsync(call.PhoneAgent.Id))
                                                .Value;

        call.PhoneAgent.Surname = phoneAgent.Surname;
        call.PhoneAgent.Username = phoneAgent.Username;
        call.PhoneAgent.Name = phoneAgent.Name;
    }
}
