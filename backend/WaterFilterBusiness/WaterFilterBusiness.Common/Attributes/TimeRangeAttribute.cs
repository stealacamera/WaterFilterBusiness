using System.ComponentModel.DataAnnotations;
using WaterFilterBusiness.Common.Converters.JsonConverters;

namespace WaterFilterBusiness.Common.Attributes;

public class TimeRangeAttribute : ValidationAttribute
{
    private TimeOnly _minTime, _maxTime;

    /// <summary>
    /// Creates a validator for the allowed range of a TimeOnly object
    /// </summary>
    /// <param name="minTime">String in H:mm:ss format</param>
    /// <param name="maxTime">String in H:mm:ss format</param>
    public TimeRangeAttribute(string minTime, string maxTime)
    {
        _minTime = TimeOnly.ParseExact(minTime, TimeOnlyJsonConverter.TimeFormat);
        _maxTime = TimeOnly.ParseExact(maxTime, TimeOnlyJsonConverter.TimeFormat);
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;
        else if (value is not TimeOnly)
            return new ValidationResult("Unexpected value");

        var timeValue = (TimeOnly)value;

        if (!timeValue.IsBetween(_minTime, _maxTime))
            return new ValidationResult($"Time value has to be within the {_minTime} - {_maxTime} range");

        return ValidationResult.Success;
    }
}
