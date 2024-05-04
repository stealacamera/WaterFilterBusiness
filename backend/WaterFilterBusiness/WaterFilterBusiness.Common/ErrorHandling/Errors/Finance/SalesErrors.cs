using FluentResults;

namespace WaterFilterBusiness.Common.ErrorHandling.Errors.Finance;

public static class SalesErrors
{
    public static Error NotFound(string reasonKey) => GeneralErrors.EntityNotFound(reasonKey, "Sale");

    public static Error InvalidUpfrontAmountValue(string reasonKey) 
        => new(reasonKey, new Error("Invalid upfront payment value"));
    
    public static Error InvalidPaymentType(string reasonKey) 
        => new(reasonKey, new Error("Incorrect payment type. Make sure it correlates correctly with the upfront payment value"));

    public static Error CannotCreate_UnsuccessfulMeeting(string reasonKey)
        => new(reasonKey, new Error("Cannot create a sale for an unsuccessful meeting"));

    public static Error AlreadyCreatedForMeeting(string reasonKey)
        => new(reasonKey, new Error("A sale has already been create for the given meeting"));
}
