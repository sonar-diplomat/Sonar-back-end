using Application.Abstractions.Interfaces.Repository.Client;
using Application.Extensions;
using Entities.Models.ClientSettings;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Infrastructure.Repository.Client;

public class SettingsRepository(SonarContext dbContext) : GenericRepository<Settings>(dbContext), ISettingsRepository
{
    public Task<Settings> CreateDefaultAsync()
    {
        throw new NotImplementedException();
    }
    public override async Task<Settings?> GetByIdAsync(int? id) {
        return RepositoryIncludeExtensions.SnInclude(RepositoryIncludeExtensions.SnInclude(context.Set<Settings>(), s => s.UserPrivacy), s => s.PreferredPlaybackQuality).FirstOrDefault(s=>s.Id == id);
    }
}
