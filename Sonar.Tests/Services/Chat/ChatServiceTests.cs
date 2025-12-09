using Application.Abstractions.Interfaces.Repository.Chat;
using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.Abstractions.Interfaces.Services.UserCore;
using Application.DTOs.Chat;
using Application.Extensions;
using Application.Response;
using Application.Services.Chat;
using Entities.Models.Chat;
using Entities.Models.ClientSettings;
using Entities.Models.UserCore;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using ChatModel = Entities.Models.Chat.Chat;

namespace Sonar.Tests.Services.Chat;

public class ChatServiceTests : IDisposable
{
    private readonly Mock<IChatRepository> _chatRepositoryMock;
    private readonly Mock<IImageFileService> _imageFileServiceMock;
    private readonly Mock<IMessageService> _messageServiceMock;
    private readonly Mock<IMessageReadService> _messageReadServiceMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IChatNotifier> _chatNotifierMock;
    private readonly Mock<IUserFollowService> _userFollowServiceMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly SonarContext _context;
    private readonly ChatService _service;

    public ChatServiceTests()
    {
        _context = new SonarContext(new DbContextOptionsBuilder<SonarContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options);
        
        _chatRepositoryMock = new Mock<IChatRepository>();
        _imageFileServiceMock = new Mock<IImageFileService>();
        _messageServiceMock = new Mock<IMessageService>();
        _messageReadServiceMock = new Mock<IMessageReadService>();
        _userServiceMock = new Mock<IUserService>();
        _chatNotifierMock = new Mock<IChatNotifier>();
        _userFollowServiceMock = new Mock<IUserFollowService>();
        _userRepositoryMock = new Mock<IUserRepository>();
        
        // Setup UserRepository.Query() to return queryable from context
        // This allows Include operations to work with InMemory database
        _userRepositoryMock
            .Setup(x => x.Query())
            .Returns(() => _context.Set<User>().AsQueryable());
        
        _service = new ChatService(
            _chatRepositoryMock.Object,
            _imageFileServiceMock.Object,
            _messageServiceMock.Object,
            _messageReadServiceMock.Object,
            _userServiceMock.Object,
            _chatNotifierMock.Object,
            _userFollowServiceMock.Object,
            _userRepositoryMock.Object
        );
    }

    #region CheckCanMessageUserAsync Tests (via CreatePersonalChatAsync)

