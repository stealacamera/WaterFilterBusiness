using FluentResults;

namespace WaterFilterBusiness.Common.ErrorHandling.Errors.Inventory;

public static class InventoryItemErrors
{
    public static Error NotFound(string reasonKey) => GeneralErrors.EntityNotFound(reasonKey, "InventoryItem");
    
    public static Error NotEnoughStock(string reasonKey) 
        => new(reasonKey, new Error("There's not enough stock of this item to complete the request"));
}
