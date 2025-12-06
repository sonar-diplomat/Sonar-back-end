using Application.Abstractions.Interfaces.Services.UserCore;
using Application.DTOs.User;
using Application.Response;
using Entities.Models.Access;
using Entities.Models.UserCore;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Sonar.Controllers.UserCore;
using System.Security.Claims;
using Sonar.Tests.Controllers.Chat;
using Xunit;

namespace Sonar.Tests.Controllers.UserCore;

public class UserFollowControllerTests : ChatControllerTestsBase, IDisposable
{
    private readonly UserManager<User> _userManager;
    private readonly Mock<IUserFollowService> _userFollowServiceMock;
    private readonly UserFollowController _controller;
    private readonly User _testUser;
    private readonly ClaimsPrincipal _claimsPrincipal;
    private readonly SonarContext _context;

    public UserFollowControllerTests()
    {
        _context = CreateInMemoryContext();
        _userManager = CreateUserManager(_context);
        _testUser = CreateTestUser(1);
        _context.Users.Add(_testUser);
        _context.SaveChanges();
        _claimsPrincipal = CreateClaimsPrincipal(_testUser);
        _userFollowServiceMock = new Mock<IUserFollowService>();
        _controller = new UserFollowController(_userFollowServiceMock.Object, _userManager);
        
        var httpContext = new DefaultHttpContext
        {
            User = _claimsPrincipal
        };
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    #region FollowUser Tests

    [Fact]
    public async Task FollowUser_ValidUser_ReturnsOk()
    {
        var userIdToFollow = 2;
        var userFollow = new Entities.Models.UserCore.UserFollow
        {
            Id = 1,
            FollowerId = _testUser.Id,
            FollowingId = userIdToFollow,
            FollowedAt = DateTime.UtcNow
        };
        
        _userFollowServiceMock
            .Setup(x => x.FollowAsync(_testUser.Id, userIdToFollow))
            .ReturnsAsync(userFollow);

        var exception = await Assert.ThrowsAsync<OkResponse>(async () => await _controller.FollowUser(userIdToFollow));
        
        _userFollowServiceMock.Verify(x => x.FollowAsync(_testUser.Id, userIdToFollow), Times.Once);
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task FollowUser_CannotFollowSelf_ThrowsBadRequest()
    {
        var userIdToFollow = _testUser.Id;
        
        _userFollowServiceMock
            .Setup(x => x.FollowAsync(_testUser.Id, userIdToFollow))
            .ThrowsAsync(ResponseFactory.Create<BadRequestResponse>(["You cannot follow yourself."]));

        var exception = await Assert.ThrowsAsync<BadRequestResponse>(async () => await _controller.FollowUser(userIdToFollow));
        
        _userFollowServiceMock.Verify(x => x.FollowAsync(_testUser.Id, userIdToFollow), Times.Once);
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task FollowUser_AlreadyFollowing_ThrowsBadRequest()
    {
        var userIdToFollow = 2;
        
        _userFollowServiceMock
            .Setup(x => x.FollowAsync(_testUser.Id, userIdToFollow))
            .ThrowsAsync(ResponseFactory.Create<BadRequestResponse>(["You are already following this user."]));

        var exception = await Assert.ThrowsAsync<BadRequestResponse>(async () => await _controller.FollowUser(userIdToFollow));
        
        _userFollowServiceMock.Verify(x => x.FollowAsync(_testUser.Id, userIdToFollow), Times.Once);
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task FollowUser_UserNotFound_ThrowsNotFound()
    {
        var userIdToFollow = 999;
        
        _userFollowServiceMock
            .Setup(x => x.FollowAsync(_testUser.Id, userIdToFollow))
            .ThrowsAsync(ResponseFactory.Create<NotFoundResponse>(["Following user not found."]));

        var exception = await Assert.ThrowsAsync<NotFoundResponse>(async () => await _controller.FollowUser(userIdToFollow));
        
        _userFollowServiceMock.Verify(x => x.FollowAsync(_testUser.Id, userIdToFollow), Times.Once);
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(404);
    }

    #endregion

    #region UnfollowUser Tests

    [Fact]
    public async Task UnfollowUser_ValidUser_ReturnsOk()
    {
        var userIdToUnfollow = 2;
        
        _userFollowServiceMock
            .Setup(x => x.UnfollowAsync(_testUser.Id, userIdToUnfollow))
            .Returns(Task.CompletedTask);

        var exception = await Assert.ThrowsAsync<OkResponse>(async () => await _controller.UnfollowUser(userIdToUnfollow));
        
        _userFollowServiceMock.Verify(x => x.UnfollowAsync(_testUser.Id, userIdToUnfollow), Times.Once);
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task UnfollowUser_NotFollowing_ThrowsNotFound()
    {
        var userIdToUnfollow = 2;
        
        _userFollowServiceMock
            .Setup(x => x.UnfollowAsync(_testUser.Id, userIdToUnfollow))
            .ThrowsAsync(ResponseFactory.Create<NotFoundResponse>(["You are not following this user."]));

        var exception = await Assert.ThrowsAsync<NotFoundResponse>(async () => await _controller.UnfollowUser(userIdToUnfollow));
        
        _userFollowServiceMock.Verify(x => x.UnfollowAsync(_testUser.Id, userIdToUnfollow), Times.Once);
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(404);
    }

    #endregion

    #region GetFollowers Tests

    [Fact]
    public async Task GetFollowers_ValidUser_ReturnsOk()
    {
        var userId = 2;
        var followers = new List<UserFollowerDTO>
        {
            new()
            {
                Id = 1,
                UserName = "follower1",
                PublicIdentifier = "follower1",
                AvatarImageId = 1,
                FollowedAt = DateTime.UtcNow.AddDays(-1)
            },
            new()
            {
                Id = 3,
                UserName = "follower2",
                PublicIdentifier = "follower2",
                AvatarImageId = 2,
                FollowedAt = DateTime.UtcNow.AddDays(-2)
            }
        };
        
        _userFollowServiceMock
            .Setup(x => x.GetFollowersAsync(userId))
            .ReturnsAsync(followers);

        var exception = await Assert.ThrowsAsync<OkResponse<IEnumerable<UserFollowerDTO>>>(async () => await _controller.GetFollowers(userId));
        
        _userFollowServiceMock.Verify(x => x.GetFollowersAsync(userId), Times.Once);
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(200);
        var serializableProps = exception.GetSerializableProperties();
        serializableProps.Should().ContainKey("data");
        var data = serializableProps["data"] as IEnumerable<UserFollowerDTO>;
        data.Should().NotBeNull();
        data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetFollowers_NoFollowers_ReturnsEmptyList()
    {
        var userId = 2;
        var followers = new List<UserFollowerDTO>();
        
        _userFollowServiceMock
            .Setup(x => x.GetFollowersAsync(userId))
            .ReturnsAsync(followers);

        var exception = await Assert.ThrowsAsync<OkResponse<IEnumerable<UserFollowerDTO>>>(async () => await _controller.GetFollowers(userId));
        
        _userFollowServiceMock.Verify(x => x.GetFollowersAsync(userId), Times.Once);
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(200);
        var serializableProps = exception.GetSerializableProperties();
        serializableProps.Should().ContainKey("data");
        var dataValue = serializableProps["data"];
        // Для пустых списков GetSerializableProperties возвращает строку "empty list"
        if (dataValue is string emptyListString)
        {
            emptyListString.Should().Be("empty list");
        }
        else
        {
            var data = dataValue as IEnumerable<UserFollowerDTO>;
            data.Should().NotBeNull();
            data.Should().BeEmpty();
        }
    }

    #endregion

    #region GetFollowing Tests

    [Fact]
    public async Task GetFollowing_ValidUser_ReturnsOk()
    {
        var userId = 1;
        var following = new List<UserFollowingDTO>
        {
            new()
            {
                Id = 2,
                UserName = "following1",
                PublicIdentifier = "following1",
                AvatarImageId = 1,
                FollowedAt = DateTime.UtcNow.AddDays(-1)
            },
            new()
            {
                Id = 3,
                UserName = "following2",
                PublicIdentifier = "following2",
                AvatarImageId = 2,
                FollowedAt = DateTime.UtcNow.AddDays(-2)
            }
        };
        
        _userFollowServiceMock
            .Setup(x => x.GetFollowingAsync(userId))
            .ReturnsAsync(following);

        var exception = await Assert.ThrowsAsync<OkResponse<IEnumerable<UserFollowingDTO>>>(async () => await _controller.GetFollowing(userId));
        
        _userFollowServiceMock.Verify(x => x.GetFollowingAsync(userId), Times.Once);
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(200);
        var serializableProps = exception.GetSerializableProperties();
        serializableProps.Should().ContainKey("data");
        var data = serializableProps["data"] as IEnumerable<UserFollowingDTO>;
        data.Should().NotBeNull();
        data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetFollowing_NoFollowing_ReturnsEmptyList()
    {
        var userId = 1;
        var following = new List<UserFollowingDTO>();
        
        _userFollowServiceMock
            .Setup(x => x.GetFollowingAsync(userId))
            .ReturnsAsync(following);

        var exception = await Assert.ThrowsAsync<OkResponse<IEnumerable<UserFollowingDTO>>>(async () => await _controller.GetFollowing(userId));
        
        _userFollowServiceMock.Verify(x => x.GetFollowingAsync(userId), Times.Once);
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(200);
        var serializableProps = exception.GetSerializableProperties();
        serializableProps.Should().ContainKey("data");
        var dataValue = serializableProps["data"];
        // Для пустых списков GetSerializableProperties возвращает строку "empty list"
        if (dataValue is string emptyListString)
        {
            emptyListString.Should().Be("empty list");
        }
        else
        {
            var data = dataValue as IEnumerable<UserFollowingDTO>;
            data.Should().NotBeNull();
            data.Should().BeEmpty();
        }
    }

    #endregion

    #region GetMutualFollows Tests

    [Fact]
    public async Task GetMutualFollows_ValidUser_ReturnsOk()
    {
        var userId = 1;
        var mutualFollows = new List<User>
        {
            new()
            {
                Id = 2,
                UserName = "friend1",
                PublicIdentifier = "friend1",
                AvatarImageId = 1
            },
            new()
            {
                Id = 3,
                UserName = "friend2",
                PublicIdentifier = "friend2",
                AvatarImageId = 2
            }
        };
        
        _userFollowServiceMock
            .Setup(x => x.GetMutualFollowsAsync(userId))
            .ReturnsAsync(mutualFollows);

        var exception = await Assert.ThrowsAsync<OkResponse<IEnumerable<UserFriendDTO>>>(async () => await _controller.GetMutualFollows(userId));
        
        _userFollowServiceMock.Verify(x => x.GetMutualFollowsAsync(userId), Times.Once);
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(200);
        var serializableProps = exception.GetSerializableProperties();
        serializableProps.Should().ContainKey("data");
        var data = serializableProps["data"] as IEnumerable<UserFriendDTO>;
        data.Should().NotBeNull();
        data.Should().HaveCount(2);
        var dataList = data!.ToList();
        dataList[0].Id.Should().Be(2);
        dataList[0].UserName.Should().Be("friend1");
    }

    [Fact]
    public async Task GetMutualFollows_NoMutualFollows_ReturnsEmptyList()
    {
        var userId = 1;
        var mutualFollows = new List<User>();
        
        _userFollowServiceMock
            .Setup(x => x.GetMutualFollowsAsync(userId))
            .ReturnsAsync(mutualFollows);

        var exception = await Assert.ThrowsAsync<OkResponse<IEnumerable<UserFriendDTO>>>(async () => await _controller.GetMutualFollows(userId));
        
        _userFollowServiceMock.Verify(x => x.GetMutualFollowsAsync(userId), Times.Once);
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(200);
        var serializableProps = exception.GetSerializableProperties();
        serializableProps.Should().ContainKey("data");
        var dataValue = serializableProps["data"];
        // Для пустых списков GetSerializableProperties возвращает строку "empty list"
        if (dataValue is string emptyListString)
        {
            emptyListString.Should().Be("empty list");
        }
        else
        {
            var data = dataValue as IEnumerable<UserFriendDTO>;
            data.Should().NotBeNull();
            data.Should().BeEmpty();
        }
    }

    #endregion

    public void Dispose()
    {
        _context?.Dispose();
        _userManager?.Dispose();
    }
}

