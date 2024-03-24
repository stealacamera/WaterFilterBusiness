using FluentResults;

namespace WaterFilterBusiness.Common.Errors;

public class NotFoundError : Error
{
    public NotFoundError(string entity = "Entity") : base($"{entity} could not be found")
    {        
    }
}