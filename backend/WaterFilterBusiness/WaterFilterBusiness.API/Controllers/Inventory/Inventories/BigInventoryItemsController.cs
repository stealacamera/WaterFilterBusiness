using FluentResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.Utilities;

namespace WaterFilterBusiness.API.Controllers.Inventory.Inventories
{
    [Route("api/[controller]")]
    [ApiController]
    public class BigInventoryItemsController : Controller
    {
        public BigInventoryItemsController(IServicesManager servicesManager) : base(servicesManager)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int page, int pageSize, bool? orderByQuantity = null)
        {
            var items = await _servicesManager.BigInventoryItemsService
                                              .GetAllAsync(page, pageSize, orderByQuantity);

            foreach (var item in items.Values)
            {
                var baseItem = await _servicesManager.InventoryItemsService.GetByIdAsync(item.Item.Id);
                item.Item = baseItem.Value;
            }

            return Ok(items);
        }
                
        [HttpPatch("tool/{toolId:int}/stock-up")]
        public async Task<IActionResult> IncreaseQuantity(
            int toolId, 
            [FromBody, Range(1, int.MaxValue)] int increaseBy)
        {
            var result = await _servicesManager.WrapInTransactionAsync(async () =>
            {
                var updateResult = await _servicesManager.BigInventoryItemsService
                                                         .UpsertAsync(toolId, increaseBy);

                if (updateResult.IsFailed)
                    return updateResult;

                var purchaseResult = await _servicesManager.InventoryPurchasesService
                                                           .CreateAsync(toolId, increaseBy);

                if (purchaseResult.IsFailed)
                    return Result.Fail(purchaseResult.Errors);

                return updateResult.Value;
            });

            return result.IsSuccess 
                   ? Ok(result.Value) 
                   : BadRequest(result.GetErrorsDictionary());
        }

        [HttpPatch("tool/{toolId:int}/reduce-stock")]
        public async Task<IActionResult> DecreaseQuantity(
            int toolId, 
            [FromBody, Range(1, int.MaxValue)] int decreaseBy)
        {
            var result = await _servicesManager.BigInventoryItemsService
                                               .DecreaseQuantityAsync(toolId, decreaseBy);

            return result.IsSuccess 
                   ? Ok(result.Value) 
                   : BadRequest(result.GetErrorsDictionary());
        }
    }
}
