using FluentResults;
using Microsoft.AspNetCore.Mvc;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.Enums;

namespace WaterFilterBusiness.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesAgentSchedulesController : Controller
    {
        public SalesAgentSchedulesController(IServicesManager servicesManager) : base(servicesManager)
        {
        }

        [HttpGet("salesAgents/{salesAgentId:int}")]
        public async Task<IActionResult> GetAllForAgent(int salesAgentId)
        {
            var result = await _servicesManager.SalesAgentSchedulesService
                                               .GetAllForSalesAgentAsync(salesAgentId);

            return result.IsFailed ? BadRequest(result) : Ok(result.Value);
        }

        //[HttpGet]
        //public async Task<IActionResult> QueryByTime([System.Web.Http.FromUri] Weekday? weekday = null, TimeOnly? time = null)
        //{
        //    var result = await _servicesManager.SalesAgentSchedulesService
        //                                       .GetAllByTimeAsync(weekday, time);

        //    return result.IsFailed ? BadRequest(result) : Ok(result.Value);
        //}

        [HttpPost("salesAgents/{salesAgentId:int}")]
        public async Task<IActionResult> Create(int salesAgentId, SalesAgentSchedule_AddRequestModel schedule)
        {
            var result = await _servicesManager.SalesAgentSchedulesService.CreateAsync(salesAgentId, schedule);
            return result.IsFailed ? BadRequest(result) : Ok(result.Value);
        }

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

            return result.IsFailed ? BadRequest(result) : Ok(result.Value);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, SalesAgentSchedule_UpdateRequestModel newSchedule)
        {
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

            return result.IsFailed ? BadRequest(result) : Ok(result.Value);
        }

        [HttpGet("salesAgents/{salesAgentId:int}/changes")]
        public async Task<IActionResult> GetAllChanges(int salesAgentId, int pageSize, int paginationCursor = 0)
        {
            var result = await _servicesManager.SalesAgentScheduleChangesService
                                               .GetAllForSalesAgentAsync(salesAgentId, paginationCursor, pageSize);

            return result.IsFailed ? BadRequest(result) : Ok(result.Value);
        }
    }
}
