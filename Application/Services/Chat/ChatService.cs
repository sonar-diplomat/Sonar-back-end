using Application.Abstractions.Interfaces.Repository.Chat;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.DTOs;
using Application.DTOs.Chat;
using Application.Extensions;
using Application.Response;
using Entities.Models.Chat;
using Entities.Models.File;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Http;
using ChatModel = Entities.Models.Chat.Chat;

namespace Application.Services.Chat;

public class ChatService(
    IChatRepository repository,
    IImageFileService imageFileService,
    IMessageService messageService,
    IMessageReadService messageReadService,
    IUserService userService,
    IChatNotifier notifier
) : GenericService<ChatModel>(repository), IChatService
{
    public async Task UpdateChatCoverAsync(int userId, int chatId, IFormFile file)
    {
        ChatModel chat = await CheckUserIsMemberAsync(userId, chatId, true);
        ImageFile image = await imageFileService.UploadFileAsync(file);
        chat.CoverId = image.Id;
        await repository.UpdateAsync(chat);
    }

    public async Task UpdateChatNameAsync(int userId, int chatId, string newName)
    {
        ChatModel chat = await CheckUserIsMemberAsync(userId, chatId, true);
        chat.Name = newName;
        await repository.UpdateAsync(chat);
    }

    public async Task<Message> GetMessageByIdAsync(int userId, int messageId)
    {
        Message message = await messageService.GetByIdValidatedAsync(messageId);
        await CheckUserIsMemberAsync(userId, message.ChatId);
        return message;
    }

    public async Task DeleteMessageAsync(int userId, int messageId)
    {
        Message message = await messageService.GetByIdValidatedAsync(messageId);
        if (message.SenderId == userId)
            await messageService.DeleteAsync(messageId);
        throw ResponseFactory.Create<ForbiddenResponse>(["User is not the sender of the message"]);
    }

    public async Task<ChatDTO> GetChatInfoAsync(int userId, int chatId)
    {
        await CheckUserIsMemberAsync(userId, chatId);
        ChatModel chat = await repository.Include(c => c.Members)
            .Include(c => c.Cover).GetByIdValidatedAsync(chatId);
        ChatDTO dto = new()
        {
            Name = chat.Name,
            Cover = chat.Cover,
            UserIds = chat.Members.Select(u => u.Id).ToArray()
        };
        return dto;
    }

    // TODO: Create some service for cleaning up old reads
    public async Task ReadMessagesAsync(int userId, int chatId, IEnumerable<int> messageIds)
    {
        await CheckUserIsMemberAsync(userId, chatId);

        List<int> validMessageIds = (await messageService.GetMessagesByIdsAsync(messageIds, chatId)).ToList();

        if (validMessageIds.Count == 0)
            throw ResponseFactory.Create<ForbiddenResponse>(["No valid messages to mark as read"]);

        IEnumerable<int> existingReadIds = await messageReadService.GetReadMessageIdsAsync(userId, validMessageIds);
        HashSet<int> existingReadSet = new(existingReadIds);

        DateTime now = DateTime.UtcNow;

        List<MessageRead> newReads = [];

        foreach (int messageId in validMessageIds)
            if (!existingReadSet.Contains(messageId))
            {
                newReads.Add(new MessageRead
                {
                    UserId = userId,
                    MessageId = messageId,
                    ReadAt = now
                });
            }
            else
            {
                MessageRead? existing = await messageReadService.GetReadRecordAsync(userId, messageId);
                if (existing?.ReadAt == null)
                    existing!.ReadAt = now;
            }

        if (newReads.Count > 0)
        {
            await messageReadService.AddRangeAsync(newReads);

            await notifier.MessagesRead(new MessagesReadEvent(
                chatId,
                userId,
                newReads.Select(mr => mr.Id).ToList(),
                now
            ));
        }
    }


    public async Task ReadAllMessagesAsync(int userId, int chatId)
    {
        await CheckUserIsMemberAsync(userId, chatId);
        ChatModel chat = await repository.Include(c => c.Messages).GetByIdValidatedAsync(chatId);
        foreach (Message message in chat.Messages)
        {
            MessageRead messageRead = new()
            {
                MessageId = message.Id,
                UserId = userId,
                ReadAt = DateTime.UtcNow
            };
            await messageReadService.CreateAsync(messageRead);
        }
    }

    public async Task AddUserToChat(int memberId, int chatId, int userId)
    {
        ChatModel chat = await CheckUserIsMemberAsync(memberId, chatId, true);
        if (!chat.IsGroup)
            throw ResponseFactory.Create<ForbiddenResponse>(["User is not the member of the chat"]);
        User user = await userService.GetByIdValidatedAsync(userId);
        if (chat.Members.Any(u => u.Id == userId))
            throw ResponseFactory.Create<BadRequestResponse>(["User is the member of the chat"]);
        chat.Members.Add(user);
        await repository.UpdateAsync(chat);
    }

    public async Task RemoveUserFromChat(int initiatorId, int userId, int chatId)
    {
        ChatModel chat = await CheckUserCanManageChatAsync(initiatorId, chatId);
        if (chat.Members.Any(u => u.Id == userId))
        {
            chat.Members.Remove(await userService.GetByIdValidatedAsync(userId));
            return;
        }

        if (chat.CreatorId != initiatorId || chat.Admins.All(a => a.Id != userId))
            throw ResponseFactory.Create<ForbiddenResponse>(["User is not a regular member of the chat"]);
        chat.Admins.Remove(await userService.GetByIdValidatedAsync(userId));
    }

    public async Task LeaveChat(int userId, int chatId)
    {
        ChatModel chat = await CheckUserIsMemberAsync(userId, chatId, true);
        if (chat.CreatorId == userId)
        {
            if (chat.Admins.Count != 0)
                chat.CreatorId = chat.Admins.First(a => a.Id == userId).Id;
            else if (chat.Members.Count != 0)
                chat.CreatorId = chat.Members.First(a => a.Id == userId).Id;
            else
                await DeleteAsync(chat);
        }

        if (chat.Admins.Any(a => a.Id == userId))
            chat.Admins.Remove(await userService.GetByIdValidatedAsync(chatId));
        chat.Members.Remove(await userService.GetByIdValidatedAsync(userId));
    }

    public async Task<ChatModel> CreateChatAsync(int userId, CreateChatDTO dto)
    {
        ChatModel chat = new()
        {
            Name = dto.Name,
            IsGroup = dto.IsGroup,
            CoverId = dto.CoverId,
            CreatorId = userId
        };
        await repository.AddAsync(chat);
        if (chat.IsGroup)
            return await repository.UpdateAsync(chat);
        if (dto.UserId == null)
            throw ResponseFactory.Create<ForbiddenResponse>(["Personal chat requires another user"]);
        chat.Members.Add(await userService.GetByIdValidatedAsync((int)dto.UserId));
        return await repository.UpdateAsync(chat);
    }

    public async Task<Message> SendMessageAsync(int userId, int chatId, MessageDTO message)
    {
        await CheckUserIsMemberAsync(userId, chatId);
        Message created = await messageService.CreateAsync(userId, chatId, message);

        await notifier.MessageCreated(new MessageCreatedEvent(
            created.Id,
            created.ChatId,
            created.SenderId,
            created.TextContent,
            created.ReplyMessageId,
            created.CreatedAt
        ));

        return created;
    }

    public async Task<CursorPageDTO<MessageDTO>> GetMessagesWithCursorAsync(int userId, int chatId, int? cursor,
        int take = 50)
    {
        await CheckUserIsMemberAsync(userId, chatId);
        return await messageService.GetMessagesWithCursorAsync(chatId, cursor, take);
    }

    private async Task<ChatModel> CheckUserCanManageChatAsync(int userId, int chatId)
    {
        ChatModel chat = await CheckUserIsMemberAsync(userId, chatId, true);
        if (chat.Admins.All(a => a.Id != userId) && chat.CreatorId != userId)
            throw ResponseFactory.Create<ForbiddenResponse>(["The user does not have admin privileges"]);
        return chat;
    }

    private async Task<ChatModel> CheckUserIsMemberAsync(int userId, int chatId, bool blockIfPrivate = false)
    {
        ChatModel chat = await repository
            .Include(c => c.Members)
            .Include(c => c.Admins)
            .Include(c => c.Creator)
            .GetByIdValidatedAsync(chatId);

        if (!chat.IsGroup && blockIfPrivate)
            throw ResponseFactory.Create<ForbiddenResponse>(["This cannot be done in the personal chat"]);

        HashSet<int> userIds = chat.Members.Select(u => u.Id).ToHashSet();
        return userIds.Contains(userId)
            ? chat
            : throw ResponseFactory.Create<ForbiddenResponse>(["User is not a member of the chat"]);
    }
}