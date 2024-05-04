using FluentResults;

namespace WaterFilterBusiness.Common.ErrorHandling.Errors;

public static class CallErrors
{
    public static Error NotFound(string reasonKey) => GeneralErrors.EntityNotFound(reasonKey, "Call");

    public static Error NotPhoneAgent(string reasonKey) 
        => new(reasonKey, new Error("Only phone agents can be assigned to a call"));
}

public static class ScheduledCallErrors
{
    public static Error AlreadyCompleted(string reasonKey) => new(reasonKey, new Error("Call is already completed"));

    public static Error CannotCreate_RedlistedCustomer(string reasonKey) 
        => new(reasonKey, new Error("A call cannot be scheduled with a redlisted customer"));
    
    public static Error CannotCreate_CustomerAlreadyScheduled(string reasonKey) 
        => new(reasonKey, new Error("Customer is already scheduled to a call with a phone operator"));

    public static Error CannotCreate_PhoneAgentBusy(string reasonKey, int withinMinutes)
        => new(reasonKey, new Error($"Phone agent has a scheduled call within {withinMinutes} of the given schedule"));
}