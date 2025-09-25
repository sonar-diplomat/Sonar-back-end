using Application.Abstractions.Interfaces.Repository.User;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.User
{
    public class UserStatusRepository : GenericRepository<Entities.Models.UserStatus>, IUserStatusRepository
    {
        public UserStatusRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
