using FluentResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.Inventory;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.Utilities;

namespace WaterFilterBusiness.API.Controllers.Inventory.Requests;

[Route("api/[controller]")]
[ApiController]
public class TechnicianInventoryRequestsController : BaseInventoryRequestsController
{
    public TechnicianInventoryRequestsController(IServicesManager servicesManager) : base(servicesManager)
    {
    }

    public override async Task<IActionResult> Create(InventoryRequest_AddRequestModel request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicesManager.WrapInTransactionAsync<InventoryRequest>(async () =>
        {
            var baseRequestCreateresult = await _servicesManager.BaseInventoryRequestsService
                                                                .CreateAsync(request);

            if (baseRequestCreateresult.IsFailed)
                return Result.Fail(baseRequestCreateresult.Errors);

            //TODO get id from authentication
            var createResult = await _servicesManager.TechnicianInventoryRequestsService
                                                     .CreateAsync(3011, baseRequestCreateresult.Value);

            return createResult.IsFailed
                   ? Result.Fail(createResult.Errors)
                   : createResult.Value;
        });

        return result.IsFailed
               ? BadRequest(result.GetErrorsDictionary())
               : Created(string.Empty, result.Value);
    }

    public override async Task<IActionResult> GetAll(int page, int pageSize)
    {
        var requests = await _servicesManager.TechnicianInventoryRequestsService
                                             .GetAllAsync(page, pageSize);

        foreach (var request in requests.Values)
            request.Tool = (await _servicesManager.InventoryItemsService
                                                 .GetByIdAsync(request.Tool.Id))
                                                 .Value;

        return Ok(requests);
    }

    public override async Task<IActionResult> CompleteRequest(int id, [FromBody, MaxLength(210)] string? conclusionNote)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicesManager.WrapInTransactionAsync<TechnicianInventoryRequest>(async () =>
        {
            var updateResult = await _servicesManager.TechnicianInventoryRequestsService
                                  .UpdateAsync(id, new InventoryRequest_PatchRequestModel
                                  {
                                      Status = InventoryRequestStatus.Completed,
                                      ConclusionNote = conclusionNote
                                  });

            if (updateResult.IsFailed)
                return Result.Fail(updateResult.Errors);

            var request = updateResult.Value;
            var inventoryResult = await _servicesManager.TechnicianInventoryItemsService
                                                        .UpsertAsync(request.Technician.Id, request.Tool.Id, request.Quantity);

            if (inventoryResult.IsFailed)
                return Result.Fail(inventoryResult.Errors);
            
            var higherUpInventoryResult = await _servicesManager.SmallInventoryItemsService
                                                                .DecreaseQuantityAsync(request.Tool.Id, request.Quantity);

            if (higherUpInventoryResult.IsFailed)
                return Result.Fail(higherUpInventoryResult.Errors);

            var movementResult = await _servicesManager.InventoryMovementsService
                                                       .CreateAsync(new InventoryMovement_AddReqestModel
                                                       {
                                                           GiverId = 2018, //TODO get cheifs op from authorization
                                                           Quantity = request.Quantity,
                                                           ReceiverId = request.Technician.Id,
                                                           ToolId = request.Tool.Id
                                                       });

            if (movementResult.IsFailed)
                return Result.Fail(movementResult.Errors);

            return request;
        });

        return result.IsFailed
               ? BadRequest(result.GetErrorsDictionary())
               : Ok(result.Value);
    }

    public override async Task<IActionResult> AcceptRequest(int id)
    {
        var result = await _servicesManager.TechnicianInventoryRequestsService
                                           .UpdateAsync(id, new InventoryRequest_PatchRequestModel
                                           {
                                               Status = InventoryRequestStatus.InProgress
                                           });

        return result.IsFailed
               ? BadRequest(result.GetErrorsDictionary())
               : Ok(result.Value);
    }

    public override async Task<IActionResult> CancelRequest(int id, [FromBody, MaxLength(210)] string? conclusionNote)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicesManager.TechnicianInventoryRequestsService
                                           .UpdateAsync(id, new InventoryRequest_PatchRequestModel
                                           {
                                               ConclusionNote = conclusionNote,
                                               Status = InventoryRequestStatus.Cancelled
                                           });

        return result.IsFailed
               ? BadRequest(result.GetErrorsDictionary())
               : Ok(result.Value);
    }
}
