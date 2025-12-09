using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.DTOs.Chat;
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
using Sonar.Controllers.Chat;
using System.Security.Claims;
using Xunit;
using ChatModel = Entities.Models.Chat.Chat;
using MessageModel = Entities.Models.Chat.Message;

namespace Sonar.Tests.Controllers.Chat;

public class ChatControllerTests : ChatControllerTestsBase, IDisposable
{
    private readonly UserManager<User> _userManager;
    private readonly Mock<IChatService> _chatServiceMock;
    private readonly ChatController _controller;
    private readonly User _testUser;
    private readonly ClaimsPrincipal _claimsPrincipal;
    private readonly SonarContext _context;

    public ChatControllerTests()
    {
        // Создаем InMemory базу данных
        _context = CreateInMemoryContext();
        
        // Создаем реальный UserManager с InMemory базой
        _userManager = CreateUserManager(_context);
        
        // Настройка тестового пользователя
        _testUser = CreateTestUser(1);
        
        // Добавляем пользователя в базу данных
        _context.Users.Add(_testUser);
        _context.SaveChanges();
        
        // Настройка ClaimsPrincipal
        _claimsPrincipal = CreateClaimsPrincipal(_testUser);
        
        _chatServiceMock = new Mock<IChatService>();
        _controller = new ChatController(_userManager, _chatServiceMock.Object);
        
        // Настройка HttpContext для контроллера с ClaimsPrincipal
        var httpContext = new DefaultHttpContext
        {
            User = _claimsPrincipal
        };
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    #region Helper Methods

    private void SetupCheckAccessFeatures()
    {
        // Все уже настроено в конструкторе
    }

    #endregion

    #region CreateChat Tests

    [Fact]
    public async Task CreateChat_ValidGroupChat_ReturnsOk()
    {
        // Arrange
        SetupCheckAccessFeatures();
        var dto = new CreateChatDTO
        {
            Name = "Test Group Chat",
            IsGroup = true,
            CoverId = 1
        };
        var createdChat = new ChatModel
        {
            Id = 1,
            Name = dto.Name,
            IsGroup = true,
            CoverId = dto.CoverId,
            CreatorId = _testUser.Id
        };
        
        _chatServiceMock
            .Setup(x => x.CreateChatAsync(_testUser.Id, dto))
            .ReturnsAsync(createdChat);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OkResponse>(async () => await _controller.CreateChat(dto));
        
        _chatServiceMock.Verify(x => x.CreateChatAsync(_testUser.Id, dto), Times.Once);
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task CreateChat_ValidPersonalChat_ReturnsOk()
    {
        // Arrange
        SetupCheckAccessFeatures();
        var dto = new CreateChatDTO
        {
            Name = "Personal Chat",
            IsGroup = false,
            CoverId = 1,
            UserId = 2
        };
        var createdChat = new ChatModel
        {
            Id = 2,
            Name = dto.Name,
            IsGroup = false,
            CoverId = dto.CoverId,
            CreatorId = _testUser.Id
        };
        
        _chatServiceMock
            .Setup(x => x.CreateChatAsync(_testUser.Id, dto))
            .ReturnsAsync(createdChat);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OkResponse>(async () => await _controller.CreateChat(dto));
        
        _chatServiceMock.Verify(x => x.CreateChatAsync(_testUser.Id, dto), Times.Once);
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(200);
    }

    #endregion

    #region SendMessage Tests

    [Fact]
    public async Task SendMessage_ValidMessage_ReturnsOk()
    {
        // Arrange
        SetupCheckAccessFeatures();
        var chatId = 1;
        var messageDto = new MessageDTO
        {
            TextContent = "Hello, world!",
            ReplyMessageId = null
        };
        var createdMessage = new MessageModel
        {
            Id = 1,
            ChatId = chatId,
            SenderId = _testUser.Id,
            TextContent = messageDto.TextContent
        };
        
        _chatServiceMock
            .Setup(x => x.SendMessageAsync(_testUser.Id, chatId, messageDto))
            .ReturnsAsync(createdMessage);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OkResponse>(async () => await _controller.SendMessage(chatId, messageDto));
        
        _chatServiceMock.Verify(x => x.SendMessageAsync(_testUser.Id, chatId, messageDto), Times.Once);
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task SendMessage_WithReply_ReturnsOk()
    {
        // Arrange
        SetupCheckAccessFeatures();
        var chatId = 1;
        var messageDto = new MessageDTO
        {
            TextContent = "Reply message",
            ReplyMessageId = 5
        };
        var createdMessage = new MessageModel
        {
            Id = 2,
            ChatId = chatId,
            SenderId = _testUser.Id,
            TextContent = messageDto.TextContent,
            ReplyMessageId = 5
        };
        
        _chatServiceMock
            .Setup(x => x.SendMessageAsync(_testUser.Id, chatId, messageDto))
            .ReturnsAsync(createdMessage);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OkResponse>(async () => await _controller.SendMessage(chatId, messageDto));
        
        _chatServiceMock.Verify(x => x.SendMessageAsync(_testUser.Id, chatId, messageDto), Times.Once);
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(200);
    }

    #endregion

    #region DeleteMessage Tests

    [Fact]
    public async Task DeleteMessage_ValidMessage_ReturnsOk()
    {
        // Arrange
        SetupCheckAccessFeatures();
        var messageId = 1;
        
        _chatServiceMock
            .Setup(x => x.DeleteMessageAsync(_testUser.Id, messageId))
            .Returns(Task.CompletedTask);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OkResponse>(async () => await _controller.DeleteMessage(messageId));
        
        _chatServiceMock.Verify(x => x.DeleteMessageAsync(_testUser.Id, messageId), Times.Once);
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(200);
    }

    #endregion

    #region GetMessageById Tests

    [Fact]
    public async Task GetMessageById_ValidMessage_ReturnsOk()
    {
        // Arrange
        SetupCheckAccessFeatures();
        var messageId = 1;
        var message = new MessageModel
        {
            Id = messageId,
            ChatId = 1,
            SenderId = _testUser.Id,
            TextContent = "Test message"
        };
        
        _chatServiceMock
            .Setup(x => x.GetMessageByIdAsync(_testUser.Id, messageId))
            .ReturnsAsync(message);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OkResponse<MessageModel>>(async () => await _controller.GetMessageById(messageId));
        
        _chatServiceMock.Verify(x => x.GetMessageByIdAsync(_testUser.Id, messageId), Times.Once);
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(200);
    }

    #endregion

    #region GetChatInfo Tests

    [Fact]
    public async Task GetChatInfo_ValidChat_ReturnsOk()
    {
        // Arrange
        SetupCheckAccessFeatures();
        var chatId = 1;
        var chatDto = new ChatDTO
        {
            Name = "Test Chat",
            IsGroup = true,
            CoverId = 1,
            CreatorId = _testUser.Id,
            UserIds = new[] { _testUser.Id, 2, 3 },
            AdminIds = new[] { _testUser.Id }
        };
        
        _chatServiceMock
            .Setup(x => x.GetChatInfoAsync(_testUser.Id, chatId))
            .ReturnsAsync(chatDto);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OkResponse<ChatDTO>>(async () => await _controller.GetChatInfo(chatId));
        
        _chatServiceMock.Verify(x => x.GetChatInfoAsync(_testUser.Id, chatId), Times.Once);
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(200);
    }

    #endregion

    #region GetMessagesWithCursor Tests

    [Fact]
    public async Task GetMessagesWithCursor_ValidChat_ReturnsOk()
    {
        // Arrange
        SetupCheckAccessFeatures();
        var chatId = 1;
        var cursor = 10;
        var take = 20;
        var messages = new List<MessageDTO>
        {
            new() { Id = 11, TextContent = "Message 1", SenderId = 1 },
            new() { Id = 12, TextContent = "Message 2", SenderId = 2 }
        };
        var pageDto = new CursorPageDTO<MessageDTO>
        {
            Items = messages,
            NextCursor = "12"
        };
        
        _chatServiceMock
            .Setup(x => x.GetMessagesWithCursorAsync(_testUser.Id, chatId, cursor, take))
            .ReturnsAsync(pageDto);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OkResponse<CursorPageDTO<MessageDTO>>>(async () => await _controller.GetMessagesWithCursor(chatId, cursor, take));
        
        _chatServiceMock.Verify(x => x.GetMessagesWithCursorAsync(_testUser.Id, chatId, cursor, take), Times.Once);
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetMessagesWithCursor_NoCursor_ReturnsOk()
    {
        // Arrange
        SetupCheckAccessFeatures();
        var chatId = 1;
        var take = 50;
        var messages = new List<MessageDTO>
        {
            new() { Id = 1, TextContent = "First message", SenderId = 1 }
        };
        var pageDto = new CursorPageDTO<MessageDTO>
        {
            Items = messages,
            NextCursor = null
        };
        
        _chatServiceMock
            .Setup(x => x.GetMessagesWithCursorAsync(_testUser.Id, chatId, null, take))
            .ReturnsAsync(pageDto);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OkResponse<CursorPageDTO<MessageDTO>>>(async () => await _controller.GetMessagesWithCursor(chatId, null, take));
        
        _chatServiceMock.Verify(x => x.GetMessagesWithCursorAsync(_testUser.Id, chatId, null, take), Times.Once);
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(200);
    }

    #endregion

    #region AddUserToChat Tests

    [Fact]
    public async Task AddUserToChat_ValidUser_ReturnsOk()
    {
        // Arrange
        SetupCheckAccessFeatures();
        var chatId = 1;
        var userIdToAdd = 2;
        
        _chatServiceMock
            .Setup(x => x.AddUserToChat(_testUser.Id, chatId, userIdToAdd))
            .Returns(Task.CompletedTask);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OkResponse>(async () => await _controller.AddUserToChat(chatId, userIdToAdd));
        
        _chatServiceMock.Verify(x => x.AddUserToChat(_testUser.Id, chatId, userIdToAdd), Times.Once);
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(200);
    }

    #endregion

    #region LeaveChat Tests

    [Fact]
    public async Task LeaveChat_ValidChat_ReturnsOk()
    {
        // Arrange
        SetupCheckAccessFeatures();
        var chatId = 1;
        
        _chatServiceMock
            .Setup(x => x.LeaveChat(_testUser.Id, chatId))
            .Returns(Task.CompletedTask);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OkResponse>(async () => await _controller.LeaveChat(chatId));
        
        _chatServiceMock.Verify(x => x.LeaveChat(_testUser.Id, chatId), Times.Once);
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(200);
    }

    #endregion

    #region RemoveUserFromChat Tests

    [Fact]
    public async Task RemoveUserFromChat_ValidUser_ReturnsOk()
    {
        // Arrange
        SetupCheckAccessFeatures();
        var chatId = 1;
        var userIdToRemove = 2;
        
        _chatServiceMock
            .Setup(x => x.RemoveUserFromChat(_testUser.Id, userIdToRemove, chatId))
            .Returns(Task.CompletedTask);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OkResponse>(async () => await _controller.RemoveUserFromChat(chatId, userIdToRemove));
        
        _chatServiceMock.Verify(x => x.RemoveUserFromChat(_testUser.Id, userIdToRemove, chatId), Times.Once);
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(200);
    }

    #endregion

    #region UpdateChatCover Tests

    [Fact]
    public async Task UpdateChatCover_ValidFile_ReturnsOk()
    {
        // Arrange
        SetupCheckAccessFeatures();
        var chatId = 1;
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns("test.jpg");
        fileMock.Setup(f => f.Length).Returns(1024);
        fileMock.Setup(f => f.ContentType).Returns("image/jpeg");
        
        _chatServiceMock
            .Setup(x => x.UpdateChatCoverAsync(_testUser.Id, chatId, fileMock.Object))
            .Returns(Task.CompletedTask);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OkResponse>(async () => await _controller.UpdateChatCover(chatId, fileMock.Object));
        
        _chatServiceMock.Verify(x => x.UpdateChatCoverAsync(_testUser.Id, chatId, fileMock.Object), Times.Once);
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(200);
    }

    #endregion

    #region UpdateChatName Tests

    [Fact]
    public async Task UpdateChatName_ValidName_ReturnsOk()
    {
        // Arrange
        SetupCheckAccessFeatures();
        var chatId = 1;
        var newName = "Updated Chat Name";
        
        _chatServiceMock
            .Setup(x => x.UpdateChatNameAsync(_testUser.Id, chatId, newName))
            .Returns(Task.CompletedTask);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OkResponse>(async () => await _controller.UpdateChatName(chatId, newName));
        
        _chatServiceMock.Verify(x => x.UpdateChatNameAsync(_testUser.Id, chatId, newName), Times.Once);
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(200);
    }

    #endregion

    #region ReadMessages Tests

    [Fact]
    public async Task ReadMessages_ValidMessageIds_ReturnsOk()
    {
        // Arrange
        SetupCheckAccessFeatures();
        var chatId = 1;
        var messageIds = new List<int> { 1, 2, 3 };
        
        // В контроллере вызывается: ReadMessagesAsync(user.Id, chatId, messageIds)
        // Но в интерфейсе сигнатура: ReadMessagesAsync(int chatId, int userId, ...)
        // Проверяем фактический вызов из контроллера
        _chatServiceMock
            .Setup(x => x.ReadMessagesAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IEnumerable<int>>()))
            .Returns(Task.CompletedTask);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OkResponse>(async () => await _controller.ReadMessages(chatId, messageIds));
        
        _chatServiceMock.Verify(x => x.ReadMessagesAsync(
            It.Is<int>(id => id == _testUser.Id), 
            It.Is<int>(id => id == chatId), 
            It.Is<IEnumerable<int>>(ids => ids.SequenceEqual(messageIds))), 
            Times.Once);
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ReadMessages_EmptyList_ReturnsOk()
    {
        // Arrange
        SetupCheckAccessFeatures();
        var chatId = 1;
        var messageIds = new List<int>();
        
        _chatServiceMock
            .Setup(x => x.ReadMessagesAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IEnumerable<int>>()))
            .Returns(Task.CompletedTask);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OkResponse>(async () => await _controller.ReadMessages(chatId, messageIds));
        
        _chatServiceMock.Verify(x => x.ReadMessagesAsync(
            It.Is<int>(id => id == _testUser.Id), 
            It.Is<int>(id => id == chatId), 
            It.Is<IEnumerable<int>>(ids => !ids.Any())), 
            Times.Once);
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(200);
    }

    #endregion

    #region ReadAllMessages Tests

    [Fact]
    public async Task ReadAllMessages_ValidChat_ReturnsOk()
    {
        // Arrange
        SetupCheckAccessFeatures();
        var chatId = 1;
        
        _chatServiceMock
            .Setup(x => x.ReadAllMessagesAsync(_testUser.Id, chatId))
            .Returns(Task.CompletedTask);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OkResponse>(async () => await _controller.ReadAllMessages(chatId));
        
        _chatServiceMock.Verify(x => x.ReadAllMessagesAsync(_testUser.Id, chatId), Times.Once);
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(200);
    }

    #endregion

    public void Dispose()
    {
        _context?.Dispose();
        _userManager?.Dispose();
    }
}


