using Ardalis.SmartEnum;

namespace WaterFilterBusiness.Common.Enums;

public sealed class Role : SmartEnum<Role>
{
    public static readonly Role Admin = new("Admin", 1);
    public static readonly Role InventoryManager = new("Inventory manager", 2);
    public static readonly Role OperationsChief = new("Chief of Operations", 3);
    public static readonly Role MarketingChief = new("Marketing Chief", 4);
    public static readonly Role Technician = new("Technician", 5);
    public static readonly Role SalesAgent = new("Sales agent", 6);
    public static readonly Role PhoneOperator = new("Phone operator", 7);

    private Role(string name, int value) : base(name, value)
    {
    }
}
