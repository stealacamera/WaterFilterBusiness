using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WaterFilterBusiness.BLL;

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

            return GetResult(result, Ok(result.Value));
        }

        [HttpPatch("{id:int}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var updateResult = await _servicesManager.CommissionsService.ApproveAsync(id);
            return GetResult(updateResult, Ok(updateResult.Value));
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

            return GetResult(result, Ok(result.Value));
        }


        [HttpPost("{id:int}/requestRelease")]
        public async Task<IActionResult> RequestEarlyRelease(int id)
        {
            var result = await _servicesManager.CommissionRequestsService
                                               .CreateAsync(id);

            return GetResult(result, Created(nameof(RequestEarlyRelease), result));
        }
    }
}