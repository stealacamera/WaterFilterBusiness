using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.Attributes;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.Utilities;

namespace WaterFilterBusiness.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientMeetingsController : Controller
{
    public ClientMeetingsController(IServicesManager servicesManager) : base(servicesManager)
    {
    }

    [HasPermission(Permission.ReadClientMeetings)]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [Required, Range(1, int.MaxValue)] int page,
        [Required, Range(1, int.MaxValue)] int pageSize,
        DateOnly? from = null,
        DateOnly? to = null,
        int? filterByOutcome = null,
        bool? filterExpressMeetings = null)
    {
        var meetings = await _servicesManager.ClientMeetingsService
                                             .GetAllAsync(
                                                page, pageSize,
                                                from, to,
                                                filterByOutcome,
                                                filterExpressMeetings);

        foreach (var meeting in meetings.Values)
            await CompleteMeetingInformation(meeting);

        return Ok(meetings);
    }

    [HasPermission(Permission.ReadClientMeetingsForWorker)]
    [HttpGet("worker/{userId:int}/date/{date}")]
    public async Task<IActionResult> GetAllByDayForWorker(
        int userId,
        DateOnly date,
        [Required, Range(0, int.MaxValue)] int paginationCursor,
        [Required, Range(1, int.MaxValue)] int pageSize,
        bool? filterByCompleted = null,
        bool? filterExpressMeetings = null)
    {
        var meetings = await _servicesManager.ClientMeetingsService
                                            .GetAllByDayForWorkerAsync(
                                                userId,
                                                date,
                                                paginationCursor, pageSize,
                                                filterByCompleted,
                                                filterExpressMeetings);

        if (meetings.IsFailed)
            return BadRequest(meetings.GetErrorsDictionary());

        foreach (var meeting in meetings.Value.Values)
            await CompleteMeetingInformation(meeting);

        return Ok(meetings.Value);
    }

    [HasPermission(Permission.ReadClientMeetingsForWorker)]
    [HttpGet("worker/{userId:int}/week/{date}")]
    public async Task<IActionResult> GetAllByWeekForWorker(
        int userId,
        DateOnly date,
        [Required, Range(0, int.MaxValue)] int paginationCursor,
        [Required, Range(1, int.MaxValue)] int pageSize,
        bool? filterByCompleted = null,
        bool? filterExpressMeetings = null)
    {
        var meetings = await _servicesManager.ClientMeetingsService
                                            .GetAllByWeekForWorkerAsync(
                                                userId,
                                                date,
                                                paginationCursor, pageSize,
                                                filterByCompleted,
                                                filterExpressMeetings);

        if (meetings.IsFailed)
            return BadRequest(meetings.GetErrorsDictionary());

        foreach (var meeting in meetings.Value.Values)
            await CompleteMeetingInformation(meeting);

        return Ok(meetings.Value);
    }

    [HasPermission(Permission.CreateClientMeetings)]
    [HttpPost]
    public async Task<IActionResult> Create(ClientMeeting_AddRequestModel meeting)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicesManager.ClientMeetingsService.CreateAsync(meeting);

        if (result.IsSuccess)
        {
            await CompleteMeetingInformation(result.Value);
            return Ok(result.Value);
        }
        else
            return BadRequest(result.GetErrorsDictionary());
    }

    [HasPermission(Permission.ConcludeClientMeetings)]
    [HttpPatch("{id:int}/conclude")]
    public async Task<IActionResult> Update(int id, ClientMeeting_UpdateRequestModel meeting)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicesManager.ClientMeetingsService.UpdateAsync(id, meeting);

        if (result.IsSuccess)
        {
            await CompleteMeetingInformation(result.Value);
            return Ok(result.Value);
        }
        else
            return BadRequest(result.GetErrorsDictionary());
    }

    private async Task CompleteMeetingInformation(ClientMeeting meeting)
    {
        var customer = (await _servicesManager.CustomersService.GetByIdAsync(meeting.Customer.Id)).Value;
        meeting.Customer.Address = customer.Address;
        meeting.Customer.FullName = customer.FullName;

        var salesAgent = (await _servicesManager.UsersService.GetByIdAsync(meeting.SalesAgent.Id)).Value;
        meeting.SalesAgent.Surname = salesAgent.Surname;
        meeting.SalesAgent.Name = salesAgent.Name;
        meeting.SalesAgent.Username = salesAgent.Username;

        if (meeting.PhoneOperator != null)
        {
            var phoneOperator = (await _servicesManager.UsersService.GetByIdAsync(meeting.PhoneOperator.Id)).Value;

            meeting.PhoneOperator.Surname = phoneOperator.Surname;
            meeting.PhoneOperator.Name = phoneOperator.Name;
            meeting.PhoneOperator.Username = phoneOperator.Username;
        }
    }
}
