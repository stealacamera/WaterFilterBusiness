using Ardalis.SmartEnum;

namespace WaterFilterBusiness.Common.Enums;

public sealed class MeetingOutcome : SmartEnum<MeetingOutcome>
{
    public static readonly MeetingOutcome Successful = new("Successful", 1);
    public static readonly MeetingOutcome ClientCancelled = new("Client cancelled", 2);
    public static readonly MeetingOutcome AgentCancelled = new("Agent cancelled", 3);
    public static readonly MeetingOutcome Failed = new("Failed", 4);

    private MeetingOutcome(string name, int value) : base(name, value)
    {
    }
}
