using Application.Abstractions.Interfaces.Repository.Client;
using Entities.Models.ClientSettings;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Client
{
    public class UserPrivacySettingsRepository : GenericRepository<UserPrivacySettings>, IUserPrivacySettingsRepository
    {
        public UserPrivacySettingsRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
