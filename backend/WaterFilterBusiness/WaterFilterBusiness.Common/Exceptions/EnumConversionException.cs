namespace WaterFilterBusiness.Common.Exceptions;

public class InvalidEnumConversionException : BaseException
{
    public InvalidEnumConversionException(string enumName) : base($"Invalid {enumName} value")
    {
    }
}
