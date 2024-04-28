using System.Text.Json;
using System.Text.Json.Serialization;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.ErrorHandling.Exceptions;

namespace WaterFilterBusiness.Common.Converters.JsonConverters;

public sealed class TimeOnlyJsonConverter : JsonConverter<TimeOnly>
{
    public static readonly string TimeFormat = "H:mm:ss";

    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        TimeOnly time;

        if (!TimeOnly.TryParseExact(reader.GetString(), TimeFormat, out time))
            throw new InvalidEnumConversionException("Time");

        return time;
    }

    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(TimeFormat));
    }
}