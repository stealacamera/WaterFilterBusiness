using Microsoft.AspNetCore.Mvc;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.DTOs.Calls;

namespace WaterFilterBusiness.API.Controllers.Calls;

[Route("api/[controller]")]
[ApiController]
public class ScheduledCallsController : Controller
{
    public ScheduledCallsController(IServicesManager servicesManager) : base(servicesManager)
    {
    }

    [HttpGet("phone-operator/{phoneOperatorId:int}")]
    public async Task<IActionResult> GetAllForPhoneOperator(
        int phoneOperatorId,
        int pageSize,
        int paginationCursor = 0,
        DateOnly? scheduledFor = null)
    {
        var result = await _servicesManager.ScheduledCallsService
                                           .GetAllForPhoneOperator(
                                                phoneOperatorId,
                                                paginationCursor,
                                                pageSize,
                                                scheduledFor);

        return result.IsFailed ? BadRequest(result) : Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ScheduledCall_AddRequestModel call)
    {
        var result = await _servicesManager.ScheduledCallsService
                                           .CreateAsync(call);

        return result.IsFailed ? BadRequest(result) : Created(string.Empty, result.Value);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _servicesManager.ScheduledCallsService
                                           .RemoveAsync(id);

        return result.IsFailed ? BadRequest(result) : NoContent();
    }
}
