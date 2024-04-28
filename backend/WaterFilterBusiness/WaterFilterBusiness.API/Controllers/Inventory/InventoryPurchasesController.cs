using Microsoft.AspNetCore.Mvc;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.DTOs;

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
                await CompleteModelForeignEntities(purchase);

            return Ok(purchases);
        }

        [HttpPost()]
        public async Task<IActionResult> Create(InventoryPurchase_AddRequestModel purchase)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createResult = await _servicesManager.InventoryPurchasesService
                                                     .CreateAsync(purchase.ToolId, purchase.Quantity);

            if (createResult.IsFailed)
                return BadRequest(createResult.Errors);
            else
            {
                await CompleteModelForeignEntities(createResult.Value);
                return Created(string.Empty, createResult.Value);
            }
        }

        private async Task CompleteModelForeignEntities(InventoryPurchase purchase)
        {
            var tool = await _servicesManager.InventoryItemsService
                                             .GetByIdAsync(purchase.Tool.Id);

            purchase.Tool = tool.Value;
        }
    }
}
