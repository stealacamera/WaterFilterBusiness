using System.Text.Json;
using System.Text.Json.Serialization;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.ErrorHandling.Exceptions;

namespace WaterFilterBusiness.Common.Converters.JsonConverters;

public sealed class PaymentTypeJsonConverter : JsonConverter<PaymentType>
{
    public override PaymentType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        PaymentType type;

        if (!PaymentType.TryFromName(reader.GetString(), ignoreCase: true, out type))
            throw new InvalidEnumConversionException(nameof(PaymentType));

        return type;
    }

    public override void Write(Utf8JsonWriter writer, PaymentType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Name);
    }
}
