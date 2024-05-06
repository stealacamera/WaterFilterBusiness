using FluentResults;

namespace WaterFilterBusiness.Common.ErrorHandling.Errors;

public static class CustomerErrors
{
    public static Error EmptyUpdate = GeneralErrors.EmptyUpdate("customer");
    public static Error NotFound(string reasonKey) => GeneralErrors.EntityNotFound(reasonKey, "Customer");
    public static Error UniqueConstraintFailed = new Error(string.Empty, new Error("Name and/or phone number are not unique"));

    public static Error CannotRedlist_ExistingScheduledCalls(string reasonKey) 
        => new Error(reasonKey, new Error("Cannot redlist a customer with scheduled calls"));
}
