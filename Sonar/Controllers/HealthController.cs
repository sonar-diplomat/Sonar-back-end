using Infrastructure.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;
using System.Reflection;

namespace Sonar.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HealthController(
    HealthCheckService healthCheckService,
    SonarContext dbContext,
    IWebHostEnvironment environment,
    IConfiguration configuration
) : ControllerBase
{
    private static readonly DateTime StartTime = DateTime.UtcNow;

    /// <summary>
    /// Returns detailed health status and service information
    /// </summary>
    /// <returns>Service health status with detailed information</returns>
    [HttpGet]
    [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetHealth()
    {
        HealthReport healthReport = await healthCheckService.CheckHealthAsync();
        
        // Get database status
        DatabaseStatus dbStatus = await GetDatabaseStatusAsync();
        
        // Get version information
        VersionInfo versionInfo = GetVersionInfo();
        
        // Calculate uptime
        TimeSpan uptime = DateTime.UtcNow - StartTime;
        
        HealthResponse response = new HealthResponse
        {
            Status = healthReport.Status.ToString(),
            Timestamp = DateTime.UtcNow,
            Uptime = new UptimeInfo
            {
                TotalSeconds = (long)uptime.TotalSeconds,
                Formatted = FormatUptime(uptime)
            },
            Environment = new EnvironmentInfo
            {
                Name = environment.EnvironmentName,
                IsDevelopment = environment.IsDevelopment(),
                IsProduction = environment.IsProduction(),
                IsStaging = environment.IsStaging()
            },
            Version = versionInfo,
            Database = dbStatus,
            Checks = healthReport.Entries.Select(e => new HealthCheckInfo
            {
                Name = e.Key,
                Status = e.Value.Status.ToString(),
                Description = e.Value.Description,
                Duration = e.Value.Duration.TotalMilliseconds,
                Data = e.Value.Data,
                Exception = e.Value.Exception?.Message
            }).ToList(),
            System = new SystemInfo
            {
                MachineName = Environment.MachineName,
                ProcessorCount = Environment.ProcessorCount,
                WorkingSet = Environment.WorkingSet,
                WorkingSetFormatted = FormatBytes(Environment.WorkingSet)
            }
        };
        
        int statusCode = healthReport.Status == HealthStatus.Healthy 
            ? StatusCodes.Status200OK 
            : StatusCodes.Status503ServiceUnavailable;
        
        return StatusCode(statusCode, response);
    }

    private async Task<DatabaseStatus> GetDatabaseStatusAsync()
    {
        try
        {
            Stopwatch sw = Stopwatch.StartNew();
            bool canConnect = await dbContext.Database.CanConnectAsync();
            sw.Stop();
            
            if (canConnect)
            {
                // Try to execute a simple query
                int userCount = await dbContext.Users.CountAsync();
                
                return new DatabaseStatus
                {
                    Status = "Healthy",
                    Connected = true,
                    ResponseTimeMs = sw.ElapsedMilliseconds,
                    Provider = dbContext.Database.ProviderName ?? "Unknown",
                    DatabaseName = dbContext.Database.GetDbConnection().Database,
                    UserCount = userCount
                };
            }
            
            return new DatabaseStatus
            {
                Status = "Unhealthy",
                Connected = false,
                ResponseTimeMs = sw.ElapsedMilliseconds,
                Provider = dbContext.Database.ProviderName ?? "Unknown"
            };
        }
        catch (Exception ex)
        {
            return new DatabaseStatus
            {
                Status = "Unhealthy",
                Connected = false,
                Error = ex.Message
            };
        }
    }

    private VersionInfo GetVersionInfo()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        AssemblyName assemblyName = assembly.GetName();
        
        FileVersionInfo? fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
        
        return new VersionInfo
        {
            AssemblyVersion = assemblyName.Version?.ToString() ?? "Unknown",
            FileVersion = fileVersionInfo?.FileVersion ?? "Unknown",
            ProductVersion = fileVersionInfo?.ProductVersion ?? "Unknown",
            AssemblyName = assemblyName.Name ?? "Unknown"
        };
    }

    private static string FormatUptime(TimeSpan uptime)
    {
        if (uptime.TotalDays >= 1)
            return $"{(int)uptime.TotalDays}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
        if (uptime.TotalHours >= 1)
            return $"{uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
        if (uptime.TotalMinutes >= 1)
            return $"{uptime.Minutes}m {uptime.Seconds}s";
        return $"{uptime.Seconds}s";
    }

    private static string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}

// Response DTOs
public class HealthResponse
{
    public string Status { get; set; } = null!;
    public DateTime Timestamp { get; set; }
    public UptimeInfo Uptime { get; set; } = null!;
    public EnvironmentInfo Environment { get; set; } = null!;
    public VersionInfo Version { get; set; } = null!;
    public DatabaseStatus Database { get; set; } = null!;
    public List<HealthCheckInfo> Checks { get; set; } = new();
    public SystemInfo System { get; set; } = null!;
}

public class UptimeInfo
{
    public long TotalSeconds { get; set; }
    public string Formatted { get; set; } = null!;
}

public class EnvironmentInfo
{
    public string Name { get; set; } = null!;
    public bool IsDevelopment { get; set; }
    public bool IsProduction { get; set; }
    public bool IsStaging { get; set; }
}

public class VersionInfo
{
    public string AssemblyVersion { get; set; } = null!;
    public string FileVersion { get; set; } = null!;
    public string ProductVersion { get; set; } = null!;
    public string AssemblyName { get; set; } = null!;
}

public class DatabaseStatus
{
    public string Status { get; set; } = null!;
    public bool Connected { get; set; }
    public long? ResponseTimeMs { get; set; }
    public string? Provider { get; set; }
    public string? DatabaseName { get; set; }
    public int? UserCount { get; set; }
    public string? Error { get; set; }
}

public class HealthCheckInfo
{
    public string Name { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string? Description { get; set; }
    public double Duration { get; set; }
    public IReadOnlyDictionary<string, object>? Data { get; set; }
    public string? Exception { get; set; }
}

public class SystemInfo
{
    public string MachineName { get; set; } = null!;
    public int ProcessorCount { get; set; }
    public long WorkingSet { get; set; }
    public string WorkingSetFormatted { get; set; } = null!;
}

