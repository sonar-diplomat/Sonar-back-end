using Application.Abstractions.Interfaces.Repository.Client;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.Client
{
    public class UserPrivacySettingsRepository : GenericRepository<Entities.Models.UserPrivacySettings>, IUserPrivacySettingsRepository
    {
        public UserPrivacySettingsRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
