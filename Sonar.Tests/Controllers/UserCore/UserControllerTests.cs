using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.Abstractions.Interfaces.Services.UserCore;
using Application.Abstractions.Interfaces.Services.Utilities;
using Application.DTOs.User;
using Application.Response;
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
using Sonar.Tests.Controllers.Chat;
using System.Security.Claims;
using Xunit;

namespace Sonar.Tests.Controllers.UserCore;

public class UserControllerTests : ChatControllerTestsBase, IDisposable
{
    private readonly UserManager<User> _userManager;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly UserController _controller;
    private readonly User _testUser;
    private readonly ClaimsPrincipal _claimsPrincipal;
    private readonly SonarContext _context;

    public UserControllerTests()
    {
        _context = CreateInMemoryContext();
        _userManager = CreateUserManager(_context);
        _testUser = CreateTestUser(1);
        _context.Users.Add(_testUser);
        _context.SaveChanges();
        _claimsPrincipal = CreateClaimsPrincipal(_testUser);
        
        var visibilityStateServiceMock = new Mock<IVisibilityStateService>();
        var inventoryServiceMock = new Mock<IInventoryService>();
        var accessFeatureServiceMock = new Mock<IAccessFeatureService>();
        var settingsServiceMock = new Mock<ISettingsService>();
        var stateServiceMock = new Mock<IUserStateService>();
        var imageFileServiceMock = new Mock<IImageFileService>();
        var libraryServiceMock = new Mock<ILibraryService>();
        var userFollowServiceMock = new Mock<IUserFollowService>();
        var shareServiceMock = new Mock<IShareService>();
        var playlistRepositoryMock = new Mock<Application.Abstractions.Interfaces.Repository.Music.IPlaylistRepository>();
        
        _userServiceMock = new Mock<IUserService>();
        _controller = new UserController(
            _userServiceMock.Object,
            _userManager,
            shareServiceMock.Object,
            userFollowServiceMock.Object,
            playlistRepositoryMock.Object
        );
        
        var httpContext = new DefaultHttpContext
        {
            User = _claimsPrincipal
        };
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    #region GetFriends Tests

    [Fact]
    public async Task GetFriends_ValidUser_ReturnsOk()
    {
        var friends = new List<User>
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
        
        _userServiceMock
            .Setup(x => x.GetFriendsAsync(_testUser.Id))
            .ReturnsAsync(friends);

        var exception = await Assert.ThrowsAsync<OkResponse<IEnumerable<UserFriendDTO>>>(async () => await _controller.GetFriends());
        
        _userServiceMock.Verify(x => x.GetFriendsAsync(_testUser.Id), Times.Once);
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
    public async Task GetFriends_NoFriends_ReturnsEmptyList()
    {
        var friends = new List<User>();
        
        _userServiceMock
            .Setup(x => x.GetFriendsAsync(_testUser.Id))
            .ReturnsAsync(friends);

        var exception = await Assert.ThrowsAsync<OkResponse<IEnumerable<UserFriendDTO>>>(async () => await _controller.GetFriends());
        
        _userServiceMock.Verify(x => x.GetFriendsAsync(_testUser.Id), Times.Once);
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(200);
        var serializableProps = exception.GetSerializableProperties();
        serializableProps.Should().ContainKey("data");
        var dataValue = serializableProps["data"];
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

