using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.Utilities;

namespace WaterFilterBusiness.API.Controllers.Inventory.Inventories;

[Route("api/[controller]")]
[ApiController]
public class TechnicianInventoryItemsController : Controller
{
    public TechnicianInventoryItemsController(IServicesManager servicesManager) : base(servicesManager)
    {
    }

    [HttpGet("technician/{technicianId:int}")]
    public async Task<IActionResult> GetAll(int technicianId, int page, int pageSize)
    {
        var result = await _servicesManager.TechnicianInventoryItemsService
                                          .GetAllAsync(technicianId, page, pageSize);

        if (result.IsFailed)
            return BadRequest(result.GetErrorsDictionary());

        foreach (var item in result.Value.Values)
        {
            var baseItem = await _servicesManager.InventoryItemsService.GetByIdAsync(item.Item.Id);
            item.Item = baseItem.Value;
        }

        return Ok(result.Value);
    }

    [HttpPatch("technician/{technicianId:int}/tool/{toolId:int}/reduce-stock")]
    public async Task<IActionResult> DecreaseQuantity(
        int technicianId, 
        int toolId, 
        [FromBody, Range(1, int.MaxValue)] int decreaseBy)
    {
        var result = await _servicesManager.TechnicianInventoryItemsService
                                           .DecreaseQuantityAsync(technicianId, toolId, decreaseBy);

        return result.IsFailed 
               ? BadRequest(result.GetErrorsDictionary()) 
               : Ok(result.Value);
    }
}
