using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.DTOs.User;
using Application.Response;
using Entities.Models.UserCore;
using FluentAssertions;
using Moq;
using Xunit;
using Application.Services.UserCore;

namespace Sonar.Tests.Services.UserCore;

public class UserFollowServiceTests
{
    private readonly Mock<IUserFollowRepository> _repositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserFollowService _service;
    private readonly User _follower;
    private readonly User _following;

    public UserFollowServiceTests()
    {
        _repositoryMock = new Mock<IUserFollowRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _service = new UserFollowService(_repositoryMock.Object, _userRepositoryMock.Object);

        _follower = new User
        {
            Id = 1,
            UserName = "follower",
            Email = "follower@example.com"
        };

        _following = new User
        {
            Id = 2,
            UserName = "following",
            Email = "following@example.com"
        };
    }

    #region FollowAsync Tests

    [Fact]
    public async Task FollowAsync_ValidUsers_CreatesFollow()
    {
        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(_follower.Id))
            .ReturnsAsync(_follower);
        
        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(_following.Id))
            .ReturnsAsync(_following);
        
        _repositoryMock
            .Setup(x => x.GetByFollowerAndFollowingAsync(_follower.Id, _following.Id))
            .ReturnsAsync((Entities.Models.UserCore.UserFollow?)null);
        
        Entities.Models.UserCore.UserFollow? createdFollow = null;
        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Entities.Models.UserCore.UserFollow>()))
            .Callback<Entities.Models.UserCore.UserFollow>(uf => createdFollow = uf)
            .ReturnsAsync((Entities.Models.UserCore.UserFollow uf) => uf);

        var result = await _service.FollowAsync(_follower.Id, _following.Id);

        _userRepositoryMock.Verify(x => x.GetByIdAsync(_follower.Id), Times.Once);
        _userRepositoryMock.Verify(x => x.GetByIdAsync(_following.Id), Times.Once);
        _repositoryMock.Verify(x => x.GetByFollowerAndFollowingAsync(_follower.Id, _following.Id), Times.Once);
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Entities.Models.UserCore.UserFollow>()), Times.Once);
        
        result.Should().NotBeNull();
        result.FollowerId.Should().Be(_follower.Id);
        result.FollowingId.Should().Be(_following.Id);
        createdFollow.Should().NotBeNull();
        createdFollow!.FollowerId.Should().Be(_follower.Id);
        createdFollow.FollowingId.Should().Be(_following.Id);
    }

    [Fact]
    public async Task FollowAsync_CannotFollowSelf_ThrowsBadRequest()
    {
        var exception = await Assert.ThrowsAsync<BadRequestResponse>(async () => 
            await _service.FollowAsync(_follower.Id, _follower.Id));
        
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(400);
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Entities.Models.UserCore.UserFollow>()), Times.Never);
    }

    [Fact]
    public async Task FollowAsync_FollowerNotFound_ThrowsNotFound()
    {
        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(_follower.Id))
            .ReturnsAsync((User?)null);

        var exception = await Assert.ThrowsAsync<NotFoundResponse>(async () => 
            await _service.FollowAsync(_follower.Id, _following.Id));
        
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(404);
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Entities.Models.UserCore.UserFollow>()), Times.Never);
    }

    [Fact]
    public async Task FollowAsync_FollowingNotFound_ThrowsNotFound()
    {
        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(_follower.Id))
            .ReturnsAsync(_follower);
        
        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(_following.Id))
            .ReturnsAsync((User?)null);

        var exception = await Assert.ThrowsAsync<NotFoundResponse>(async () => 
            await _service.FollowAsync(_follower.Id, _following.Id));
        
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(404);
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Entities.Models.UserCore.UserFollow>()), Times.Never);
    }

    [Fact]
    public async Task FollowAsync_AlreadyFollowing_ThrowsBadRequest()
    {
        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(_follower.Id))
            .ReturnsAsync(_follower);
        
        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(_following.Id))
            .ReturnsAsync(_following);
        
        var existingFollow = new Entities.Models.UserCore.UserFollow
        {
            Id = 1,
            FollowerId = _follower.Id,
            FollowingId = _following.Id,
            FollowedAt = DateTime.UtcNow.AddDays(-1)
        };
        
        _repositoryMock
            .Setup(x => x.GetByFollowerAndFollowingAsync(_follower.Id, _following.Id))
            .ReturnsAsync(existingFollow);

        var exception = await Assert.ThrowsAsync<BadRequestResponse>(async () => 
            await _service.FollowAsync(_follower.Id, _following.Id));
        
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(400);
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Entities.Models.UserCore.UserFollow>()), Times.Never);
    }

    #endregion

    #region UnfollowAsync Tests

    [Fact]
    public async Task UnfollowAsync_ValidFollow_DeletesFollow()
    {
        var userFollow = new Entities.Models.UserCore.UserFollow
        {
            Id = 1,
            FollowerId = _follower.Id,
            FollowingId = _following.Id,
            FollowedAt = DateTime.UtcNow.AddDays(-1)
        };
        
        _repositoryMock
            .Setup(x => x.GetByFollowerAndFollowingAsync(_follower.Id, _following.Id))
            .ReturnsAsync(userFollow);
        
        _repositoryMock
            .Setup(x => x.RemoveAsync(userFollow))
            .Returns(Task.CompletedTask);

        await _service.UnfollowAsync(_follower.Id, _following.Id);

        _repositoryMock.Verify(x => x.GetByFollowerAndFollowingAsync(_follower.Id, _following.Id), Times.Once);
        _repositoryMock.Verify(x => x.RemoveAsync(userFollow), Times.Once);
    }

    [Fact]
    public async Task UnfollowAsync_NotFollowing_ThrowsNotFound()
    {
        _repositoryMock
            .Setup(x => x.GetByFollowerAndFollowingAsync(_follower.Id, _following.Id))
            .ReturnsAsync((Entities.Models.UserCore.UserFollow?)null);

        var exception = await Assert.ThrowsAsync<NotFoundResponse>(async () => 
            await _service.UnfollowAsync(_follower.Id, _following.Id));
        
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(404);
        _repositoryMock.Verify(x => x.RemoveAsync(It.IsAny<Entities.Models.UserCore.UserFollow>()), Times.Never);
    }

    #endregion

    #region GetFollowersAsync Tests

    [Fact]
    public async Task GetFollowersAsync_ValidUser_ReturnsFollowers()
    {
        var userId = 2;
        var follower1 = new User { Id = 1, UserName = "follower1", PublicIdentifier = "follower1", AvatarImageId = 1 };
        var follower2 = new User { Id = 3, UserName = "follower2", PublicIdentifier = "follower2", AvatarImageId = 2 };
        
        var follows = new List<Entities.Models.UserCore.UserFollow>
        {
            new()
            {
                Id = 1,
                FollowerId = 1,
                FollowingId = userId,
                FollowedAt = DateTime.UtcNow.AddDays(-1),
                Follower = follower1
            },
            new()
            {
                Id = 2,
                FollowerId = 3,
                FollowingId = userId,
                FollowedAt = DateTime.UtcNow.AddDays(-2),
                Follower = follower2
            }
        };
        
        _repositoryMock
            .Setup(x => x.GetFollowersAsync(userId))
            .ReturnsAsync(follows);

        // Act
        var result = await _service.GetFollowersAsync(userId);

        // Assert
        _repositoryMock.Verify(x => x.GetFollowersAsync(userId), Times.Once);
        result.Should().HaveCount(2);
        result.First().Id.Should().Be(1);
        result.First().UserName.Should().Be("follower1");
        result.Last().Id.Should().Be(3);
        result.Last().UserName.Should().Be("follower2");
    }

    [Fact]
    public async Task GetFollowersAsync_NoFollowers_ReturnsEmpty()
    {
        var userId = 2;
        var follows = new List<Entities.Models.UserCore.UserFollow>();
        
        _repositoryMock
            .Setup(x => x.GetFollowersAsync(userId))
            .ReturnsAsync(follows);

        // Act
        var result = await _service.GetFollowersAsync(userId);

        // Assert
        _repositoryMock.Verify(x => x.GetFollowersAsync(userId), Times.Once);
        result.Should().BeEmpty();
    }

    #endregion

    #region GetFollowingAsync Tests

    [Fact]
    public async Task GetFollowingAsync_ValidUser_ReturnsFollowing()
    {
        var userId = 1;
        var following1 = new User { Id = 2, UserName = "following1", PublicIdentifier = "following1", AvatarImageId = 1 };
        var following2 = new User { Id = 3, UserName = "following2", PublicIdentifier = "following2", AvatarImageId = 2 };
        
        var follows = new List<Entities.Models.UserCore.UserFollow>
        {
            new()
            {
                Id = 1,
                FollowerId = userId,
                FollowingId = 2,
                FollowedAt = DateTime.UtcNow.AddDays(-1),
                Following = following1
            },
            new()
            {
                Id = 2,
                FollowerId = userId,
                FollowingId = 3,
                FollowedAt = DateTime.UtcNow.AddDays(-2),
                Following = following2
            }
        };
        
        _repositoryMock
            .Setup(x => x.GetFollowingAsync(userId))
            .ReturnsAsync(follows);

        // Act
        var result = await _service.GetFollowingAsync(userId);

        // Assert
        _repositoryMock.Verify(x => x.GetFollowingAsync(userId), Times.Once);
        result.Should().HaveCount(2);
        result.First().Id.Should().Be(2);
        result.First().UserName.Should().Be("following1");
        result.Last().Id.Should().Be(3);
        result.Last().UserName.Should().Be("following2");
    }

    [Fact]
    public async Task GetFollowingAsync_NoFollowing_ReturnsEmpty()
    {
        var userId = 1;
        var follows = new List<Entities.Models.UserCore.UserFollow>();
        
        _repositoryMock
            .Setup(x => x.GetFollowingAsync(userId))
            .ReturnsAsync(follows);

        // Act
        var result = await _service.GetFollowingAsync(userId);

        // Assert
        _repositoryMock.Verify(x => x.GetFollowingAsync(userId), Times.Once);
        result.Should().BeEmpty();
    }

    #endregion

    #region IsFollowingAsync Tests

    [Fact]
    public async Task IsFollowingAsync_IsFollowing_ReturnsTrue()
    {
        var followerId = 1;
        var followingId = 2;
        var userFollow = new Entities.Models.UserCore.UserFollow
        {
            Id = 1,
            FollowerId = followerId,
            FollowingId = followingId,
            FollowedAt = DateTime.UtcNow
        };
        
        _repositoryMock
            .Setup(x => x.GetByFollowerAndFollowingAsync(followerId, followingId))
            .ReturnsAsync(userFollow);

        var result = await _service.IsFollowingAsync(followerId, followingId);

        _repositoryMock.Verify(x => x.GetByFollowerAndFollowingAsync(followerId, followingId), Times.Once);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsFollowingAsync_NotFollowing_ReturnsFalse()
    {
        var followerId = 1;
        var followingId = 2;
        
        _repositoryMock
            .Setup(x => x.GetByFollowerAndFollowingAsync(followerId, followingId))
            .ReturnsAsync((Entities.Models.UserCore.UserFollow?)null);

        var result = await _service.IsFollowingAsync(followerId, followingId);

        _repositoryMock.Verify(x => x.GetByFollowerAndFollowingAsync(followerId, followingId), Times.Once);
        result.Should().BeFalse();
    }

    #endregion

    #region GetMutualFollowsAsync Tests

    [Fact]
    public async Task GetMutualFollowsAsync_HasMutualFollows_ReturnsMutualFollows()
    {
        var userId = 1;
        var mutualUser1 = new User { Id = 2, UserName = "mutual1", PublicIdentifier = "mutual1", AvatarImageId = 1 };
        var mutualUser2 = new User { Id = 3, UserName = "mutual2", PublicIdentifier = "mutual2", AvatarImageId = 2 };
        var nonMutualUser = new User { Id = 4, UserName = "nonmutual", PublicIdentifier = "nonmutual", AvatarImageId = 3 };
        
        // Users who follow userId
        var followers = new List<Entities.Models.UserCore.UserFollow>
        {
            new() { Id = 1, FollowerId = 2, FollowingId = userId, Follower = mutualUser1 },
            new() { Id = 2, FollowerId = 3, FollowingId = userId, Follower = mutualUser2 },
            new() { Id = 3, FollowerId = 4, FollowingId = userId, Follower = nonMutualUser }
        };
        
        // Users that userId follows
        var following = new List<Entities.Models.UserCore.UserFollow>
        {
            new() { Id = 4, FollowerId = userId, FollowingId = 2, Following = mutualUser1 },
            new() { Id = 5, FollowerId = userId, FollowingId = 3, Following = mutualUser2 }
            // userId does not follow nonMutualUser (id=4)
        };
        
        _repositoryMock
            .Setup(x => x.GetFollowersAsync(userId))
            .ReturnsAsync(followers);
        
        _repositoryMock
            .Setup(x => x.GetFollowingAsync(userId))
            .ReturnsAsync(following);

        var result = await _service.GetMutualFollowsAsync(userId);

        _repositoryMock.Verify(x => x.GetFollowersAsync(userId), Times.Once);
        _repositoryMock.Verify(x => x.GetFollowingAsync(userId), Times.Once);
        result.Should().HaveCount(2);
        result.Select(u => u.Id).Should().Contain(new[] { 2, 3 });
        result.Select(u => u.Id).Should().NotContain(4);
    }

    [Fact]
    public async Task GetMutualFollowsAsync_NoMutualFollows_ReturnsEmpty()
    {
        var userId = 1;
        var followers = new List<Entities.Models.UserCore.UserFollow>
        {
            new() { Id = 1, FollowerId = 2, FollowingId = userId }
        };
        
        var following = new List<Entities.Models.UserCore.UserFollow>
        {
            new() { Id = 2, FollowerId = userId, FollowingId = 3 }
        };
        
        _repositoryMock
            .Setup(x => x.GetFollowersAsync(userId))
            .ReturnsAsync(followers);
        
        _repositoryMock
            .Setup(x => x.GetFollowingAsync(userId))
            .ReturnsAsync(following);

        var result = await _service.GetMutualFollowsAsync(userId);

        _repositoryMock.Verify(x => x.GetFollowersAsync(userId), Times.Once);
        _repositoryMock.Verify(x => x.GetFollowingAsync(userId), Times.Once);
        result.Should().BeEmpty();
    }

    #endregion
}

