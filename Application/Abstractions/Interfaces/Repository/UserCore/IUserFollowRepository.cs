using Entities.Models.UserCore;

namespace Application.Abstractions.Interfaces.Repository.UserCore;

public interface IUserFollowRepository : IGenericRepository<UserFollow>
{
    Task<UserFollow?> GetByFollowerAndFollowingAsync(int followerId, int followingId);
    Task<IEnumerable<UserFollow>> GetFollowersAsync(int userId);
    Task<IEnumerable<UserFollow>> GetFollowingAsync(int userId);
}

