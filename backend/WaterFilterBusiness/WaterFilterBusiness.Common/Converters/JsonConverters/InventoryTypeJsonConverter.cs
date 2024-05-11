using System.Text.Json;
using System.Text.Json.Serialization;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.ErrorHandling.Exceptions;

namespace WaterFilterBusiness.Common.Converters.JsonConverters;

public sealed class InventoryTypeJsonConverter : JsonConverter<InventoryType>
{
    public override InventoryType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        InventoryType type;

        if (!InventoryType.TryFromName(reader.GetString(), ignoreCase: true, out type))
            throw new InvalidEnumConversionException(nameof(InventoryType));

        return type;
    }

    public override void Write(Utf8JsonWriter writer, InventoryType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Name);
    }
}
