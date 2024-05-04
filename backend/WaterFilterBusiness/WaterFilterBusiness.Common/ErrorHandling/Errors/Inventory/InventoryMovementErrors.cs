using FluentResults;

namespace WaterFilterBusiness.Common.ErrorHandling.Errors.Inventory;

public static class InventoryMovementErrors
{
    public static Error InvalidReceiver(string reasonKey) 
        => new(reasonKey, new Error("Only technicians and operation chiefs can receive items"));
    
    public static Error InvalidGiver(string reasonKey) 
        => new(reasonKey, new Error("Only inventory managers and operation chiefs can give items"));
}
