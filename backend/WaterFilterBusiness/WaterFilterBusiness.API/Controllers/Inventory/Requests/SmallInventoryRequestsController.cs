using FluentResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.Inventory;
using WaterFilterBusiness.Common.Enums;

namespace WaterFilterBusiness.API.Controllers.Inventory.Requests;

[Route("api/[controller]")]
[ApiController]
public class SmallInventoryRequestsController : BaseInventoryRequestsController
{
    public SmallInventoryRequestsController(IServicesManager servicesManager) : base(servicesManager)
    {
    }

    public override async Task<IActionResult> AcceptRequest(int id)
    {
        var result = await _servicesManager.SmallInventoryRequestsService
                                           .UpdateAsync(id, new InventoryRequest_PatchRequestModel
                                           {
                                               Status = InventoryRequestStatus.InProgress
                                           });

        return result.IsFailed ? BadRequest(result.Errors) : Ok(result.Value);
    }

    public override async Task<IActionResult> CancelRequest(int id, [FromBody, MaxLength(210)] string? conclusionNote)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicesManager.SmallInventoryRequestsService
                                           .UpdateAsync(id, new InventoryRequest_PatchRequestModel
                                           {
                                               ConclusionNote = conclusionNote,
                                               Status = InventoryRequestStatus.Cancelled
                                           });

        return result.IsFailed ? BadRequest(result.Errors) : Ok(result.Value);
    }

    public override async Task<IActionResult> CompleteRequest(int id, [FromBody, MaxLength(210)] string? conclusionNote)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicesManager.WrapInTransactionAsync<SmallInventoryRequest>(async () =>
        {
            var requestResult = await _servicesManager.SmallInventoryRequestsService
                                                      .UpdateAsync(id, new InventoryRequest_PatchRequestModel
                                                      {
                                                          ConclusionNote = conclusionNote,
                                                          Status = InventoryRequestStatus.Completed
                                                      });

            if (requestResult.IsFailed)
                return Result.Fail(requestResult.Errors);

            var request = requestResult.Value;
            var inventoryResult = await _servicesManager.SmallInventoryItemsService
                                                        .UpsertAsync(request.Tool.Id, request.Quantity);

            if (inventoryResult.IsFailed)
                return Result.Fail(inventoryResult.Errors);

            var movementResult = await _servicesManager.InventoryMovementsService
                                                       .CreateAsync(new InventoryMovement_AddReqestModel
                                                       {
                                                           GiverId = 1, // TODO get from authentication
                                                           Quantity = request.Quantity,
                                                           ReceiverId = request.Requester.Id,
                                                           ToolId = request.Tool.Id
                                                       });

            if (movementResult.IsFailed)
                return Result.Fail(movementResult.Errors);

            return request;
        });

        return result.IsFailed ? BadRequest(result.Errors) : Ok(result.Value);
    }

    public override async Task<IActionResult> Create(InventoryRequest_AddRequestModel request)
    {
        // TODO get user id
        var result = await _servicesManager.SmallInventoryRequestsService
                                           .CreateAsync(1, request);

        return result.IsFailed ? BadRequest(result.Errors) : Created(string.Empty, result.Value);
    }

    public override async Task<IActionResult> GetAll(int page, int pageSize)
    {
        var requests = await _servicesManager.SmallInventoryRequestsService
                                           .GetAllAsync(page, pageSize);

        foreach (var request in requests.Values)
            request.Tool = (await _servicesManager.InventoryItemsService.GetByIdAsync(request.Tool.Id)).Value;

        return Ok(requests);
    }
}
