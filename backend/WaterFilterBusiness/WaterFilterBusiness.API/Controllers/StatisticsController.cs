using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.Utilities;

namespace WaterFilterBusiness.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StatisticsController : Controller
{
    public StatisticsController(IServicesManager servicesManager) : base(servicesManager)
    {
    }

    // TODO inv type json converterwe


    // TODO schedule removing customers from redlisted

    // TODO commission requests
    // or has kristi done it?

    //TODO SA = Meeting completing, success rates, cancellations(+ nice-to-have: nr slots in day filled by PHA?)
    //    + filters(esp to find a specific meeting)

    //Sales + debts = Filters for a given day, week, month


    [HttpGet("phoneAgent/{id:int}/calls/yearlyTotalMonthly")]
    public async Task<IActionResult> GetPhoneAgentTotalMonthlyCalls(int id, DateOnly? from, DateOnly? to)
    {
        var result = await _servicesManager.CustomerCallsService
                                           .GetYearlyTotalMonthlyNrCallsForPhoneAgentAsync(id, from, to);
        
        return result.IsFailed ? BadRequest(result.GetErrorsDictionary()) : Ok(result.Value);
    }

    [HttpGet("phoneAgent/{id:int}/calls/latestWeeklyTotals")]
    public async Task<IActionResult> GetPhoneAgentLatestTotalWeeklyCalls(
        int id, 
        [Required, Range(1, int.MaxValue)] int nrLatestWeeks)
    {
        var result = await _servicesManager.CustomerCallsService
                                           .GetLastXWeeksNrCallsForPhoneAgentAsync(id, nrLatestWeeks);

        return result.IsFailed ? BadRequest(result.GetErrorsDictionary()) : Ok(result.Value);
    }

    [HttpGet("phoneAgent/{id:int}/meetings/yearlyTotalMonthly")]
    public async Task<IActionResult> GetPhoneAgentTotalMonthlyMeetingsSetup(int id, DateOnly? from, DateOnly? to)
    {
        var result = await _servicesManager.ClientMeetings
                                           .GetYearlyTotalMonthlyMeetingsSetupForPhoneAgentAsync(id, from, to);

        return result.IsFailed ? BadRequest(result.GetErrorsDictionary()) : Ok(result.Value);
    }

    [HttpGet("phoneAgent/{id:int}/meetings/latestWeeklyTotals")]
    public async Task<IActionResult> GetPhoneAgentLatestTotalWeeklyMeetingsSetup(
        int id, 
        [Required, Range(1, int.MaxValue)] int nrLatestWeeks)
    {
        var result = await _servicesManager.ClientMeetings
                                           .GetLastestXWeeklyMeetingsSetupForPhoneAgentAsync(id, nrLatestWeeks);

        return result.IsFailed ? BadRequest(result.GetErrorsDictionary()) : Ok(result.Value);
    }
}
