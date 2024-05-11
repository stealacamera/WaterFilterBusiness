using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.Attributes;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.Utilities;

namespace WaterFilterBusiness.API.Controllers;

[HasPermission(Permission.ReadStatistics)]
[Route("api/[controller]")]
[ApiController]
public class StatisticsController : Controller
{
    public StatisticsController(IServicesManager servicesManager) : base(servicesManager)
    {
    }

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
        var result = await _servicesManager.ClientMeetingsService
                                           .GetYearlyTotalMonthlyMeetingsSetupForPhoneAgentAsync(id, from, to);

        return result.IsFailed ? BadRequest(result.GetErrorsDictionary()) : Ok(result.Value);
    }

    [HttpGet("phoneAgent/{id:int}/meetings/latestWeeklyTotals")]
    public async Task<IActionResult> GetPhoneAgentLatestTotalWeeklyMeetingsSetup(
        int id, 
        [Required, Range(1, int.MaxValue)] int nrLatestWeeks)
    {
        var result = await _servicesManager.ClientMeetingsService
                                           .GetLastestXWeeklyMeetingsSetupForPhoneAgentAsync(id, nrLatestWeeks);

        return result.IsFailed ? BadRequest(result.GetErrorsDictionary()) : Ok(result.Value);
    }

    [HttpGet("salesAgent/{id:int}/meetings")]
    public async Task<IActionResult> GetTotalNrMeetingsBySalesAgent(int id, MeetingOutcome? filterByOutcome = null)
    {
        var result = await _servicesManager.ClientMeetingsService
                                           .GetTotalNrMeetingsBySalesAgentAsync(id, filterByOutcome);

        return result.IsFailed
               ? BadRequest(result.GetErrorsDictionary())
               : Ok(result.Value);
    }

    [HttpGet("salesAgent/{id:int}/sales")]
    public async Task<IActionResult> GetTotalNrSalesBySalesAgent(int id, DateOnly? filterByDate = null)
    {
        var result = await _servicesManager.SalesService.GetTotalSalesCreatedBySalesAgentAsync(id, filterByDate);

        return result.IsFailed
               ? BadRequest(result.GetErrorsDictionary())
               : Ok(result.Value);
    }
}
