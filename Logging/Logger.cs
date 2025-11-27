using Entities.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Logging;

/// <summary>
/// Logger with support for categories, guilds and file saving
/// </summary>
public static class Logger
{
    private static readonly object _logLock = new();
    private static readonly ConcurrentQueue<LogEntry> _logQueue = new();
    private static readonly string _logsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "logs");
    private static readonly string _generalLogsDirectory = Path.Combine(_logsDirectory, "general");
    private static readonly string _guildLogsDirectory = Path.Combine(_logsDirectory, "guilds");

    private static Timer? _flushTimer;

    public static LogLevel MinimumLogLevel { get; set; } = LogLevel.Information;
    public static bool EnableFileLogging { get; set; } = true;
    public static bool EnableConsoleLogging { get; set; } = true;

    public const ConsoleColor _DEFAULT_OUTLINE_COLOR = ConsoleColor.Cyan;
    public static ConsoleColor outline_color = ConsoleColor.Cyan;

    /// <summary>
    /// Automatically determines logging depth based on call stack
    /// Counts number of method nesting levels until Logger.AddLog call
    /// </summary>
    private static int CalculateDepth() {
        try {
            StackTrace stackTrace = new(skipFrames: 1, fNeedFileInfo: false);
            int depth = 0;
            bool foundLoggerCall = false;

            for (int i = 0; i < stackTrace.FrameCount; i++) {
                StackFrame? frame = stackTrace.GetFrame(i);
                if (frame == null)
                    continue;

                MethodBase? method = frame.GetMethod();
                if (method == null)
                    continue;

                string? declaringTypeName = method.DeclaringType?.FullName;

                if (declaringTypeName != null && declaringTypeName.StartsWith("Logging.Logger")) {
                    foundLoggerCall = true;
                    continue;
                }

                if (declaringTypeName != null &&
                    (declaringTypeName.StartsWith("System.") ||
                     declaringTypeName.StartsWith("Microsoft.") ||
                     declaringTypeName.StartsWith("Discord.")))
                    continue;

                if (foundLoggerCall)
                    depth++;

                if (depth >= 15)
                    break;
            }

            return depth;
        }
        catch {
            return 0;
        }
    }

    public static void Initialize(IConfiguration configuration) {
        if (!string.IsNullOrEmpty(configuration["logging:level"]) &&
            Enum.TryParse<LogLevel>(configuration["logging:level"], ignoreCase: true, out LogLevel parsedLevel))
            MinimumLogLevel = parsedLevel;

        if (bool.TryParse(configuration["logging:enableFileLogging"], out bool enableFile))
            EnableFileLogging = enableFile;

        if (bool.TryParse(configuration["logging:enableConsoleLogging"], out bool enableConsole))
            EnableConsoleLogging = enableConsole;

        if (EnableFileLogging) {
            Directory.CreateDirectory(_logsDirectory);
            Directory.CreateDirectory(_generalLogsDirectory);
            Directory.CreateDirectory(_guildLogsDirectory);

            _flushTimer = new Timer(FlushLogsToFiles, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
        }
    }

    /// <summary>
    /// Determines log category based on class attribute or file path
    /// </summary>
    private static LogCategory DetermineCategory(string filePath, string caller) {
        LogCategory? categoryFromAttribute = GetCategoryFromAttribute();
        if (categoryFromAttribute.HasValue)
            return categoryFromAttribute.Value;

        if (string.IsNullOrEmpty(filePath))
            return LogCategory.General;

        string normalizedPath = filePath.Replace('\\', '/').ToLowerInvariant();
        string fileName = Path.GetFileNameWithoutExtension(filePath);

        if (normalizedPath.Contains("/music_parts/") || normalizedPath.Contains("\\music_parts\\")) {
            if (fileName.Contains("MusicClientService"))
                return LogCategory.Music;
            if (fileName.Contains("AudioDownloader"))
                return LogCategory.AudioDownload;
            return LogCategory.Music;
        }

        if (normalizedPath.Contains("/handlers/") || normalizedPath.Contains("\\handlers\\")) {
            if (fileName.Contains("CommandHandler") || fileName.Contains("Command"))
                return LogCategory.Command;
            if (fileName.Contains("ComponentHandler") || fileName.Contains("Component"))
                return LogCategory.Component;
            if (fileName.Contains("EventHandler") || fileName.Contains("Event"))
                return LogCategory.Event;
        }

        if (normalizedPath.Contains("/services/") || normalizedPath.Contains("\\services\\")) {
            if (fileName.Contains("VideoFinder"))
                return LogCategory.VideoFinder;
            if (fileName.Contains("DiscordBot") || fileName.Contains("Discord"))
                return LogCategory.Discord;
            return LogCategory.Service;
        }

        if (normalizedPath.Contains("/repository/") || normalizedPath.Contains("\\repository\\") ||
            normalizedPath.Contains("/infrastructure/") || normalizedPath.Contains("\\infrastructure\\"))
            return LogCategory.Database;

        if (fileName.Contains("Ffmpeg") || fileName.Contains("FFmpeg"))
            return LogCategory.FFmpeg;

        if (normalizedPath.Contains("/commands/") || normalizedPath.Contains("\\commands\\"))
            return LogCategory.Command;

        if (caller.Contains("Database") || caller.Contains("Repository") || caller.Contains("Context"))
            return LogCategory.Database;

        return LogCategory.General;
    }

    /// <summary>
    /// Gets log category from class attribute via reflection
    /// </summary>
    private static LogCategory? GetCategoryFromAttribute() {
        try {
            StackTrace stackTrace = new(skipFrames: 3, fNeedFileInfo: false);

            for (int i = 0; i < stackTrace.FrameCount; i++) {
                StackFrame? frame = stackTrace.GetFrame(i);
                if (frame == null)
                    continue;

                MethodBase? method = frame.GetMethod();
                if (method == null)
                    continue;

                Type? declaringType = method.DeclaringType;
                if (declaringType == null)
                    continue;

                LogCategoryAttribute? attribute = declaringType.GetCustomAttribute<LogCategoryAttribute>();
                if (attribute != null)
                    return attribute.Category;

                Type? baseType = declaringType.BaseType;
                while (baseType != null && baseType != typeof(object)) {
                    LogCategoryAttribute? baseAttribute = baseType.GetCustomAttribute<LogCategoryAttribute>();
                    if (baseAttribute != null)
                        return baseAttribute.Category;
                    baseType = baseType.BaseType;
                }
            }
        }
        catch {
        }

        return null;
    }

    /// <summary>
    /// Add log with category specification (if you need to override automatic detection)
    /// </summary>
    public static async Task AddLog(
        string message,
        LogCategory category,
        LogLevel msgType = LogLevel.Information,
        ulong? guildId = null,
        string? guildName = null,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        Exception? exception = null) {
        await AddLogInternal(message, category, msgType, guildId, guildName, null, caller, file, line, exception);
    }

    /// <summary>
    /// Add log with named parameter support for guild
    /// </summary>
    public static async Task AddLog(
        string message,
        LogLevel msgType = LogLevel.Information,
        ulong? guildId = null,
        string? guildName = null,
        Exception? exception = null,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0) {
        await AddLogInternal(message, null, msgType, guildId, guildName, null, caller, file, line, exception);
    }


    /// <summary>
    /// Internal method for adding log
    /// </summary>
    private static async Task AddLogInternal(
        string message,
        LogCategory? category = null,
        LogLevel level = LogLevel.Information,
        ulong? guildId = null,
        string? guildName = null,
        ConsoleColor? color = null,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        Exception? exception = null) {
        await Task.Yield();

        guildId ??= LogContext.GuildId;
        guildName ??= LogContext.GuildName;

        LogCategory determinedCategory = category ?? DetermineCategory(file, caller);

        if (level < MinimumLogLevel)
            return;

        string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string fileShort = Path.GetFileName(file);
        int threadId = Thread.CurrentThread.ManagedThreadId;

        ConsoleColor consoleColor = color ??
            (level >= LogLevel.Error ? ConsoleColor.Red :
             level == LogLevel.Warning ? ConsoleColor.Yellow :
             ConsoleColor.White);

        int calculatedDepth = CalculateDepth();

        LogEntry logEntry = new LogEntry {
            Timestamp = DateTime.Now,
            Level = level,
            Category = determinedCategory,
            GuildId = guildId,
            GuildName = guildName,
            Message = message,
            Caller = caller,
            File = fileShort,
            Line = line,
            ThreadId = threadId,
            Exception = exception?.ToString(),
            Depth = calculatedDepth
        };

        if (EnableFileLogging)
            _logQueue.Enqueue(logEntry);

        if (EnableConsoleLogging)
            await WriteToConsole(logEntry, consoleColor, time, fileShort);
    }

    private static async Task WriteToConsole(LogEntry entry, ConsoleColor color, string time, string fileShort) {
        //if (entry.GuildId is not (null or 1364606623775068191))
        //    return;

        await Task.Yield();

        string offset = "";
        for (int q = 0; q < entry.Depth; q++)
            offset += " |";
        if (entry.Depth > 0)
            offset += "->";
        else if (color == ConsoleColor.White)
            color = outline_color;

        string categoryStr = $"[{entry.Category}]";
        string guildStr = entry.GuildId.HasValue ? $"[Guild: {entry.GuildName ?? entry.GuildId.ToString()}]" : "";
        string levelStr = entry.Level switch {
            LogLevel.Critical => "CRI",
            LogLevel.Error => "ERR",
            LogLevel.Warning => "WAR",
            LogLevel.Information => "INF",
            LogLevel.Debug => "DBG",
            LogLevel.Trace => "TRC",
            _ => "INF"
        };

        lock (_logLock) {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"[{time}]");
            Console.ForegroundColor = outline_color;
            Console.Write($"{offset}");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write($"{categoryStr} ");
            if (!string.IsNullOrEmpty(guildStr)) {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write($"{guildStr} ");
            }
            Console.ForegroundColor = color;
            Console.Write($"{entry.Message} ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"[{levelStr}][{entry.ThreadId}][{entry.Caller} @ {fileShort}:{entry.Line}]");
            Console.WriteLine();
            Console.ResetColor();
        }
    }

    private static async void FlushLogsToFiles(object? state) {
        if (!EnableFileLogging || _logQueue.IsEmpty)
            return;

        List<LogEntry> logsToWrite = new();
        while (_logQueue.TryDequeue(out LogEntry? logEntry))
            logsToWrite.Add(logEntry);

        if (logsToWrite.Count == 0)
            return;

        try {
            IEnumerable<IGrouping<(LogCategory Category, ulong? GuildId), LogEntry>> groupedLogs = logsToWrite.GroupBy(log => (log.Category, log.GuildId));

            foreach (IGrouping<(LogCategory Category, ulong? GuildId), LogEntry> group in groupedLogs) {
                string fileName;
                string directory;

                if (group.Key.GuildId.HasValue) {
                    directory = Path.Combine(_guildLogsDirectory, group.Key.GuildId.Value.ToString());
                    Directory.CreateDirectory(directory);
                    fileName = Path.Combine(directory, $"{group.Key.Category}_{DateTime.Now:yyyy-MM-dd}.json");
                }
                else {
                    directory = Path.Combine(_generalLogsDirectory, group.Key.Category.ToString());
                    Directory.CreateDirectory(directory);
                    fileName = Path.Combine(directory, $"{DateTime.Now:yyyy-MM-dd}.json");
                }

                List<LogEntry> existingLogs = new();
                if (File.Exists(fileName)) {
                    try {
                        string json = await File.ReadAllTextAsync(fileName);
                        existingLogs = JsonSerializer.Deserialize<List<LogEntry>>(json) ?? new List<LogEntry>();
                    }
                    catch {
                        existingLogs = new List<LogEntry>();
                    }
                }

                existingLogs.AddRange(group);

                JsonSerializerOptions options = new JsonSerializerOptions {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                string jsonContent = JsonSerializer.Serialize(existingLogs, options);
                await File.WriteAllTextAsync(fileName, jsonContent);
            }
        }
        catch (Exception ex) {
            Console.WriteLine($"[ERROR] Failed to write logs to file: {ex.Message}");
        }
    }

    public static void Shutdown() {
        _flushTimer?.Dispose();
        FlushLogsToFiles(null);
    }

    /// <summary>
    /// Get logs for specific guild
    /// </summary>
    public static async Task<List<LogEntry>> GetGuildLogsAsync(ulong guildId, LogCategory? category = null, DateTime? fromDate = null, DateTime? toDate = null) {
        List<LogEntry> logs = new();
        string guildDir = Path.Combine(_guildLogsDirectory, guildId.ToString());

        if (!Directory.Exists(guildDir))
            return logs;

        string searchPattern = category.HasValue ? $"{category}*.json" : "*.json";
        string[] files = Directory.GetFiles(guildDir, searchPattern);

        foreach (string file in files) {
            try {
                string json = await File.ReadAllTextAsync(file);
                List<LogEntry> fileLogs = JsonSerializer.Deserialize<List<LogEntry>>(json) ?? new List<LogEntry>();

                if (fromDate.HasValue || toDate.HasValue) {
                    fileLogs = fileLogs.Where(log =>
                        (!fromDate.HasValue || log.Timestamp >= fromDate.Value) &&
                        (!toDate.HasValue || log.Timestamp <= toDate.Value)
                    ).ToList();
                }

                logs.AddRange(fileLogs);
            }
            catch {
            }
        }

        return logs.OrderBy(log => log.Timestamp).ToList();
    }

    /// <summary>
    /// Get logs by category
    /// </summary>
    public static async Task<List<LogEntry>> GetCategoryLogsAsync(LogCategory category, DateTime? fromDate = null, DateTime? toDate = null) {
        List<LogEntry> logs = new();
        string categoryDir = Path.Combine(_generalLogsDirectory, category.ToString());

        if (!Directory.Exists(categoryDir))
            return logs;

        string[] files = Directory.GetFiles(categoryDir, "*.json");

        foreach (string file in files) {
            try {
                string json = await File.ReadAllTextAsync(file);
                List<LogEntry> fileLogs = JsonSerializer.Deserialize<List<LogEntry>>(json) ?? new List<LogEntry>();

                if (fromDate.HasValue || toDate.HasValue) {
                    fileLogs = fileLogs.Where(log =>
                        (!fromDate.HasValue || log.Timestamp >= fromDate.Value) &&
                        (!toDate.HasValue || log.Timestamp <= toDate.Value)
                    ).ToList();
                }

                logs.AddRange(fileLogs);
            }
            catch {
            }
        }

        return logs.OrderBy(log => log.Timestamp).ToList();
    }

    /// <summary>
    /// Get the most recent log file (by last write time)
    /// </summary>
    public static async Task<(string FilePath, string Content)?> GetLastLogFileAsync() {
        if (!Directory.Exists(_logsDirectory))
            return null;

        string? lastFile = null;
        DateTime lastWriteTime = DateTime.MinValue;

        // Search in general logs
        if (Directory.Exists(_generalLogsDirectory)) {
            string[] generalFiles = Directory.GetFiles(_generalLogsDirectory, "*.json", SearchOption.AllDirectories);
            foreach (string file in generalFiles) {
                DateTime writeTime = File.GetLastWriteTime(file);
                if (writeTime > lastWriteTime) {
                    lastWriteTime = writeTime;
                    lastFile = file;
                }
            }
        }

        // Search in guild logs
        if (Directory.Exists(_guildLogsDirectory)) {
            string[] guildFiles = Directory.GetFiles(_guildLogsDirectory, "*.json", SearchOption.AllDirectories);
            foreach (string file in guildFiles) {
                DateTime writeTime = File.GetLastWriteTime(file);
                if (writeTime > lastWriteTime) {
                    lastWriteTime = writeTime;
                    lastFile = file;
                }
            }
        }

        if (lastFile == null || !File.Exists(lastFile))
            return null;

        try {
            string content = await File.ReadAllTextAsync(lastFile);
            return (lastFile, content);
        }
        catch {
            return null;
        }
    }
}
