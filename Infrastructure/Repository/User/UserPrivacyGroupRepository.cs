using Application.Abstractions.Interfaces.Repository.User;
using Entities.Models.User;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.User
{
    public class UserPrivacyGroupRepository : GenericRepository<UserPrivacyGroup>, IUserPrivacyGroupRepository
    {
        public UserPrivacyGroupRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
