using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.Abstractions.Interfaces.Services.UserCore;
using Application.DTOs.User;
using Application.Response;
using Entities.Models.UserCore;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.UserCore;

public class UserFollowService(
    IUserFollowRepository repository,
    IUserRepository userRepository
) : GenericService<UserFollow>(repository), IUserFollowService
{
    public async Task<UserFollow> FollowAsync(int followerId, int followingId)
    {
        if (followerId == followingId)
            throw ResponseFactory.Create<BadRequestResponse>(["You cannot follow yourself."]);

        User? follower = await userRepository.GetByIdAsync(followerId);
        if (follower == null)
            throw ResponseFactory.Create<NotFoundResponse>(["Follower user not found."]);

        User? following = await userRepository.GetByIdAsync(followingId);
        if (following == null)
            throw ResponseFactory.Create<NotFoundResponse>(["Following user not found."]);

        UserFollow? existingFollow = await repository.GetByFollowerAndFollowingAsync(followerId, followingId);
        if (existingFollow != null)
            throw ResponseFactory.Create<BadRequestResponse>(["You are already following this user."]);

        UserFollow userFollow = new()
        {
            FollowerId = followerId,
            FollowingId = followingId,
            FollowedAt = DateTime.UtcNow
        };

        return await CreateAsync(userFollow);
    }

    public async Task UnfollowAsync(int followerId, int followingId)
    {
        UserFollow? userFollow = await repository.GetByFollowerAndFollowingAsync(followerId, followingId);
        if (userFollow == null)
            throw ResponseFactory.Create<NotFoundResponse>(["You are not following this user."]);

        await DeleteAsync(userFollow);
    }

    public async Task<IEnumerable<UserFollowerDTO>> GetFollowersAsync(int userId)
    {
        IEnumerable<UserFollow> follows = await repository.GetFollowersAsync(userId);
        return follows.Select(f => new UserFollowerDTO
        {
            Id = f.Follower.Id,
            UserName = f.Follower.UserName ?? string.Empty,
            PublicIdentifier = f.Follower.PublicIdentifier,
            AvatarImageId = f.Follower.AvatarImageId,
            FollowedAt = f.FollowedAt
        });
    }

    public async Task<IEnumerable<UserFollowingDTO>> GetFollowingAsync(int userId)
    {
        IEnumerable<UserFollow> follows = await repository.GetFollowingAsync(userId);
        return follows.Select(f => new UserFollowingDTO
        {
            Id = f.Following.Id,
            UserName = f.Following.UserName ?? string.Empty,
            PublicIdentifier = f.Following.PublicIdentifier,
            AvatarImageId = f.Following.AvatarImageId,
            FollowedAt = f.FollowedAt
        });
    }

    public async Task<bool> IsFollowingAsync(int followerId, int followingId)
    {
        UserFollow? userFollow = await repository.GetByFollowerAndFollowingAsync(followerId, followingId);
        return userFollow != null;
    }

    public async Task<IEnumerable<User>> GetMutualFollowsAsync(int userId)
    {
        var followers = await repository.GetFollowersAsync(userId);
        var following = await repository.GetFollowingAsync(userId);

        var followerIds = followers.Select(f => f.FollowerId).ToHashSet();
        var mutualFollows = following
            .Where(f => followerIds.Contains(f.FollowingId))
            .Select(f => f.Following)
            .ToList();

        return mutualFollows;
    }
}

