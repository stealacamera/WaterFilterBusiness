namespace WaterFilterBusiness.Common.ErrorHandling.Exceptions;

public class InvalidArgumentsException : BaseException
{
    public InvalidArgumentsException() : base("Invalid arguments supplied")
    {
    }
}
