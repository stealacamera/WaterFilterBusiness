using FluentResults;

namespace WaterFilterBusiness.Common.ErrorHandling.Errors.Finance;

public static class SalesErrors
{
    public static Error NotFound = GeneralErrors.NotFoundError("Sale");

    public static Error InvalidUpfrontAmountValue = new("Invalid upfront payment value");
    public static Error InvalidPaymentType = new("Incorrect payment type. Make sure it correlates correctly with the upfront payment value");
}
