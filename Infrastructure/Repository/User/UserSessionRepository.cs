using Application.Abstractions.Interfaces.Repository.User;
using Entities.Models.User;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.User
{
    public class UserSessionRepository : GenericRepository<UserSession>, IUserSessionRepository
    {
        public UserSessionRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
