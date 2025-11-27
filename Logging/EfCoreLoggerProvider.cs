using Entities.Enums;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Logging;

/// <summary>
/// Logger provider for Entity Framework Core that uses our logging system
/// </summary>
public class EfCoreLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) {
        if (categoryName.StartsWith("Microsoft.EntityFrameworkCore"))
            return new EfCoreLogger(categoryName);
        return new NullLogger();
    }

    public void Dispose() { }
}

/// <summary>
/// Logger for Entity Framework Core that redirects logs to our logging system
/// </summary>
public class EfCoreLogger : ILogger
{
    private readonly string _categoryName;

    public EfCoreLogger(string categoryName) {
        _categoryName = categoryName;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel) =>
        logLevel >= Logger.MinimumLogLevel && logLevel != Microsoft.Extensions.Logging.LogLevel.None;

    public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) {
        if (!IsEnabled(logLevel))
            return;

        string message = formatter(state, exception);

        _ = Task.Run(async () => {
            try {
                ulong? guildIdFromQuery = ExtractGuildIdFromMessage(message);
                
                await Logger.AddLog(message, LogCategory.Database, logLevel, 
                    guildId: guildIdFromQuery ?? LogContext.GuildId, 
                    guildName: LogContext.GuildName, 
                    exception: exception);
            }
            catch {
            }
        });
    }

    /// <summary>
    /// Attempts to extract guildId from SQL query if it contains Discord_id parameter
    /// </summary>
    private static ulong? ExtractGuildIdFromMessage(string message) {
        try {
            if (message.Contains("Discord_id") || message.Contains("discordId")) {
                System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(
                    message, 
                    @"@__discordId_\d+='(\d+)'",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                
                if (match.Success && match.Groups.Count > 1) {
                    if (ulong.TryParse(match.Groups[1].Value, out ulong guildId))
                        return guildId;
                }
            }
        }
        catch {
        }
        
        return null;
    }
}

/// <summary>
/// Empty logger for categories we don't process
/// </summary>
public class NullLogger : ILogger
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
    public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel) => false;
    public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
}
