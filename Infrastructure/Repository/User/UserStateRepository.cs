using Application.Abstractions.Interfaces.Repository.UserCore;
using Entities.Models.UserCore;
using Infrastructure.Data;

namespace Sonar.Infrastructure.Repository.UserCore;

public class UserStateRepository : GenericRepository<UserState>, IUserStateRepository
{
    public UserStateRepository(SonarContext dbContext) : base(dbContext)
    {
    }
}
