using FluentResults;

namespace WaterFilterBusiness.Common.ErrorHandling.Errors;

public static class SalesAgentScheduleErrors
{
    public static Error TimespanTaken(string reasonKey, string dayOfWeek) 
        => new(reasonKey, new Error($"This timespan is taken for {dayOfWeek}"));
    
    public static Error EmptyUpdate = GeneralErrors.EmptyUpdate("sales agent schedule");
    public static Error UniqueConstraintFailed = new(string.Empty, new Error("The day and time has to be unique for the agent"));
    public static Error SalesAgentNotFound(string reasonKey) => GeneralErrors.EntityNotFound(reasonKey, "Sales agent");
    public static Error NotFound(string reasonKey) => GeneralErrors.EntityNotFound(reasonKey, "Schedule");
}
