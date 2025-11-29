using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Sonar.HealthChecks;

public class SonarContextHealthCheck(SonarContext dbContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            bool canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);
            
            if (canConnect)
            {
                // Try a simple query to ensure database is responsive
                _ = await dbContext.Users.CountAsync(cancellationToken);
                
                return HealthCheckResult.Healthy(
                    "Database is accessible and responsive",
                    new Dictionary<string, object>
                    {
                        ["provider"] = dbContext.Database.ProviderName ?? "Unknown",
                        ["database"] = dbContext.Database.GetDbConnection().Database
                    });
            }
            
            return HealthCheckResult.Unhealthy("Database is not accessible");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "Database health check failed",
                ex,
                new Dictionary<string, object>
                {
                    ["error"] = ex.Message
                });
        }
    }
}

