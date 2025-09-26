using Application.Abstractions.Interfaces.Repository.User;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.User
{
    public class UserRepository : GenericRepository<Entities.Models.UserCore.User>, IUserRepository
    {
        public UserRepository(SonarContext dbContext) : base(dbContext)
        {
        }
    }
}
