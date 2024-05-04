using FluentResults;

namespace WaterFilterBusiness.Common.ErrorHandling.Errors.Inventory;

public class InventoryRequestErrors
{
    public static Error NotFound(string reasonKey) => GeneralErrors.EntityNotFound(reasonKey, "Inventory request");

    public static Error CannotChangeStatus(string reasonKey, string oldStatus, string newStatus) 
        => new(reasonKey, new Error($"Cannot change status from {oldStatus} to {newStatus}"));
}
