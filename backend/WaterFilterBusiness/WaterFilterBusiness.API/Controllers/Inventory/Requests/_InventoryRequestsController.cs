using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.DTOs.Inventory;

namespace WaterFilterBusiness.API.Controllers.Inventory.Requests;

[ApiController]
public abstract class BaseInventoryRequestsController : Controller
{
    public BaseInventoryRequestsController(IServicesManager servicesManager) : base(servicesManager)
    {
    }

    [HttpPost]
    public abstract Task<IActionResult> Create(InventoryRequest_AddRequestModel request);

    [HttpGet]
    public abstract Task<IActionResult> GetAll(int page, int pageSize);

    [HttpPatch("{id:int}/complete")]
    public abstract Task<IActionResult> CompleteRequest(int id, [FromBody, MaxLength(210)] string? conclusionNote);

    [HttpPatch("{id:int}/accept")]
    public abstract Task<IActionResult> AcceptRequest(int id);

    [HttpPatch("{id:int}/cancel")]
    public abstract Task<IActionResult> CancelRequest(int id, [FromBody, MaxLength(210)] string? conclusionNote);
}
