using Application.Abstractions.Interfaces.Repository.UserCore;
using Entities.Models.UserCore;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.UserCore
{
    public class UserSessionRepository : GenericRepository<UserSession>, IUserSessionRepository
    {
        public UserSessionRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
