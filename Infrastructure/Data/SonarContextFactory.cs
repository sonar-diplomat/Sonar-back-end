using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data;

public class SonarContextFactory : IDesignTimeDbContextFactory<SonarContext>
{
    public SonarContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Sonar"))
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile("appsettings.Development.json", true, true)
            .Build();
        var optionsBuilder = new DbContextOptionsBuilder<SonarContext>();
        var connectionString = configuration.GetConnectionString("SonarContext");
        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("Connection string 'SonarContext' not found.");
        optionsBuilder.UseNpgsql(connectionString);
        return new SonarContext(optionsBuilder.Options);
    }
}