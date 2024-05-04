using FluentResults;

namespace WaterFilterBusiness.Common.ErrorHandling.Errors;

public static class UserErrors
{
    public static Error NotFound(string reasonKey) => GeneralErrors.EntityNotFound(reasonKey, "User");
    
    public static Error RoleNotFound(string reasonKey) 
        => new Error(reasonKey, new Error("The given role does not exist"));
}
