using Application.Abstractions.Interfaces.Repository.UserCore;
using Entities.Models.UserCore;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Infrastructure.Repository.UserCore;

public class UserStateRepository(SonarContext dbContext) : GenericRepository<UserState>(dbContext), IUserStateRepository
{
    public async Task<UserState?> GetByUserIdAsync(int userId)
    {
        return await context.UserStates.Include(u => u.User)
            .Include(us => us.Queue)
            .FirstOrDefaultAsync(us => us.User.Id == userId);
    }
}