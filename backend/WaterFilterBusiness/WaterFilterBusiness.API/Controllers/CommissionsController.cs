using FluentResults;
using Microsoft.AspNetCore.Mvc;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.ViewModels;

namespace WaterFilterBusiness.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommissionsController: Controller
    {
        public CommissionsController(IServicesManager servicesManager) : base(servicesManager)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int pageSize,int page = 1)
        {
            OffsetPaginatedList<Commission> results
            results = await _servicesManager.CommissionService.GetAllAsync(page, pageSize);
            return Ok(results);
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetSpecficWorker(int pageSize, int page = 1, int id)
        {
            var result = await _servicesManager.CommissionService.GetAllFromOneWorkerAsync(page, pageSize,id);
            return result.IsFailed ? BadRequest(result) : Ok(result.Value);
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> UpdateValue(int id)
        {
            var UpdatedCom = await _servicesManager.CommissionService.GetByID(id);
            await _servicesManager.CommissionService.Update(id);
        }


    }
}