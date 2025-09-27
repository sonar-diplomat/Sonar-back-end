using Application.Abstractions.Interfaces.Repository.UserCore;
using Entities.Models.UserCore;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.UserCore
{
    public class UserPrivacyGroupRepository : GenericRepository<UserPrivacyGroup>, IUserPrivacyGroupRepository
    {
        public UserPrivacyGroupRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
