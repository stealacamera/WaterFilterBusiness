using Microsoft.AspNetCore.Mvc;
using WaterFilterBusiness.BLL;

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
            return BadRequest(result.Errors);

        foreach (var item in result.Value.Values)
            item.Item = (await _servicesManager.InventoryItemsService.GetByIdAsync(item.Item.Id)).Value;

        return Ok(result.Value.Values);
    }

    [HttpPatch("technician/{technicianId:int}/tool/{toolId:int}/reduce-stock")]
    public async Task<IActionResult> DecreaseQuantity(int technicianId, int toolId, [FromBody] int decreaseBy)
    {
        var result = await _servicesManager.TechnicianInventoryItemsService
                                           .DecreaseQuantityAsync(technicianId, toolId, decreaseBy);

        return result.IsFailed ? BadRequest(result.Errors) : Ok(result.Value);
    }
}
