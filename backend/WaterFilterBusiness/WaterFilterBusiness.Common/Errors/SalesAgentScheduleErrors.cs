using FluentResults;

namespace WaterFilterBusiness.Common.Errors;

public static class SalesAgentScheduleErrors
{
    public static Error TimespanTaken(string dayOfWeek) => new($"This timespan is taken for {dayOfWeek}");
    public static Error EmptyUpdate => GeneralErrors.EmptyUpdate("sales agent schedule");
    public static Error SalesAgentNotFound => GeneralErrors.NotFoundError("Sales agent");
    public static Error NotFound => GeneralErrors.NotFoundError("Schedule");
}
