using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.Utilities;

namespace WaterFilterBusiness.API.Controllers.Finance
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommissionsController : Controller
    {
        public CommissionsController(IServicesManager servicesManager) : base(servicesManager)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [Required, Range(1, int.MaxValue)] int pageSize,
            [Required, Range(1, int.MaxValue)] int page = 1)
        {
            var result = await _servicesManager.CommissionsService
                                               .GetAllAsync(page, pageSize);

            foreach (var commission in result.Values)
                await PopulateModel(commission);

            return Ok(result);
        }

        [HttpGet("worker/{workerId:int}")]
        public async Task<IActionResult> GetSpecficWorker(
            int workerId,
            [Required, Range(1, int.MaxValue)] int pageSize,
            [Required, Range(1, int.MaxValue)] int page)
        {
            var result = await _servicesManager.CommissionsService
                                               .GetAllForWorkerAsync(page, pageSize, workerId);

            if (result.IsFailed)
                return BadRequest(result.GetErrorsDictionary());

            foreach (var commission in result.Value.Values)
                await PopulateModel(commission);

            return Ok(result.Value);
        }

        [HttpPatch("{id:int}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var updateResult = await _servicesManager.CommissionsService.ApproveAsync(id);

            if (updateResult.IsFailed)
                return BadRequest(updateResult.GetErrorsDictionary());

            await PopulateModel(updateResult.Value);
            return Ok(updateResult.Value);
        }

        [HttpPatch("{id:int}/releaseEarly")]
        public async Task<IActionResult> ReleaseEarly(int id)
        {
            var result = await _servicesManager.WrapInTransactionAsync(async () =>
            {
                var updateResult = await _servicesManager.CommissionsService.ReleaseAsync(id);

                if (updateResult.IsFailed)
                    return updateResult;

                await _servicesManager.CommissionRequestsService.UpdateAsync(id);
                return updateResult.Value;
            });

            if (result.IsFailed)
                return BadRequest(result.GetErrorsDictionary());

            await PopulateModel(result.Value);
            return Ok(result.Value);
        }

        [HttpPost("{id:int}/requestRelease")]
        public async Task<IActionResult> RequestEarlyRelease(int id)
        {
            var result = await _servicesManager.CommissionRequestsService
                                               .CreateAsync(id);

            if (result.IsFailed)
                return BadRequest(result.GetErrorsDictionary());

            await PopulateModel(result.Value);
            return Created(nameof(RequestEarlyRelease), result);
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
                Id = commission.Id,
                Amount = commission.Amount,
                CommissionType = commission.CommissionType,
                Worker = new User_BriefDescription
                {
                    Id = worker.Id,
                    Name = worker.Name,
                    Surname = worker.Surname,
                    Username = worker.Username
                }
            };
        }

        private async Task PopulateModel(Commission model)
        {
            var worker = (await _servicesManager.UsersService
                                                .GetByIdAsync(model.Worker.Id))
                                                .Value;

            model.Worker = new User_BriefDescription
            {
                Id = worker.Id,
                Name = worker.Name,
                Surname = worker.Surname,
                Username = worker.Username
            };

        }
    }
}