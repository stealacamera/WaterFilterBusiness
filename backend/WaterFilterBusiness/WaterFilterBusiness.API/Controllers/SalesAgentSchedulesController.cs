using FluentResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.Attributes;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.ErrorHandling.Exceptions;
using WaterFilterBusiness.Common.Utilities;

namespace WaterFilterBusiness.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SalesAgentSchedulesController : Controller
{
    public SalesAgentSchedulesController(IServicesManager servicesManager) : base(servicesManager)
    {
    }

    [HasPermission(Permission.ReadSalesAgentSchedules)]
    [HttpGet("salesAgents/{salesAgentId:int}")]
    public async Task<IActionResult> GetAllForAgent(int salesAgentId)
    {
        var result = await _servicesManager.SalesAgentSchedulesService
                                           .GetAllForSalesAgentAsync(salesAgentId);

        return result.IsFailed 
               ? BadRequest(result.GetErrorsDictionary()) 
               : Ok(result.Value);
    }

    [HasPermission(Permission.ReadSalesAgentSchedules)]
    [HttpGet]
    public async Task<IActionResult> QueryByTime(TimeOnly? time, Weekday? weekday = null)
    {
        if (time == null && weekday == null)
            return BadRequest("At least one filter should be applied");

        var result = await _servicesManager.SalesAgentSchedulesService
                                           .GetAllByTimeAsync(weekday, time);

        return result.IsFailed 
               ? BadRequest(result.GetErrorsDictionary()) 
               : Ok(result.Value);
    }

    [HasPermission(Permission.CreateSalesAgentSchedules)]
    [HttpPost("salesAgents/{salesAgentId:int}")]
    public async Task<IActionResult> Create(int salesAgentId, SalesAgentSchedule_AddRequestModel schedule)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicesManager.SalesAgentSchedulesService.CreateAsync(salesAgentId, schedule);
        return result.IsFailed 
               ? BadRequest(result.GetErrorsDictionary()) 
               : Ok(result.Value);
    }

    [HasPermission(Permission.DeleteSalesAgentSchedules)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _servicesManager.WrapInTransactionAsync<SalesAgentSchedule>(async () =>
        {
            var result = await _servicesManager.SalesAgentScheduleChangesService.RemoveAllForScheduleAsync(id);

            if (result.IsFailed)
                return result;

            return await _servicesManager.SalesAgentSchedulesService.RemoveAsync(id);
        });

        return result.IsFailed 
               ? BadRequest(result.GetErrorsDictionary()) 
               : Ok(result.Value);
    }

    [HasPermission(Permission.UpdateSalesAgentSchedules)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, SalesAgentSchedule_UpdateRequestModel newSchedule)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicesManager.WrapInTransactionAsync<SalesAgentScheduleUpdate>(async () =>
        {
            var currentScheduleResult = await _servicesManager.SalesAgentSchedulesService
                                                        .GetByIdAsync(id);

            if (currentScheduleResult.IsFailed)
                return Result.Fail(currentScheduleResult.Errors);

            var updateResult = await _servicesManager.SalesAgentSchedulesService
                                                     .UpdateAsync(id, newSchedule);

            if (updateResult.IsFailed)
                return Result.Fail(updateResult.Errors);

            var changeResult = await _servicesManager.SalesAgentScheduleChangesService
                                                     .CreateAsync(id, currentScheduleResult.Value, updateResult.Value);

            return changeResult.IsFailed ?
                   Result.Fail(changeResult.Errors) :
                    new SalesAgentScheduleUpdate { UpdatedSchedule = updateResult.Value, ScheduleChange = changeResult.Value };
        });

        return result.IsFailed 
               ? BadRequest(result.GetErrorsDictionary()) 
               : Ok(result.Value);
    }

    [HttpGet("salesAgents/{salesAgentId:int}/changes")]
    public async Task<IActionResult> GetAllChanges(
        int salesAgentId, 
        [Required, Range(1, int.MaxValue)] int pageSize,
        [Required, Range(0, int.MaxValue)] int paginationCursor)
    {
        var result = await _servicesManager.SalesAgentScheduleChangesService
                                           .GetAllForSalesAgentAsync(salesAgentId, paginationCursor, pageSize);

        return result.IsFailed 
               ? BadRequest(result.GetErrorsDictionary()) 
               : Ok(result.Value);
    }
}
