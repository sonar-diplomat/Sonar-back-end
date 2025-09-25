using Application.Abstractions.Interfaces.Repository.User;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.User
{
    public class UserPrivacyGroupRepository : GenericRepository<Entities.Models.UserPrivacyGroup>, IUserPrivacyGroupRepository
    {
        public UserPrivacyGroupRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
