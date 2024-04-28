using FluentResults;

namespace WaterFilterBusiness.Common.Errors.Inventory;

public class InventoryRequestErrors
{
    public static Error NotFound => GeneralErrors.NotFoundError("Inventory request");
    public static Error CannotChangeStatus(string oldStatus, string newStatus) => new($"Cannot change status from {oldStatus} to {newStatus}");
}
