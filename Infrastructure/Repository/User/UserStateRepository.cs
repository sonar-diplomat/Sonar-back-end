using Application.Abstractions.Interfaces.Repository.User;
using Entities.Models.UserCore;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.User
{
    public class UserStateRepository : GenericRepository<UserState>, IUserStateRepository
    {
        public UserStateRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
