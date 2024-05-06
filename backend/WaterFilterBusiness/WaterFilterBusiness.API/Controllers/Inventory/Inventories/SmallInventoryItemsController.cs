using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.Attributes;
using WaterFilterBusiness.Common.Enums;

namespace WaterFilterBusiness.API.Controllers.Inventory.Inventories;

[Route("api/[controller]")]
[ApiController]
public class SmallInventoryItemsController : Controller
{
    public SmallInventoryItemsController(IServicesManager servicesManager) : base(servicesManager)
    {
    }

    [HasPermission(Permission.ReadSmallInventory)]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [Required, Range(1, int.MaxValue)] int page,
        [Required, Range(1, int.MaxValue)] int pageSize,
        bool? orderByQuantity = null)
    {
        var items = await _servicesManager.SmallInventoryItemsService
                                          .GetAllAsync(page, pageSize, orderByQuantity);

        foreach (var item in items.Values)
            item.Item = (await _servicesManager.InventoryItemsService.GetByIdAsync(item.Item.Id)).Value;

        return Ok(items);
    }
}
