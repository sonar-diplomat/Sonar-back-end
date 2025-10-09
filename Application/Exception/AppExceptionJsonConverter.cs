using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.Exception;

public class AppExceptionJsonConverter : JsonConverter<AppException>
{
    public override AppException? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotSupportedException("Deserialization not supported");
    }

    public override void Write(Utf8JsonWriter writer, AppException? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        Dictionary<string, object> dict = value.GetSerializableProperties();
        JsonSerializer.Serialize(writer, dict, options);
    }
}