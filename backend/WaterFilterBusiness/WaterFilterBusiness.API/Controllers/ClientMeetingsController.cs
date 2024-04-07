using Microsoft.AspNetCore.Mvc;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.DTOs;

namespace WaterFilterBusiness.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientMeetingsController : Controller
    {
        public ClientMeetingsController(IServicesManager servicesManager) : base(servicesManager)
        {
        }

        //[authorized only for MARK MG]
        [HttpGet]
        public async Task<IActionResult> GetAll(
            int paginationCursor,
            int pageSize,
            DateOnly? from = null,
            DateOnly? to = null,
            bool onlyCompleted = false, bool onlyUpcoming = false,
            bool onlyExpressMeetings = false)
        {
            var meetings = await _servicesManager.ClientMeetings
                                                 .GetAllAsync(
                                                    paginationCursor, pageSize, 
                                                    from, to, 
                                                    onlyCompleted, onlyUpcoming, onlyExpressMeetings);

            //foreach (var meeting in meetings.Values)
            //    await CompleteMeetingInformation(meeting);

            return Ok(meetings);
        }

        //[authorized only for PHA and SA]
        [HttpGet("worker/{userId:int}/date/{date}")]
        public async Task<IActionResult> GetAllByDayForWorker(
            int userId,
            DateOnly date,
            int paginationCursor,
            int pageSize,
            bool onlyExpressMeetings = false)
        {
            var meetings = await _servicesManager.ClientMeetings
                                                .GetAllByDayForWorkerAsync(
                                                    userId,
                                                    date,
                                                    paginationCursor, pageSize,
                                                    onlyExpressMeetings);

            foreach (var meeting in meetings.Values)
                await CompleteMeetingInformation(meeting);

            return Ok(meetings);
        }

        //[authorized only for PHA and SA]
        [HttpGet("worker/{userId:int}/week/{date}")]
        public async Task<IActionResult> GetAllByWeekForWorker(int userId, DateOnly date, int paginationCursor, int pageSize, bool onlyExpressMeetings = false)
        {
            var meetings = await _servicesManager.ClientMeetings
                                                .GetAllByWeekForWorkerAsync(
                                                    userId,
                                                    date,
                                                    paginationCursor, pageSize,
                                                    onlyExpressMeetings);

            foreach (var meeting in meetings.Values)
                await CompleteMeetingInformation(meeting);

            return Ok(meetings);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ClientMeeting_AddRequestModel meeting)
        {
            var result = await _servicesManager.ClientMeetings.CreateAsync(meeting);
            return result.IsFailed ? BadRequest(result.Errors.First()) : Ok(result.Value);
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Update(CLientMeeting_UpdateRequestModel meeting)
        {
            var result = await _servicesManager.ClientMeetings.UpdateAsync(meeting);
            return result.IsFailed ? BadRequest(result.Errors.First()) : Ok(result.Value);
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
}
