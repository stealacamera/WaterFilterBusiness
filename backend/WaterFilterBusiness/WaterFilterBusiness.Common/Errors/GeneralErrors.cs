using FluentResults;

namespace WaterFilterBusiness.Common.Errors;

public static class GeneralErrors
{
    public static Error NotFoundError(string entity = "Entity") => new($"{entity} could not be found");
    public static Error EmptyUpdate(string entity = "Entity") => new($"At least one attribute of {entity} should be changed/non-empty");
    public static Error UnchangedUpdate => new("The given values are the same as the current ones");
}
