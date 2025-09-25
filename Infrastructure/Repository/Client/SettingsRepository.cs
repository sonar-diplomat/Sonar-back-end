using Application.Abstractions.Interfaces.Repository.Client;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Client
{
    public class SettingsRepository : GenericRepository<Entities.Models.Settings>, ISettingsRepository
    {
        public SettingsRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
