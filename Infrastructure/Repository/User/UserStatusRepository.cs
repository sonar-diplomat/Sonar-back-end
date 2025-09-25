using Application.Abstractions.Interfaces.Repository.User;
using Entities.Models.User;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.User
{
    public class UserStatusRepository : GenericRepository<UserStatus>, IUserStatusRepository
    {
        public UserStatusRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
