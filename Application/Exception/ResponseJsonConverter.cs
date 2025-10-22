using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.Exception;

public class ResponseJsonConverter : JsonConverter<Response>
{
    public override Response? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotSupportedException("Deserialization not supported");
    }

    public override void Write(Utf8JsonWriter writer, Response value, JsonSerializerOptions options)
    {
        Dictionary<string, object> dict = value.GetSerializableProperties();
        string traceId = Activity.Current?.TraceId.ToString() ?? Activity.Current?.Id ?? string.Empty;
        Dictionary<string, object> metadata = new()
        {
            ["timestamp"] = DateTime.UtcNow,
            ["traceId"] = traceId
        };
        dict["metadata"] = metadata;
        JsonSerializer.Serialize(writer, dict, options);
    }
}