using Entities.Models.UserCore;
using FluentAssertions;
using Infrastructure.Data;
using Infrastructure.Repository.UserCore;
using Microsoft.EntityFrameworkCore;
using Sonar.Tests.Repositories.UserCore;
using Xunit;

namespace Sonar.Tests.Repositories.UserCore;

public class UserFollowRepositoryTests : DatabaseTestBase
{
    private UserFollowRepository? _repository;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        
        if (DbContext != null)
        {
            _repository = new UserFollowRepository(DbContext);
        }
    }

    #region GetByFollowerAndFollowingAsync Tests

    [Fact]
    public async Task GetByFollowerAndFollowingAsync_ExistingFollow_ReturnsFollow()
    {
        // Arrange
        if (DbContext == null || _repository == null) return;

        await CleanDatabaseAsync();

        // Create dependencies for users
        await CreateUserDependenciesAsync(1);
        await CreateUserDependenciesAsync(2);

        var follower = CreateTestUser(1, "follower", "follower@test.com");
        var following = CreateTestUser(2, "following", "following@test.com");

        DbContext.Users.AddRange(follower, following);
        await DbContext.SaveChangesAsync(); // Save users first

        var userFollow = new UserFollow
        {
            FollowerId = 1,
            FollowingId = 2,
            FollowedAt = DateTime.UtcNow
        };

        DbContext.UserFollows.Add(userFollow);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByFollowerAndFollowingAsync(1, 2);

        // Assert
        result.Should().NotBeNull();
        result!.FollowerId.Should().Be(1);
        result.FollowingId.Should().Be(2);
    }

    [Fact]
    public async Task GetByFollowerAndFollowingAsync_NonExistentFollow_ReturnsNull()
    {
        // Arrange
        if (DbContext == null || _repository == null) return;

        await CleanDatabaseAsync();

        // Act
        var result = await _repository.GetByFollowerAndFollowingAsync(1, 2);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetFollowersAsync Tests

    [Fact]
    public async Task GetFollowersAsync_WithFollowers_ReturnsFollowers()
    {
        // Arrange
        if (DbContext == null || _repository == null) return;

        await CleanDatabaseAsync();

        // Create dependencies for users
        await CreateUserDependenciesAsync(1);
        await CreateUserDependenciesAsync(2);
        await CreateUserDependenciesAsync(3);

        var user1 = CreateTestUser(1, "user1", "user1@test.com");
        var user2 = CreateTestUser(2, "user2", "user2@test.com");
        var user3 = CreateTestUser(3, "user3", "user3@test.com");

        DbContext.Users.AddRange(user1, user2, user3);
        await DbContext.SaveChangesAsync(); // Save users first

        var follow1 = new UserFollow { FollowerId = 2, FollowingId = 1, FollowedAt = DateTime.UtcNow };
        var follow2 = new UserFollow { FollowerId = 3, FollowingId = 1, FollowedAt = DateTime.UtcNow };

        DbContext.UserFollows.AddRange(follow1, follow2);
        await DbContext.SaveChangesAsync();

        // Act
        var result = (await _repository.GetFollowersAsync(1)).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(f => f.FollowerId == 2);
        result.Should().Contain(f => f.FollowerId == 3);
        result.All(f => f.Follower != null).Should().BeTrue();
    }

    [Fact]
    public async Task GetFollowersAsync_NoFollowers_ReturnsEmpty()
    {
        // Arrange
        if (DbContext == null || _repository == null) return;

        await CleanDatabaseAsync();

        // Create dependencies for user
        await CreateUserDependenciesAsync(1);

        var user1 = CreateTestUser(1, "user1", "user1@test.com");
        DbContext.Users.Add(user1);
        await DbContext.SaveChangesAsync();

        // Act
        var result = (await _repository.GetFollowersAsync(1)).ToList();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetFollowersAsync_LoadsFollowerNavigationProperty()
    {
        // Arrange
        if (DbContext == null || _repository == null) return;

        await CleanDatabaseAsync();

        // Create dependencies for users
        await CreateUserDependenciesAsync(1);
        await CreateUserDependenciesAsync(2);

        var follower = CreateTestUser(2, "follower", "follower@test.com");
        var following = CreateTestUser(1, "following", "following@test.com");

        DbContext.Users.AddRange(follower, following);
        await DbContext.SaveChangesAsync(); // Save users first

        var follow = new UserFollow { FollowerId = 2, FollowingId = 1, FollowedAt = DateTime.UtcNow };

        DbContext.UserFollows.Add(follow);
        await DbContext.SaveChangesAsync();

        // Clear tracking to ensure fresh load
        DbContext.ChangeTracker.Clear();

        // Act
        var result = (await _repository.GetFollowersAsync(1)).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Follower.Should().NotBeNull();
        result[0].Follower!.Id.Should().Be(2);
        result[0].Follower.UserName.Should().Be("follower");
    }

    #endregion

    #region GetFollowingAsync Tests

    [Fact]
    public async Task GetFollowingAsync_WithFollowing_ReturnsFollowing()
    {
        // Arrange
        if (DbContext == null || _repository == null) return;

        await CleanDatabaseAsync();

        // Create dependencies for users
        await CreateUserDependenciesAsync(1);
        await CreateUserDependenciesAsync(2);
        await CreateUserDependenciesAsync(3);

        var user1 = CreateTestUser(1, "user1", "user1@test.com");
        var user2 = CreateTestUser(2, "user2", "user2@test.com");
        var user3 = CreateTestUser(3, "user3", "user3@test.com");

        var follow1 = new UserFollow { FollowerId = 1, FollowingId = 2, FollowedAt = DateTime.UtcNow };
        var follow2 = new UserFollow { FollowerId = 1, FollowingId = 3, FollowedAt = DateTime.UtcNow };

        DbContext.Users.AddRange(user1, user2, user3);
        DbContext.UserFollows.AddRange(follow1, follow2);
        await DbContext.SaveChangesAsync();

        // Act
        var result = (await _repository.GetFollowingAsync(1)).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(f => f.FollowingId == 2);
        result.Should().Contain(f => f.FollowingId == 3);
        result.All(f => f.Following != null).Should().BeTrue();
    }

    [Fact]
    public async Task GetFollowingAsync_NoFollowing_ReturnsEmpty()
    {
        // Arrange
        if (DbContext == null || _repository == null) return;

        await CleanDatabaseAsync();

        // Create dependencies for user
        await CreateUserDependenciesAsync(1);

        var user1 = CreateTestUser(1, "user1", "user1@test.com");
        DbContext.Users.Add(user1);
        await DbContext.SaveChangesAsync();

        // Act
        var result = (await _repository.GetFollowingAsync(1)).ToList();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetFollowingAsync_LoadsFollowingNavigationProperty()
    {
        // Arrange
        if (DbContext == null || _repository == null) return;

        await CleanDatabaseAsync();

        // Create dependencies for users
        await CreateUserDependenciesAsync(1);
        await CreateUserDependenciesAsync(2);

        var follower = CreateTestUser(1, "follower", "follower@test.com");
        var following = CreateTestUser(2, "following", "following@test.com");

        DbContext.Users.AddRange(follower, following);
        await DbContext.SaveChangesAsync(); // Save users first

        var follow = new UserFollow { FollowerId = 1, FollowingId = 2, FollowedAt = DateTime.UtcNow };

        DbContext.UserFollows.Add(follow);
        await DbContext.SaveChangesAsync();

        // Clear tracking to ensure fresh load
        DbContext.ChangeTracker.Clear();

        // Act
        var result = (await _repository.GetFollowingAsync(1)).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Following.Should().NotBeNull();
        result[0].Following!.Id.Should().Be(2);
        result[0].Following.UserName.Should().Be("following");
    }

    #endregion

    #region Database Constraints Tests

    [Fact]
    public async Task CreateFollow_DuplicateFollow_ThrowsException()
    {
        // Arrange
        if (DbContext == null || _repository == null) return;

        await CleanDatabaseAsync();

        // Create dependencies for users
        await CreateUserDependenciesAsync(1);
        await CreateUserDependenciesAsync(2);

        var user1 = CreateTestUser(1, "user1", "user1@test.com");
        var user2 = CreateTestUser(2, "user2", "user2@test.com");

        DbContext.Users.AddRange(user1, user2);
        await DbContext.SaveChangesAsync(); // Save users first

        var follow1 = new UserFollow { FollowerId = 1, FollowingId = 2, FollowedAt = DateTime.UtcNow };
        var follow2 = new UserFollow { FollowerId = 1, FollowingId = 2, FollowedAt = DateTime.UtcNow };

        DbContext.UserFollows.Add(follow1);
        await DbContext.SaveChangesAsync();

        // Act & Assert
        DbContext.UserFollows.Add(follow2);
        var act = async () => await DbContext.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task CreateFollow_InvalidFollowerId_ThrowsException()
    {
        // Arrange
        if (DbContext == null || _repository == null) return;

        await CleanDatabaseAsync();

        var follow = new UserFollow
        {
            FollowerId = 999, // Non-existent user
            FollowingId = 1,
            FollowedAt = DateTime.UtcNow
        };

        // Act & Assert
        DbContext.UserFollows.Add(follow);
        var act = async () => await DbContext.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task CreateFollow_InvalidFollowingId_ThrowsException()
    {
        // Arrange
        if (DbContext == null || _repository == null) return;

        await CleanDatabaseAsync();

        // Create dependencies for user
        await CreateUserDependenciesAsync(1);

        var user1 = CreateTestUser(1, "user1", "user1@test.com");
        DbContext.Users.Add(user1);
        await DbContext.SaveChangesAsync();

        var follow = new UserFollow
        {
            FollowerId = 1,
            FollowingId = 999, // Non-existent user
            FollowedAt = DateTime.UtcNow
        };

        // Act & Assert
        DbContext.UserFollows.Add(follow);
        var act = async () => await DbContext.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    #endregion
}

