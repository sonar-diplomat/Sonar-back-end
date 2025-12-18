using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.Abstractions.Interfaces.Services.UserCore;
using Application.Services.UserCore;
using Entities.Models.UserCore;
using FluentAssertions;
using Moq;
using Xunit;

namespace Sonar.Tests.Services.UserCore;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUserFollowService> _userFollowServiceMock;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userFollowServiceMock = new Mock<IUserFollowService>();
        
        var visibilityStateServiceMock = new Mock<IVisibilityStateService>();
        var inventoryServiceMock = new Mock<IInventoryService>();
        var accessFeatureServiceMock = new Mock<IAccessFeatureService>();
        var settingsServiceMock = new Mock<ISettingsService>();
        var stateServiceMock = new Mock<IUserStateService>();
        var imageFileServiceMock = new Mock<IImageFileService>();
        var libraryServiceMock = new Mock<ILibraryService>();
        
        _service = new UserService(
            _userRepositoryMock.Object,
            visibilityStateServiceMock.Object,
            inventoryServiceMock.Object,
            accessFeatureServiceMock.Object,
            settingsServiceMock.Object,
            stateServiceMock.Object,
            imageFileServiceMock.Object,
            libraryServiceMock.Object,
            _userFollowServiceMock.Object
        );
    }

    #region GetFriendsAsync Tests

    [Fact]
    public async Task GetFriendsAsync_ValidUser_ReturnsMutualFollows()
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

        var result = await _service.GetFriendsAsync(userId);

        _userFollowServiceMock.Verify(x => x.GetMutualFollowsAsync(userId), Times.Once);
        result.Should().HaveCount(2);
        result.First().Id.Should().Be(2);
        result.First().UserName.Should().Be("friend1");
        result.Last().Id.Should().Be(3);
        result.Last().UserName.Should().Be("friend2");
    }

    [Fact]
    public async Task GetFriendsAsync_NoFriends_ReturnsEmpty()
    {
        var userId = 1;
        var mutualFollows = new List<User>();
        
        _userFollowServiceMock
            .Setup(x => x.GetMutualFollowsAsync(userId))
            .ReturnsAsync(mutualFollows);

        var result = await _service.GetFriendsAsync(userId);

        _userFollowServiceMock.Verify(x => x.GetMutualFollowsAsync(userId), Times.Once);
        result.Should().BeEmpty();
    }

    #endregion
}

