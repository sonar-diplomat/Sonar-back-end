using Application.Abstractions.Interfaces.Repository.UserCore;
using Entities.Models.UserCore;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.UserCore
{
    public class UserStatusRepository : GenericRepository<UserStatus>, IUserStatusRepository
    {
        public UserStatusRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
