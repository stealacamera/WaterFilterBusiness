using FluentResults;
using WaterFilterBusiness.Common.DTOs;

namespace WaterFilterBusiness.Common.ErrorHandling.Errors.Finance;

public static class CommissionErrors
{
    public static Error NotFound(string reasonKey) 
        => GeneralErrors.EntityNotFound(nameof(Commission));

    public static Error ExistingRequest(string reasonKey) 
        => new Error(reasonKey, new Error("A request already exists for the given commission"));

    public static Error Unapproved(string reasonKey)
        => new Error(reasonKey, new Error("Commission is not approved"));

    public static Error AlreadyReleased(string reasonKey)
        => new Error(reasonKey, new Error("Commission is already released"));
}
