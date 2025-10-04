using Application.Abstractions.Interfaces.Repository.Client;
using Entities.Models.ClientSettings;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Client
{
    public class SettingsRepository : GenericRepository<Settings>, ISettingsRepository
    {
        public SettingsRepository(SonarContext dbContext) : base(dbContext)
        {
        }

        public Task<Settings> CreateDefaultAsync()
        {
            throw new NotImplementedException();
        }
    }
}
