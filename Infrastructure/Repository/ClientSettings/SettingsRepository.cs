using Application.Abstractions.Interfaces.Repository.Client;
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
        return context.Set<Settings>().Include(s => s.UserPrivacy).Include(s => s.PreferredPlaybackQuality).FirstOrDefault(s=>s.Id == id);
    }
}
