using System.Text.Json;
using System.Text.Json.Serialization;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.ErrorHandling.Exceptions;

namespace WaterFilterBusiness.Common.Converters.JsonConverters;

public sealed class WeekdayJsonConverter : JsonConverter<Weekday>
{
    public override Weekday Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Weekday weekday;

        if (!Weekday.TryFromName(reader.GetString(), ignoreCase: true, out weekday))
            throw new InvalidEnumConversionException(nameof(Weekday));

        return weekday;
    }

    public override void Write(Utf8JsonWriter writer, Weekday value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Name);
    }
}
