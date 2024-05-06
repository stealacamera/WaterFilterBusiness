using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.Attributes;
using WaterFilterBusiness.Common.Enums;

namespace WaterFilterBusiness.API.Controllers.Inventory;

[Route("api/[controller]")]
[ApiController]
public class InventoryMovementsController : Controller
{
    public InventoryMovementsController(IServicesManager servicesManager) : base(servicesManager)
    {
    }

    [HasPermission(Permission.ReadInventoryMovements)]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [Required, Range(0, int.MaxValue)] int cursor,
        [Required, Range(1, int.MaxValue)] int pageSize)
    {
        var result = await _servicesManager.InventoryMovementsService
                                           .GetAllAsync(cursor, pageSize);

        return Ok(result);
    }
}
