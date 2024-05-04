using Microsoft.AspNetCore.Mvc;
using WaterFilterBusiness.BLL;

namespace WaterFilterBusiness.API.Controllers.Inventory
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryPurchasesController : Controller
    {
        public InventoryPurchasesController(IServicesManager servicesManager) : base(servicesManager)
        {
        }

        [HttpGet()]
        public async Task<IActionResult> GetAll(int page, int pageSize)
        {
            var purchases = await _servicesManager.InventoryPurchasesService
                                                  .GetAllAsync(page, pageSize);

            foreach (var purchase in purchases.Values)
                purchase.Tool = (await _servicesManager.InventoryItemsService
                                                 .GetByIdAsync(purchase.Tool.Id))
                                                 .Value;

            return Ok(purchases);
        }
    }
}
