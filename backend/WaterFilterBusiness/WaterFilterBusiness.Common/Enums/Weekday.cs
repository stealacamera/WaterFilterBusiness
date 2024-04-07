using Ardalis.SmartEnum;

namespace WaterFilterBusiness.Common.Enums;

public sealed class Weekday : SmartEnum<Weekday>
{
    public static readonly Weekday Sunday = new("Sunday", 1);
    public static readonly Weekday Monday = new("Monday", 2);
    public static readonly Weekday Tuesday = new("Tuesday", 3);
    public static readonly Weekday Wednesday = new("Wednesday", 4);
    public static readonly Weekday Thursday = new("Thursday", 5);
    public static readonly Weekday Friday = new("Friday", 6);
    public static readonly Weekday Saturday = new("Saturday", 7);

    private Weekday(string name, int value) : base(name, value)
    {
    }
}
