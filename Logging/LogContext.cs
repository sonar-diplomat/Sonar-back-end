namespace Logging;

/// <summary>
/// Logging context for current async thread
/// </summary>
public static class LogContext
{
    private static readonly AsyncLocal<ulong?> _guildId = new();
    private static readonly AsyncLocal<string?> _guildName = new();

    /// <summary>
    /// Current guild ID in execution context
    /// </summary>
    public static ulong? GuildId {
        get => _guildId.Value;
        set => _guildId.Value = value;
    }

    /// <summary>
    /// Current guild name in execution context
    /// </summary>
    public static string? GuildName {
        get => _guildName.Value;
        set => _guildName.Value = value;
    }

    /// <summary>
    /// Set guild context
    /// </summary>
    public static void SetGuild(ulong? guildId, string? guildName = null) {
        GuildId = guildId;
        GuildName = guildName;
    }

    /// <summary>
    /// Clear guild context
    /// </summary>
    public static void Clear() {
        GuildId = null;
        GuildName = null;
    }
}
