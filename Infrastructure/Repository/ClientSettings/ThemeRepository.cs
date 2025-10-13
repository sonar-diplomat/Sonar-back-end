using Application.Abstractions.Interfaces.Repository.Client;
using Entities.Models.ClientSettings;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Infrastructure.Repository.Client;

public class ThemeRepository(SonarContext dbContext) : GenericRepository<Theme>(dbContext), IThemeRepository
{
    public async Task<Theme> GetDefaultAsync()
    {
        return (await context.Themes.FirstOrDefaultAsync(t => t.Id == 1))!;
    }
}
