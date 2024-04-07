using Ardalis.SmartEnum;

namespace WaterFilterBusiness.Common.Enums;

public sealed class InventoryType : SmartEnum<InventoryType>
{
    public static readonly InventoryType TechnicianInventory = new ("Technician inventory", 1);
    public static readonly InventoryType SmallInventory = new("Small inventory", 2);
    public static readonly InventoryType BigInventory = new ("Big inventory", 3);

    private InventoryType(string name, int value) : base(name, value)
    {
    }
}
