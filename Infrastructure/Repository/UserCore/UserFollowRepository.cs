using Application.Abstractions.Interfaces.Repository.UserCore;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Sonar.Infrastructure.Repository;

namespace Infrastructure.Repository.UserCore;

public class UserFollowRepository(SonarContext dbContext)
    : GenericRepository<Entities.Models.UserCore.UserFollow>(dbContext), IUserFollowRepository
{
    public async Task<Entities.Models.UserCore.UserFollow?> GetByFollowerAndFollowingAsync(int followerId, int followingId)
    {
        return await dbContext.UserFollows
            .FirstOrDefaultAsync(uf => uf.FollowerId == followerId && uf.FollowingId == followingId);
    }

    public async Task<IEnumerable<Entities.Models.UserCore.UserFollow>> GetFollowersAsync(int userId)
    {
        return await dbContext.UserFollows
            .Where(uf => uf.FollowingId == userId)
            .Include(uf => uf.Follower)
            .ToListAsync();
    }

    public async Task<IEnumerable<Entities.Models.UserCore.UserFollow>> GetFollowingAsync(int userId)
    {
        return await dbContext.UserFollows
            .Where(uf => uf.FollowerId == userId)
            .Include(uf => uf.Following)
            .ToListAsync();
    }
}

