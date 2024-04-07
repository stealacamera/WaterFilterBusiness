using Ardalis.SmartEnum;

namespace WaterFilterBusiness.Common.Enums;

public sealed class CallOutcome : SmartEnum<CallOutcome>
{
    public static readonly CallOutcome Success = new("Success", 1);
    public static readonly CallOutcome NoAnswer = new("No answer", 2);
    public static readonly CallOutcome Rescheduled = new("Rescheduled", 3);
    public static readonly CallOutcome RedList = new("Excessive argument", 4);

    private CallOutcome(string name, int value) : base(name, value)
    {
    }
}