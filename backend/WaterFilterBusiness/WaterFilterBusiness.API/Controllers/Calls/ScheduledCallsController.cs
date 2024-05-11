using FluentResults;
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
public class ScheduledCallsController : Controller
{
    public ScheduledCallsController(IServicesManager servicesManager) : base(servicesManager)
    {
    }

    [HasPermission(Permission.ReadScheduledCalls)]
    [HttpGet("phone-operator/{phoneOperatorId:int}")]
    public async Task<IActionResult> GetAllForPhoneOperator(
        int phoneOperatorId,
        [Required, Range(0, int.MaxValue)] int paginationCursor,
        [Required, Range(1, int.MaxValue)] int pageSize,
        DateOnly? scheduledFor = null,
        bool? filterByCompletionStatus = null)
    {
        var result = await _servicesManager.ScheduledCallsService
                                           .GetAllForPhoneOperatorAsync(
                                                phoneOperatorId,
                                                paginationCursor,
                                                pageSize,
                                                scheduledFor,
                                                filterByCompletionStatus);

        if (result.IsFailed)
            return BadRequest(result.GetErrorsDictionary());

        foreach (var call in result.Value.Values)
            await PopulateCall(call);

        return Ok(result.Value);
    }

    [HasPermission(Permission.CreateScheduledCalls)]
    [HttpPost]
    public async Task<IActionResult> Create(ScheduledCall_AddRequestModel call)
    {
        var result = await _servicesManager.ScheduledCallsService
                                           .CreateAsync(call);

        if (result.IsFailed)
            return BadRequest(result.GetErrorsDictionary());

        await PopulateCall(result.Value);
        return Created(string.Empty, result.Value);
    }

    [HasPermission(Permission.EditScheduledCalls)]
    [HttpPatch("{id:int}/markComplete")]
    public async Task<IActionResult> MarkComplete(int id, CallOutcome outcome)
    {
        var result = await _servicesManager.WrapInTransactionAsync<CustomerCall>(async () =>
        {
            var patchResult = await _servicesManager.ScheduledCallsService.MarkCompleteAsync(id);

            if (patchResult.IsFailed)
                return Result.Fail(patchResult.Errors);

            var createResult = await _servicesManager.CustomerCallsService
                                                     .CreateAsync(new CustomerCall_AddRequestModel
                                                     {
                                                         CustomerId = patchResult.Value.Customer.Id,
                                                         OccuredAt = patchResult.Value.CompletedAt.Value,
                                                         Outcome = outcome,
                                                         PhoneAgentId = patchResult.Value.PhoneAgent.Id
                                                     });

            if (createResult.IsFailed)
                return Result.Fail(createResult.Errors);

            var phoneAgent = (await _servicesManager.UsersService
                                                    .GetByIdAsync(createResult.Value.PhoneAgent.Id)).Value;

            createResult.Value.PhoneAgent.Surname = phoneAgent.Surname;
            createResult.Value.PhoneAgent.Username = phoneAgent.Username;
            createResult.Value.PhoneAgent.Name = phoneAgent.Name;
            
            return createResult.Value;
        });

        return result.IsFailed 
               ? BadRequest(result.GetErrorsDictionary()) 
               : Ok(result.Value);
    }

    [HasPermission(Permission.DeleteScheduledCalls)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _servicesManager.ScheduledCallsService
                                           .RemoveAsync(id);

        return result.IsFailed 
               ? BadRequest(result.GetErrorsDictionary()) 
               : NoContent();
    }

    private async Task PopulateCall(ScheduledCall call)
    {
        var phoneAgent = (await _servicesManager.UsersService
                                                .GetByIdAsync(call.PhoneAgent.Id))
                                                .Value;

        call.PhoneAgent.Username = phoneAgent.Username;
        call.PhoneAgent.Name = phoneAgent.Name;
        call.PhoneAgent.Surname = phoneAgent.Surname;

        var customer = (await _servicesManager.CustomersService
                                              .GetByIdAsync(call.Customer.Id))
                                              .Value;

        call.Customer.Address = customer.Address;
        call.Customer.FullName = customer.FullName;
    }
}
