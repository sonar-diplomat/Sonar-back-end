using Application.DTOs.User;
using Entities.Models.UserCore;

namespace Application.Abstractions.Interfaces.Services.UserCore;

public interface IUserFollowService : IGenericService<UserFollow>
{
    Task<UserFollow> FollowAsync(int followerId, int followingId);
    Task UnfollowAsync(int followerId, int followingId);
    Task<IEnumerable<UserFollowerDTO>> GetFollowersAsync(int userId);
    Task<IEnumerable<UserFollowingDTO>> GetFollowingAsync(int userId);
    Task<bool> IsFollowingAsync(int followerId, int followingId);
    Task<IEnumerable<User>> GetMutualFollowsAsync(int userId);
}

