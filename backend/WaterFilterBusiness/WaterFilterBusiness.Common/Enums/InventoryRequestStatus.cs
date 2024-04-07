using Ardalis.SmartEnum;

namespace WaterFilterBusiness.Common.Enums;

public sealed class InventoryRequestStatus : SmartEnum<InventoryRequestStatus>
{
    public static readonly InventoryRequestStatus Pending = new("Pending", 1);
    public static readonly InventoryRequestStatus InProgress = new("In progress", 2);
    public static readonly InventoryRequestStatus Completed = new("Completed", 3);
    public static readonly InventoryRequestStatus Cancelled = new("Cancelled", 4);

    private InventoryRequestStatus(string name, int value) : base(name, value)
    {
    }
}
