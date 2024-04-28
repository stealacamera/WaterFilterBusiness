using Microsoft.AspNetCore.Mvc;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.DTOs;

namespace WaterFilterBusiness.API.Controllers.Inventory;

public class InventoryMovementsController : Controller
{
    public InventoryMovementsController(IServicesManager servicesManager) : base(servicesManager)
    {
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int cursor, int pageSize)
    {
        var result = await _servicesManager.InventoryMovementsService
                                           .GetAllAsync(cursor, pageSize);

        return Ok(result);
    }
}
