using Microsoft.AspNetCore.Mvc;
using WaterFilterBusiness.BLL;

namespace WaterFilterBusiness.API.Controllers.Finance
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientDebtsController : Controller
    {
        public ClientDebtsController(IServicesManager servicesManager) : base(servicesManager)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            int paginationCursor, 
            int pageSize, 
            int? filterByClient = null,
            bool? filterByCompletionStatus = null)
        {
            var debts = await _servicesManager.ClientDebtsService
                                              .GetAllAsync(
                                                paginationCursor, pageSize, 
                                                filterByClient, filterByCompletionStatus);

            return Ok(debts);
        }

        [HttpPatch("{id:int}/complete")]
        public async Task<IActionResult> MarkComplete(int id)
        {
            var updateResult = await _servicesManager.ClientDebtsService.CompletePaymentAsync(id);
            return updateResult.IsSuccess ? Ok(updateResult.Value) : BadRequest(updateResult.Errors);
        }
    }
}
