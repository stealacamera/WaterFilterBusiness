using FluentResults;
using Microsoft.AspNetCore.Mvc;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.Attributes;
using WaterFilterBusiness.Common.DTOs;
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
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicesManager.WrapInTransactionAsync<CustomerCall>(async () =>
        {
            var createCallResult = await _servicesManager.CustomerCallsService
                                               .CreateAsync(customerCall);

            if (createCallResult.IsFailed)
                return Result.Fail(createCallResult.Errors);

            var callOutcome = createCallResult.Value.Outcome;
            if (callOutcome == CallOutcome.NoAnswer || callOutcome == CallOutcome.Uninterested)
            {
                var scheduleCallResult = await _servicesManager.ScheduledCallsService
                                                             .CreateAsync(new ScheduledCall_AddRequestModel
                                                             {
                                                                 CustomerId = createCallResult.Value.Customer.Id,
                                                                 PhoneAgentId = createCallResult.Value.PhoneAgent.Id,
                                                                 ScheduledAt = DateTime.Now.AddMonths(1)
                                                             });

                if (scheduleCallResult.IsFailed)
                    return Result.Fail(scheduleCallResult.Errors);
            }
            else if (callOutcome == CallOutcome.Rescheduled)
            {
                var scheduleCallResult = await _servicesManager.ScheduledCallsService
                                                             .CreateAsync(new ScheduledCall_AddRequestModel
                                                             {
                                                                 CustomerId = createCallResult.Value.Customer.Id,
                                                                 PhoneAgentId = createCallResult.Value.PhoneAgent.Id,
                                                                 ScheduledAt = DateTime.Now.AddHours(1)
                                                             });

                if (scheduleCallResult.IsFailed)
                    return Result.Fail(scheduleCallResult.Errors);
            }
            else if (callOutcome == CallOutcome.RedList)
            {
                var redlistCustomerResult = await _servicesManager.CustomersService
                                                                  .UpdateAsync(
                                                                        createCallResult.Value.Customer.Id,
                                                                        new Customer_UpdateRequestModel
                                                                        {
                                                                            RedListedAt = DateTime.Now
                                                                        });

                if (redlistCustomerResult.IsFailed)
                    return Result.Fail(redlistCustomerResult.Errors);
            }

            return createCallResult.Value;
        });


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
