using Microsoft.AspNetCore.Mvc;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.Utilities;

namespace WaterFilterBusiness.API.Controllers.Inventory
{
    public class InventoryItemsController : Controller
    {
        public InventoryItemsController(IServicesManager servicesManager) : base(servicesManager)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int page, int pageSize)
        {
            var items = await _servicesManager.InventoryItemsService
                                              .GetAllAsync(page, pageSize);
            
            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> Create(InventoryItem_AddRequestModel item)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var model = await _servicesManager.InventoryItemsService.CreateAsync(item);
            return Created(string.Empty, model);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _servicesManager.InventoryItemsService.RemoveAsync(id);
            return result.IsSuccess ? NoContent() : BadRequest(result.GetErrorsDictionary());
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Update(int id, InventoryItem_PatchRequestModel updatedItem)
        {
            var result = await _servicesManager.InventoryItemsService.UpdateAsync(id, updatedItem);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.GetErrorsDictionary());
        }
    }
}
