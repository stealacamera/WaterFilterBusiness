using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.ViewModels;

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

            return GetResult(results, Ok(results.Value));
        }

        [HttpPatch("{commissionId:int}/markComplete")]
        public async Task<IActionResult> UpdateValue(int commissionId)
        {
            var updateResult = await _servicesManager.CommissionRequestsService
                                                     .UpdateAsync(commissionId);
            
            return GetResult(updateResult, Ok(updateResult.Value));
        }
    }
}