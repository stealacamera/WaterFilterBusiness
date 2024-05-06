using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.Attributes;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.Utilities;

namespace WaterFilterBusiness.API.Controllers.Finance
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientDebtsController : Controller
    {
        public ClientDebtsController(IServicesManager servicesManager) : base(servicesManager)
        {
        }

        [HasPermission(Permission.ReadClientDebts)]
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [Required, Range(0, int.MaxValue)] int paginationCursor, 
            [Required, Range(1, int.MaxValue)] int pageSize, 
            int? filterByClient = null,
            bool? filterByCompletionStatus = null)
        {
            var debts = await _servicesManager.ClientDebtsService
                                              .GetAllAsync(
                                                paginationCursor, pageSize, 
                                                filterByClient, filterByCompletionStatus);

            return Ok(debts);
        }

        [HasPermission(Permission.EditClientDebts)]
        [HttpPatch("{id:int}/complete")]
        public async Task<IActionResult> MarkComplete(int id)
        {
            var updateResult = await _servicesManager.ClientDebtsService.CompletePaymentAsync(id);
            return updateResult.IsSuccess 
                   ? Ok(updateResult.Value) 
                   : BadRequest(updateResult.GetErrorsDictionary());
        }
    }
}
