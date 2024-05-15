using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.Utilities;

namespace WaterFilterBusiness.API.Controllers.Finance
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommissionRequestController : Controller
    {
        public CommissionRequestController(IServicesManager servicesManager) : base(servicesManager)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [Required, Range(1, int.MaxValue)]int pageSize,
            [Required, Range(1, int.MaxValue)] int page,
            bool? filterByCompleted = null)
        {
            var results = await _servicesManager.CommissionRequestsService
                                                .GetAllAsync(page, pageSize, filterByCompleted);

            foreach (var request in results.Values)
                await PopulateModel(request);

            return Ok(results);
        }

        [HttpGet("worker/{workerId:int}")]
        public async Task<IActionResult> GetAllForWorker(
            int workerId,
            [Required, Range(1, int.MaxValue)] int pageSize,
            [Required, Range(1, int.MaxValue)] int page,
            bool? filterByCompleted = null)
        {
            var results = await _servicesManager.CommissionRequestsService
                                                .GetAllFromWorkerAsync(page, pageSize, workerId, filterByCompleted);

            if (results.IsFailed)
                return BadRequest(results.GetErrorsDictionary());

            foreach (var request in results.Value.Values)
                await PopulateModel(request);

            return Ok(results.Value);
        }

        [HttpPatch("{commissionId:int}/markComplete")]
        public async Task<IActionResult> UpdateValue(int commissionId)
        {
            var updateResult = await _servicesManager.CommissionRequestsService
                                                     .UpdateAsync(commissionId);

            if (updateResult.IsFailed)
                return BadRequest(updateResult.GetErrorsDictionary());

            await PopulateModel(updateResult.Value);
            return Ok(updateResult.Value);
        }

        private async Task PopulateModel(CommissionRequest model)
        {
            var commission = (await _servicesManager.CommissionsService
                                                        .GetByIdAsync(model.Commission.Id))
                                                        .Value;

            var worker = (await _servicesManager.UsersService
                                                .GetByIdAsync(commission.Worker.Id))
                                                .Value;

            model.Commission = new Commission_BriefDescription
            {
                Amount = commission.Amount,
                CommissionType = commission.CommissionType,
                Id = commission.Id,
                Worker = new User_BriefDescription
                {
                    Id = worker.Id,
                    Name = worker.Name,
                    Surname = worker.Surname,
                    Username = worker.Username
                }
            };
        }
    }
}