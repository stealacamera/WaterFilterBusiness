using FluentResults;
using WaterFilterBusiness.Common.Errors;

namespace WaterFilterBusiness.Common.Results;

public sealed class GeneralResults
{
    public static Result NotFoundFailResult(string entity = "Entity") => Result.Fail(new NotFoundError(entity));
}
