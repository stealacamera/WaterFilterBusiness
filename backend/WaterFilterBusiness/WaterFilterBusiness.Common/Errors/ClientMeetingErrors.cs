using FluentResults;

namespace WaterFilterBusiness.Common.Errors;

public static class ClientMeetingErrors
{
    public static Error NotFound = GeneralErrors.NotFoundError("Client meeting");

    public static Error CustomerIsBuyer = new Error("Customer is already a client/buyer");
    public static Error CustomerIsAlreadyScheduled = new Error("Customer already has a meeting scheduled in the future");
    
    public static Error SalesAgentBusy_CannotAssignMeeting = new Error("Sales agent is occupied with another meeting during the specified time");
    public static Error MeetingOutsideSalesAgentSchedule = new Error("The specified meeting time falls outside the sales agent's schedule");
    
    public static Error CannotUpdate_OutcomeAlreadySet = new Error("Cannot reset the outcome");
    public static Error CannotUpdate_ExpressMeetingOutcome = new Error("Express meetigns can only be given the successful and failed outcomes");
}
