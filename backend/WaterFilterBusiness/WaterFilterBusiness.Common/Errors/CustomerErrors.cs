using FluentResults;

namespace WaterFilterBusiness.Common.Errors;

public static class CustomerErrors
{
    public static Error EmptyUpdate => GeneralErrors.EmptyUpdate("customer");
    public static Error NotFound => GeneralErrors.NotFoundError("Customer");
    public static Error CannotRedlist_ExistingScheduledCalls => new Error("Cannot redlist a customer with scheduled calls");
}
