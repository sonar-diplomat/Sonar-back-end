using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Entities.Enums;

namespace Application.Exception
{
    public class AppException : System.Exception
    {
        [JsonIgnore]
        public ErrorType ErrorId { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public virtual new string Message { get; set; } = null!;
        public virtual string? Details { get; set; }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
        }
    }
}