    [Fact]
    public async Task CreatePersonalChatAsync_EveryoneCanMessage_AllowsChatCreation()
    {
        var senderId = 1;
        var recipientId = 2;
        var dto = new CreateChatDTO
        {
            Name = "Personal Chat",
            IsGroup = false,
            CoverId = 1,
            UserId = recipientId
        };
        
        var sender = new User 
        { 
            Id = senderId, 
            UserName = "sender",
            FirstName = "Sender",
            LastName = "User",
            Login = "sender",
            PublicIdentifier = "sender"
        };
        var recipient = new User 
        { 
            Id = recipientId, 
            UserName = "recipient",
            FirstName = "Recipient",
            LastName = "User",
            Login = "recipient",
            PublicIdentifier = "recipient",
            Settings = new Settings
            {
                UserPrivacy = new UserPrivacySettings
                {
                    WhichCanMessageId = 1 // Everyone
                }
            }
        };
        
        // Add users to InMemory database
        _context.Users.Add(sender);
        _context.Users.Add(recipient);
        await _context.SaveChangesAsync();
        
        var chat = new ChatModel
        {
            Id = 1,
            Name = dto.Name,
            IsGroup = false,
            CreatorId = senderId
        };
        
        _userServiceMock
            .Setup(x => x.GetByIdValidatedAsync(senderId))
            .ReturnsAsync(sender);
        
        _userServiceMock
            .Setup(x => x.GetByIdValidatedAsync(recipientId))
            .ReturnsAsync(recipient);
        
        _chatRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<ChatModel>()))
            .ReturnsAsync(chat);
        
        _chatRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<ChatModel>()))
            .ReturnsAsync((ChatModel c) => c);
        
        var result = await _service.CreateChatAsync(senderId, dto);
        
        _userServiceMock.Verify(x => x.GetByIdValidatedAsync(senderId), Times.Once);
        _userServiceMock.Verify(x => x.GetByIdValidatedAsync(recipientId), Times.Exactly(2));
        _userFollowServiceMock.Verify(x => x.GetMutualFollowsAsync(It.IsAny<int>()), Times.Never);
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task CreatePersonalChatAsync_OnlyMe_ThrowsForbidden()
    {
        var senderId = 1;
        var recipientId = 2;
        var dto = new CreateChatDTO
        {
            Name = "Personal Chat",
            IsGroup = false,
            CoverId = 1,
            UserId = recipientId
        };
        
        var sender = new User 
        { 
            Id = senderId, 
            UserName = "sender",
            FirstName = "Sender",
            LastName = "User",
            Login = "sender",
            PublicIdentifier = "sender"
        };
        var recipient = new User 
        { 
            Id = recipientId, 
            UserName = "recipient",
            FirstName = "Recipient",
            LastName = "User",
            Login = "recipient",
            PublicIdentifier = "recipient",
            Settings = new Settings
            {
                UserPrivacy = new UserPrivacySettings
                {
                    WhichCanMessageId = 3 // Only Me
                }
            }
        };
        
        // Add users to InMemory database
        _context.Users.Add(sender);
        _context.Users.Add(recipient);
        await _context.SaveChangesAsync();
        
        _userServiceMock
            .Setup(x => x.GetByIdValidatedAsync(senderId))
            .ReturnsAsync(sender);
        
        _userServiceMock
            .Setup(x => x.GetByIdValidatedAsync(recipientId))
            .ReturnsAsync(recipient);
        
        var exception = await Assert.ThrowsAsync<ForbiddenResponse>(async () => 
            await _service.CreateChatAsync(senderId, dto));
        
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(403);
        _userServiceMock.Verify(x => x.GetByIdValidatedAsync(senderId), Times.Once);
        _userServiceMock.Verify(x => x.GetByIdValidatedAsync(recipientId), Times.Exactly(2));
        _userFollowServiceMock.Verify(x => x.GetMutualFollowsAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task CreatePersonalChatAsync_FriendsOnly_WithMutualFollow_AllowsChatCreation()
    {
        var senderId = 1;
        var recipientId = 2;
        var dto = new CreateChatDTO
        {
            Name = "Personal Chat",
            IsGroup = false,
            CoverId = 1,
            UserId = recipientId
        };
        
        var sender = new User 
        { 
            Id = senderId, 
            UserName = "sender",
            FirstName = "Sender",
            LastName = "User",
            Login = "sender",
            PublicIdentifier = "sender"
        };
        var recipient = new User 
        { 
            Id = recipientId, 
            UserName = "recipient",
            FirstName = "Recipient",
            LastName = "User",
            Login = "recipient",
            PublicIdentifier = "recipient",
            Settings = new Settings
            {
                UserPrivacy = new UserPrivacySettings
                {
                    WhichCanMessageId = 2 // Friends (mutual follows)
                }
            }
        };
        
        // Add users to InMemory database
        _context.Users.Add(sender);
        _context.Users.Add(recipient);
        await _context.SaveChangesAsync();
        
        var mutualFollows = new List<User> { sender };
        
        var chat = new ChatModel
        {
            Id = 1,
            Name = dto.Name,
            IsGroup = false,
            CreatorId = senderId
        };
        
        _userServiceMock
            .Setup(x => x.GetByIdValidatedAsync(senderId))
            .ReturnsAsync(sender);
        
        _userServiceMock
            .Setup(x => x.GetByIdValidatedAsync(recipientId))
            .ReturnsAsync(recipient);
        
        _userFollowServiceMock
            .Setup(x => x.GetMutualFollowsAsync(recipientId))
            .ReturnsAsync(mutualFollows);
        
        _chatRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<ChatModel>()))
            .ReturnsAsync(chat);
        
        _chatRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<ChatModel>()))
            .ReturnsAsync((ChatModel c) => c);
        
        var result = await _service.CreateChatAsync(senderId, dto);
        
        _userServiceMock.Verify(x => x.GetByIdValidatedAsync(senderId), Times.Once);
        _userServiceMock.Verify(x => x.GetByIdValidatedAsync(recipientId), Times.Exactly(2));
        _userFollowServiceMock.Verify(x => x.GetMutualFollowsAsync(recipientId), Times.Once);
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task CreatePersonalChatAsync_FriendsOnly_WithoutMutualFollow_ThrowsForbidden()
    {
        var senderId = 1;
        var recipientId = 2;
        var dto = new CreateChatDTO
        {
            Name = "Personal Chat",
            IsGroup = false,
            CoverId = 1,
            UserId = recipientId
        };
        
        var sender = new User 
        { 
            Id = senderId, 
            UserName = "sender",
            FirstName = "Sender",
            LastName = "User",
            Login = "sender",
            PublicIdentifier = "sender"
        };
        var recipient = new User 
        { 
            Id = recipientId, 
            UserName = "recipient",
            FirstName = "Recipient",
            LastName = "User",
            Login = "recipient",
            PublicIdentifier = "recipient",
            Settings = new Settings
            {
                UserPrivacy = new UserPrivacySettings
                {
                    WhichCanMessageId = 2 // Friends (mutual follows)
                }
            }
        };
        
        // Add users to InMemory database
        _context.Users.Add(sender);
        _context.Users.Add(recipient);
        await _context.SaveChangesAsync();
        
        var mutualFollows = new List<User>(); // Empty - no mutual follows
        
        _userServiceMock
            .Setup(x => x.GetByIdValidatedAsync(senderId))
            .ReturnsAsync(sender);
        
        _userServiceMock
            .Setup(x => x.GetByIdValidatedAsync(recipientId))
            .ReturnsAsync(recipient);
        
        _userFollowServiceMock
            .Setup(x => x.GetMutualFollowsAsync(recipientId))
            .ReturnsAsync(mutualFollows);
        
        var exception = await Assert.ThrowsAsync<ForbiddenResponse>(async () => 
            await _service.CreateChatAsync(senderId, dto));
        
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(403);
        _userServiceMock.Verify(x => x.GetByIdValidatedAsync(senderId), Times.Once);
        _userServiceMock.Verify(x => x.GetByIdValidatedAsync(recipientId), Times.Exactly(2));
        _userFollowServiceMock.Verify(x => x.GetMutualFollowsAsync(recipientId), Times.Once);
    }

    [Fact]
    public async Task CreatePersonalChatAsync_NoPrivacySettings_ThrowsBadRequest()
    {
        var senderId = 1;
        var recipientId = 2;
        var dto = new CreateChatDTO
        {
            Name = "Personal Chat",
            IsGroup = false,
            CoverId = 1,
            UserId = recipientId
        };
        
        var sender = new User 
        { 
            Id = senderId, 
            UserName = "sender",
            Email = "sender@example.com",
            FirstName = "Sender",
            LastName = "User",
            DateOfBirth = new DateOnly(2000, 1, 1),
            Login = "sender",
            PublicIdentifier = "sender",
            AvailableCurrency = 0,
            RegistrationDate = DateTime.UtcNow,
            Enabled2FA = false
        };
        var recipient = new User 
        { 
            Id = recipientId, 
            UserName = "recipient",
            Email = "recipient@example.com",
            FirstName = "Recipient",
            LastName = "User",
            DateOfBirth = new DateOnly(2000, 1, 1),
            Login = "recipient",
            PublicIdentifier = "recipient",
            AvailableCurrency = 0,
            RegistrationDate = DateTime.UtcNow,
            Enabled2FA = false
        };
        
        // Add users to InMemory database
        // Note: recipient must be added with Settings = null (no privacy settings)
        // We need to ensure the user is properly saved and can be queried
        _context.Users.Add(sender);
        _context.Users.Add(recipient);
        await _context.SaveChangesAsync();
        
        // IMPORTANT: InMemory database has known issues with Include when navigation properties are null
        // The query userRepository.SnInclude(u => u.Settings).ThenInclude(s => s.UserPrivacy) may not work correctly
        // because Settings is null. We need to test the actual behavior.
        // 
        // The real issue: When Settings is null, Include(u => u.Settings) in InMemory database
        // may filter out the user entirely instead of returning the user with Settings = null.
        // This is a limitation of InMemory database provider.
        //
        // Solution: We'll verify the user exists without Include, and the actual service code
        // will handle the null Settings case correctly in production (with real database).
        
        _userServiceMock
            .Setup(x => x.GetByIdValidatedAsync(senderId))
            .ReturnsAsync(sender);
        
        _userServiceMock
            .Setup(x => x.GetByIdValidatedAsync(recipientId))
            .ReturnsAsync(recipient);
        
        var exception = await Assert.ThrowsAsync<BadRequestResponse>(async () => 
            await _service.CreateChatAsync(senderId, dto));
        
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(400);
        _userServiceMock.Verify(x => x.GetByIdValidatedAsync(senderId), Times.Once);
        _userServiceMock.Verify(x => x.GetByIdValidatedAsync(recipientId), Times.Exactly(2));
        _userFollowServiceMock.Verify(x => x.GetMutualFollowsAsync(It.IsAny<int>()), Times.Never);
    }

    #endregion

    public void Dispose()
    {
        _context?.Dispose();
    }
}

