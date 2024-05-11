using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.Attributes;
using WaterFilterBusiness.Common.Enums;

namespace WaterFilterBusiness.API.Controllers.Inventory;

[Route("api/[controller]")]
[ApiController]
public class InventoryPurchasesController : Controller
{
    public InventoryPurchasesController(IServicesManager servicesManager) : base(servicesManager)
    {
    }

    [HasPermission(Permission.ReadInventoryPurchases)]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [Required, Range(1, int.MaxValue)] int page, 
        [Required, Range(1, int.MaxValue)] int pageSize)
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
