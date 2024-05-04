using FluentResults;

namespace WaterFilterBusiness.Common.ErrorHandling.Errors;

public static class GeneralErrors
{
    public static Error EntityNotFound(string reasonKey, string entity = "Entity") 
        => new(reasonKey, new Error($"{entity} could not be found"));
    
    public static Error EmptyUpdate(string entity = "Entity") 
        => new(string.Empty, new Error($"At least one attribute of {entity} should be changed/non-empty"));
    
    public static Error UnchangedUpdate 
        => new(string.Empty, new Error("The given values are the same as the current ones"));

    public static Error Unauthorized(string reasonKey) 
        => new(reasonKey, new Error("User is unauthorized to perform action"));
}
