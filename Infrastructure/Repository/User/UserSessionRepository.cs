using Application.Abstractions.Interfaces.Repository.User;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.User
{
    public class UserSessionRepository : GenericRepository<Entities.Models.UserSession>, IUserSessionRepository
    {
        public UserSessionRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
