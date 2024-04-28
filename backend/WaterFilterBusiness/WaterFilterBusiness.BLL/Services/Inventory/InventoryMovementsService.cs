using FluentResults;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.Inventory;
using WaterFilterBusiness.Common.DTOs.ViewModels;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.Errors.Inventory;
using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL.Services.Inventory;

public interface IInventoryMovementsService
{
    Task<Result<InventoryMovement>> CreateAsync(InventoryMovement_AddReqestModel movement);
    Task<CursorPaginatedList<InventoryMovement, int>> GetAllAsync(int cursor, int pageSize);
}

internal class InventoryMovementsService : Service, IInventoryMovementsService
{
    public InventoryMovementsService(IWorkUnit workUnit, IUtilityService utilityService) : base(workUnit, utilityService)
    {
    }

    public async Task<Result<InventoryMovement>> CreateAsync(InventoryMovement_AddReqestModel movement)
    {
        if (!await _utilityService.DoesInventoryItemExistAsync(movement.ToolId))
            return InventoryItemErrors.NotFound;

        if (!await _utilityService.DoesUserExistAsync(movement.ReceiverId))
            return new Error("Receiver not found");
        else if (!await _utilityService.DoesUserExistAsync(movement.GiverId))
            return new Error("Giver not found");

        InventoryType? giverInventory = await GetInventoryTypeFromUser(movement.GiverId),
                      receiverInventory = await GetInventoryTypeFromUser(movement.ReceiverId);

        var participantsValidation = AreParticipantsValid(giverInventory, receiverInventory);

        if (participantsValidation.IsFailed)
            return Result.Fail(participantsValidation.Errors);

        var dbModel = await _workUnit.InventoryMovementsRepository
                                     .AddAsync(ConvertModelToEntity(movement, giverInventory, receiverInventory));

        await _workUnit.SaveChangesAsync();
        return ConvertEntityToModel(dbModel);
    }

    public async Task<CursorPaginatedList<InventoryMovement, int>> GetAllAsync(int cursor, int pageSize)
    {
        var result = await _workUnit.InventoryMovementsRepository
                                    .GetAllAsync(cursor, pageSize);

        return new CursorPaginatedList<InventoryMovement, int>
        {
            Cursor = cursor,
            Values = result.Values.Select(ConvertEntityToModel).ToList()
        };
    }

    private Result AreParticipantsValid(InventoryType? giverInventory, InventoryType? receiverInventory)
    {
        // Check validity of each participant
        if (giverInventory == null || receiverInventory == null)
            return new Error("Only technicians, operation chiefs, and inventory managers can receive or give an item");
        else if (receiverInventory == InventoryType.BigInventory)
            return InventoryMovementErrors.InvalidReceiver;
        else if (giverInventory == InventoryType.TechnicianInventory)
            return InventoryMovementErrors.InvalidGiver;

        // Check validity of exchange        
        if (giverInventory == InventoryType.SmallInventory && receiverInventory != InventoryType.TechnicianInventory)
            return InventoryMovementErrors.InvalidReceiver;
        else if (giverInventory == InventoryType.BigInventory && receiverInventory != InventoryType.SmallInventory)
            return InventoryMovementErrors.InvalidReceiver;

        return Result.Ok();
    }

    private async Task<InventoryType?> GetInventoryTypeFromUser(int userId)
    {
        if (await _utilityService.IsUserInRoleAsync(userId, Role.Technician))
            return InventoryType.TechnicianInventory;
        else if (await _utilityService.IsUserInRoleAsync(userId, Role.InventoryManager))
            return InventoryType.BigInventory;
        else if (await _utilityService.IsUserInRoleAsync(userId, Role.OperationsChief))
            return InventoryType.SmallInventory;

        return null;
    }

    private DAL.Entities.Inventory.InventoryItemMovement ConvertModelToEntity(
        InventoryMovement_AddReqestModel model,
        InventoryType giverInventory,
        InventoryType receiverInventory)
    {
        return new DAL.Entities.Inventory.InventoryItemMovement
        {
            GiverId = model.GiverId,
            OccurredAt = DateTime.Now,
            Quantity = model.Quantity,
            ReceiverId = model.ReceiverId,
            ToolId = model.ToolId,
            FromInventoryId = giverInventory.Value,
            ToInventoryId = receiverInventory.Value
        };
    }

    private InventoryMovement ConvertEntityToModel(DAL.Entities.Inventory.InventoryItemMovement entity)
    {
        return new InventoryMovement
        {
            Id = entity.Id,
            FromInventory = InventoryType.FromValue(entity.FromInventoryId),
            Giver = new User_BriefDescription { Id = entity.GiverId },
            OccurredAt = entity.OccurredAt,
            Quantity = entity.Quantity,
            Receiver = new User_BriefDescription { Id = entity.ReceiverId },
            ToInventory = InventoryType.FromValue(entity.ToInventoryId),
            Tool = new InventoryItem { Id = entity.ToolId }
        };
    }
}
