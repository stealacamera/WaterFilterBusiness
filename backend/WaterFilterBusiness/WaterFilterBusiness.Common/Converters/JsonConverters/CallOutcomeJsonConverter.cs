using System.Text.Json;
using System.Text.Json.Serialization;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.ErrorHandling.Exceptions;

namespace WaterFilterBusiness.Common.Converters.JsonConverters;

public sealed class CallOutcomeJsonConverter : JsonConverter<CallOutcome>
{
    public override CallOutcome? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        CallOutcome outcome;

        if (!CallOutcome.TryFromName(reader.GetString(), ignoreCase: true, out outcome))
            throw new InvalidEnumConversionException(nameof(CallOutcome));

        return outcome;
    }

    public override void Write(Utf8JsonWriter writer, CallOutcome value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Name);
    }
}
