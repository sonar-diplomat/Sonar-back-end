using Application.Abstractions.Interfaces.Repository.User;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.User
{
    public class UserStateRepository : GenericRepository<Entities.Models.UserState>, IUserStateRepository
    {
        public UserStateRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
