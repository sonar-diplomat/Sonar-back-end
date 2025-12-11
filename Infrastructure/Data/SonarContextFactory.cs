using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data;

public class SonarContextFactory : IDesignTimeDbContextFactory<SonarContext>
{
    public SonarContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Sonar"))
            .AddUserSecrets<SonarContext>()
            //.AddEnvironmentVariables()
            .Build();

        DbContextOptionsBuilder<SonarContext> optionsBuilder = new();
        string? connectionString = configuration.GetConnectionString("SonarContext");

        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("Connection string 'SonarContext' not found.");

        optionsBuilder.UseNpgsql(connectionString);
        return new SonarContext(optionsBuilder.Options);
    }
}
