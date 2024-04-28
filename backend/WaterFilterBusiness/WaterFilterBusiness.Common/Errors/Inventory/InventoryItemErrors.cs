using FluentResults;

namespace WaterFilterBusiness.Common.Errors.Inventory;

public static class InventoryItemErrors
{
    public static Error NotFound => GeneralErrors.NotFoundError("InventoryItem");
    public static Error NotEnoughStock => new("There's not enough stock of this item to complete the request");
}
