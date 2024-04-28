using System.Text.Json;
using System.Text.Json.Serialization;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.ErrorHandling.Exceptions;

namespace WaterFilterBusiness.Common.Converters.JsonConverters;

public sealed class RoleJsonConverter : JsonConverter<Role>
{
    public override Role? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Role role;

        if (!Role.TryFromName(reader.GetString(), ignoreCase: true, out role))
            throw new InvalidEnumConversionException(nameof(Role));

        return role;
    }

    public override void Write(Utf8JsonWriter writer, Role value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Name);
    }
}
