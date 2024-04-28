using FluentResults;

namespace WaterFilterBusiness.Common.ErrorHandling.Errors.Inventory;

public static class InventoryMovementErrors
{
    public static Error InvalidReceiver => new("Only technicians and operation chiefs can receive items");
    public static Error InvalidGiver => new("Only inventory managers and operation chiefs can give items");
}
