using FluentResults;

namespace WaterFilterBusiness.Common.ErrorHandling.Errors;

public static class ClientMeetingErrors
{
    public static Error NotFound(string reasonKey) 
        => GeneralErrors.EntityNotFound(reasonKey, "Client meeting");

    public static Error InvalidWorker(string reasonKey) 
        => new Error(reasonKey, new Error("Only sales agents and phone operators can view meetings"));
    
    public static Error CustomerIsAlreadyScheduled(string reasonKey) 
        => new Error(reasonKey, new Error("A meeting has already been scheduled with this customer"));
    
    public static Error SalesAgentBusy_CannotAssignMeeting(string reasonKey) 
        => new Error(reasonKey, new Error("Sales agent is occupied with another meeting during the specified time"));
    
    public static Error MeetingOutsideSalesAgentSchedule(string reasonKey) 
        => new Error(reasonKey, new Error("The specified meeting time falls outside the sales agent's schedule"));
    
    public static Error CannotUpdate_OutcomeAlreadySet(string reasonKey) 
        => new Error(reasonKey, new Error("Cannot reset the outcome"));
    
    public static Error CannotUpdate_ExpressMeetingOutcome(string reasonKey) 
        => new Error(reasonKey, new Error("Express meetigns can only be given the successful and failed outcomes"));
}
