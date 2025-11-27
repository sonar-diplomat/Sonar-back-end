using Entities.Enums;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace Logging;

/// <summary>
/// Structured log entry for storage and processing
/// </summary>
public class LogEntry
{
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("level")]
    public Microsoft.Extensions.Logging.LogLevel Level { get; set; }

    [JsonPropertyName("category")]
    public LogCategory Category { get; set; }

    [JsonPropertyName("guildId")]
    public ulong? GuildId { get; set; }

    [JsonPropertyName("guildName")]
    public string? GuildName { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("caller")]
    public string? Caller { get; set; }

    [JsonPropertyName("file")]
    public string? File { get; set; }

    [JsonPropertyName("line")]
    public int Line { get; set; }

    [JsonPropertyName("threadId")]
    public int ThreadId { get; set; }

    [JsonPropertyName("exception")]
    public string? Exception { get; set; }

    [JsonPropertyName("depth")]
    public int Depth { get; set; }
}
