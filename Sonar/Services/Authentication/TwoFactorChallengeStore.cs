using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace Sonar.Services.Authentication;

/// <summary>
/// Password-verified login challenges. A restart invalidates outstanding challenges.
/// Multiple replicas require shared atomic storage or routing affinity.
/// </summary>
public sealed class TwoFactorChallengeStore(TimeProvider clock)
{
    public static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(5);
    public const int MaximumAttempts = 5;
    private const int Capacity = 4096;
    private readonly object gate = new();
    private readonly Dictionary<string, Entry> entries = new(StringComparer.Ordinal);
    private readonly Dictionary<int, string> activeByUser = new();

    public sealed record Identity(int UserId, string? SecurityStamp, string Email, string DeviceName);
    public sealed record Issued(string Token, string Code, DateTimeOffset ExpiresAt);
    private sealed record Entry(Identity Identity, byte[] Salt, byte[] CodeDigest, DateTimeOffset ExpiresAt)
    {
        public int Attempts { get; set; }
    }

    public Issued? Issue(Identity identity)
    {
        lock (gate)
        {
            var now = clock.GetUtcNow();
            foreach (var expiredKey in entries.Where(pair => pair.Value.ExpiresAt <= now).Select(pair => pair.Key).ToArray())
                Remove(expiredKey);
            if (activeByUser.TryGetValue(identity.UserId, out var previous)) Remove(previous);
            if (entries.Count >= Capacity) return null;

            var token = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
            var code = RandomNumberGenerator.GetInt32(1_000_000).ToString("D6", System.Globalization.CultureInfo.InvariantCulture);
            var salt = RandomNumberGenerator.GetBytes(32);
            var expiresAt = now.Add(Lifetime);
            var key = TokenKey(token);
            entries.Add(key, new Entry(identity, salt, CodeDigest(salt, code), expiresAt));
            activeByUser[identity.UserId] = key;
            return new Issued(token, code, expiresAt);
        }
    }

    /// <summary>Atomically consumes the challenge before a session can be created.</summary>
    public bool TryConsume(string? token, string? code, out Identity? identity)
    {
        identity = null;
        if (token is null || token.Length != 43) return false;
        lock (gate)
        {
            var key = TokenKey(token);
            if (!entries.TryGetValue(key, out var entry)) return false;
            if (entry.ExpiresAt <= clock.GetUtcNow()) { Remove(key); return false; }

            entry.Attempts++;
            var valid = code is { Length: 6 } && code.All(c => c is >= '0' and <= '9') &&
                        CryptographicOperations.FixedTimeEquals(entry.CodeDigest, CodeDigest(entry.Salt, code));
            if (valid || entry.Attempts >= MaximumAttempts) Remove(key);
            if (!valid) return false;
            identity = entry.Identity;
            return true;
        }
    }

    public void Revoke(string token)
    {
        lock (gate) Remove(TokenKey(token));
    }

    private void Remove(string key)
    {
        if (entries.Remove(key, out var entry)) activeByUser.Remove(entry.Identity.UserId);
    }

    private static string TokenKey(string token) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    private static byte[] CodeDigest(byte[] salt, string code) => HMACSHA256.HashData(salt, Encoding.UTF8.GetBytes(code));
}
