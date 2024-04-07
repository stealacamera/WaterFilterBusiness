using System.Text.Json;
using System.Text.Json.Serialization;

namespace WaterFilterBusiness.Common.JsonConverters;

public sealed class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    public static readonly string DateFormat = "yyyy-M-d";

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        DateOnly date;

        if (!DateOnly.TryParseExact(reader.GetString(), DateFormat, out date))
            throw new JsonException();

        return date;
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateFormat));
    }
}
