using Microsoft.AspNetCore.Mvc;
using WaterFilterBusiness.BLL;

namespace WaterFilterBusiness.API.Controllers.Inventory.Inventories;

[Route("api/[controller]")]
[ApiController]
public class SmallInventoryItemsController : Controller
{
    public SmallInventoryItemsController(IServicesManager servicesManager) : base(servicesManager)
    {
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int page, int pageSize, bool? orderByQuantity = null)
    {
        var items = await _servicesManager.SmallInventoryItemsService
                                          .GetAllAsync(page, pageSize, orderByQuantity);

        foreach (var item in items.Values)
            item.Item = (await _servicesManager.InventoryItemsService.GetByIdAsync(item.Item.Id)).Value;

        return Ok(items);
    }
}
