using FluentResults;

namespace WaterFilterBusiness.Common.Errors;

public static class CallErrors
{
    public static Error NotPhoneAgent => new("Only phone agents can be assigned to a call");
    public static Error CannotCreate_RedlistedCustomer => new("A call cannot be scheduled with a redlisted customer");
    public static Error CannotCreate_CustomerAlreadyScheduled => new("Customer is already scheduled to a call with a phone operator");
}
