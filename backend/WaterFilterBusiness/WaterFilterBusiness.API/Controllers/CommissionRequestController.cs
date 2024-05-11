using FluentResults;
using Microsoft.AspNetCore.Mvc;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.ViewModels;

namespace WaterFilterBusiness.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommissionRequestController : Controller
    {
        public CommissionRequestController(IServicesManager servicesManager) : base(servicesManager)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRequests(int pageSize, int page = 1)
        {
            OffsetPaginatedList<CommissionRequest> results
            results = await _servicesManager.CommissionRequestService.GetAllAsync(page, pageSize);
            return Ok(results);
        }


        [HttpPatch("{id:int}")]
        public async Task<IActionResult> UpdateValue(int id)
        {
            var UpdatedReq = await _servicesManager.CommissionRequestService.GetByID(id);
            await _servicesManager.CommissionRequestService.Update(id);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEarlyRequests(int pageSize, int page = 1)
        {
            OffsetPaginatedList<CommissionRequest> results
            results = await _servicesManager.CommissionRequestService.GetAllEarkyRequestsAsyncc(page = 1, pageSize);
            return Ok(results);
        }


    }
}