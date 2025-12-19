using Application.Abstractions.Interfaces.Repository.Chat;
using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.Abstractions.Interfaces.Services.UserCore;
using Application.DTOs;
using Application.DTOs.Chat;
using Application.Extensions;
using Application.Response;
using Entities.Models.Chat;
using Entities.Models.File;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ChatModel = Entities.Models.Chat.Chat;

namespace Application.Services.Chat;

public class ChatService(
    IChatRepository repository,
    IImageFileService imageFileService,
    IMessageService messageService,
    IMessageReadService messageReadService,
    IUserService userService,
    IChatNotifier notifier,
    IUserFollowService userFollowService,
    IUserRepository userRepository
) : GenericService<ChatModel>(repository), IChatService
{
    public async Task UpdateChatCoverAsync(int userId, int chatId, IFormFile file)
    {
        ChatModel chat = await CheckUserIsMemberAsync(userId, chatId, true);
        ImageFile image = await imageFileService.UploadFileAsync(file);
        chat.CoverId = image.Id;
        await repository.UpdateAsync(chat);

        await notifier.ChatCoverUpdated(new ChatCoverUpdatedEvent(chatId, chat.CoverId));
    }

    public async Task UpdateChatNameAsync(int userId, int chatId, string newName)
    {
        ChatModel chat = await CheckUserIsMemberAsync(userId, chatId, true);
        chat.Name = newName;
        await repository.UpdateAsync(chat);

        await notifier.ChatNameUpdated(new ChatNameUpdatedEvent(chatId, chat.Name));
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
        if (message.SenderId != userId)
            throw ResponseFactory.Create<ForbiddenResponse>(["User is not the sender of the message"]);
        
        await messageReadService.DeleteByMessageIdAsync(messageId);
        
        // Now delete the message
        await messageService.DeleteAsync(messageId);

        await notifier.MessageDeleted(new MessageDeletedEvent(
            message.ChatId,
            message.Id,
            userId
        ));
    }

    public async Task<Message> EditMessageAsync(int userId, int messageId, EditMessageDTO dto)
    {
        Message message = await messageService.GetByIdValidatedAsync(messageId);
        if (message.SenderId != userId)
            throw ResponseFactory.Create<ForbiddenResponse>(["User is not the sender of the message"]);
        
        message.TextContent = dto.TextContent;
        Message updated = await messageService.UpdateAsync(message);

        await notifier.MessageUpdated(new MessageUpdatedEvent(
            message.ChatId,
            message.Id,
            userId,
            dto.TextContent,
            DateTime.UtcNow
        ));

        return updated;
    }

    public async Task<ChatDTO> GetChatInfoAsync(int userId, int chatId)
    {
        await CheckUserIsMemberAsync(userId, chatId);
        ChatModel chat = await repository.SnInclude(c => c.Members)
            .SnInclude(c => c.Admins)
            .SnInclude(c => c.Cover).GetByIdValidatedAsync(chatId);
        ChatDTO dto = new()
        {
            Name = chat.Name,
            IsGroup = chat.IsGroup,
            CoverId = chat.CoverId,
            CreatorId = chat.CreatorId,
            UserIds = chat.Members.Select(u => u.Id).ToArray(),
            AdminIds = chat.Admins.Select(a => a.Id).ToArray()
        };
        return dto;
    }

    // TODO: Create some service for cleaning up old reads
    public async Task ReadMessagesAsync(int chatId, int userId, IEnumerable<int> messageIds)
    {
        await CheckUserIsMemberAsync(userId, chatId);

        List<int> validMessageIds = (await messageService.GetMessagesByIdsAsync(messageIds, chatId)).ToList();

        if (validMessageIds.Count == 0)
            throw ResponseFactory.Create<ForbiddenResponse>(["No valid messages to mark as read"]);

        IEnumerable<int> existingReadIds = await messageReadService.GetReadMessageIdsAsync(userId, validMessageIds);
        HashSet<int> existingReadSet = new(existingReadIds);

        DateTime now = DateTime.UtcNow;

        List<MessageRead> newReads = [];
        List<MessageRead> updatedReads = [];

        foreach (int messageId in validMessageIds)
        {
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
                if (existing != null && existing.ReadAt == null)
                {
                    existing.ReadAt = now;
                    updatedReads.Add(existing);
                }
            }
        }

        if (newReads.Count > 0)
        {
            await messageReadService.AddRangeAsync(newReads);
        }

        foreach (MessageRead updatedRead in updatedReads)
        {
            await messageReadService.UpdateAsync(updatedRead);
        }

        if (newReads.Count > 0 || updatedReads.Count > 0)
        {
            await notifier.MessagesRead(new MessagesReadEvent(
                chatId,
                userId,
                validMessageIds,
                now
            ));
        }
    }

    public async Task ReadAllMessagesAsync(int userId, int chatId)
    {
        await CheckUserIsMemberAsync(userId, chatId);
        ChatModel chat = await repository.SnInclude(c => c.Messages).GetByIdValidatedAsync(chatId);
        
        if (chat.Messages.Count == 0)
            return;

        List<int> messageIds = chat.Messages.Select(m => m.Id).ToList();
        IEnumerable<int> existingReadIds = await messageReadService.GetReadMessageIdsAsync(userId, messageIds);
        HashSet<int> existingReadSet = new(existingReadIds);

        DateTime now = DateTime.UtcNow;
        List<MessageRead> newReads = [];
        List<MessageRead> updatedReads = [];

        foreach (Message message in chat.Messages)
        {
            if (!existingReadSet.Contains(message.Id))
            {
                newReads.Add(new MessageRead
                {
                    MessageId = message.Id,
                    UserId = userId,
                    ReadAt = now
                });
            }
            else
            {
                MessageRead? existingRead = await messageReadService.GetReadRecordAsync(userId, message.Id);
                if (existingRead != null && existingRead.ReadAt == null)
                {
                    existingRead.ReadAt = now;
                    updatedReads.Add(existingRead);
                }
            }
        }

        if (newReads.Count > 0)
        {
            await messageReadService.AddRangeAsync(newReads);
        }

        foreach (MessageRead updatedRead in updatedReads)
        {
            await messageReadService.UpdateAsync(updatedRead);
        }

        if (newReads.Count > 0 || updatedReads.Count > 0)
        {
            List<int> readMessageIds = newReads.Select(mr => mr.MessageId)
                .Concat(updatedReads.Select(mr => mr.MessageId))
                .ToList();
            
            await notifier.MessagesRead(new MessagesReadEvent(
                chatId,
                userId,
                readMessageIds,
                now
            ));
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

        await notifier.UserAdded(new UserAddedToChatEvent(chatId, userId));
    }

    public async Task RemoveUserFromChat(int initiatorId, int userId, int chatId)
    {
        ChatModel chat = await CheckUserCanManageChatAsync(initiatorId, chatId);
        if (chat.Members.Any(u => u.Id == userId))
        {
            chat.Members.Remove(await userService.GetByIdValidatedAsync(userId));
            await repository.UpdateAsync(chat);
            return;
        }

        if (chat.CreatorId != initiatorId || chat.Admins.All(a => a.Id != userId))
            throw ResponseFactory.Create<ForbiddenResponse>(["User is not a regular member of the chat"]);
        chat.Admins.Remove(await userService.GetByIdValidatedAsync(userId));
        await repository.UpdateAsync(chat);

        await notifier.UserRemoved(new UserRemovedFromChatEvent(chatId, userId));
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
        await repository.UpdateAsync(chat);

        await notifier.UserRemoved(new UserRemovedFromChatEvent(chatId, userId));
    }

    public async Task<ChatModel> CreateChatAsync(int userId, CreateChatDTO dto)
    {
        User creator = await userService.GetByIdValidatedAsync(userId);
        
        if (dto.IsGroup)
        {
            ChatModel groupChat = new()
            {
                Name = dto.Name,
                IsGroup = dto.IsGroup,
                CoverId = dto.CoverId,
                CreatorId = userId,
                Members = new List<User> { creator }
            };
            await repository.AddAsync(groupChat);
            return await repository.UpdateAsync(groupChat);
        }
        
        if (dto.UserId == null)
            throw ResponseFactory.Create<ForbiddenResponse>(["Personal chat requires another user"]);
        
        if (dto.UserId == userId)
            throw ResponseFactory.Create<BadRequestResponse>(["Cannot create personal chat with yourself"]);
        
        User otherUser = await userService.GetByIdValidatedAsync((int)dto.UserId);
        
        ChatModel? existingChat = await repository.FindPersonalChatBetweenUsersAsync(userId, (int)dto.UserId);
        if (existingChat != null)
            throw ResponseFactory.Create<BadRequestResponse>(["Personal chat between these users already exists"]);
        
        await CheckCanMessageUserAsync(userId, (int)dto.UserId);
        
        ChatModel personalChat = new()
        {
            Name = dto.Name,
            IsGroup = dto.IsGroup,
            CoverId = dto.CoverId,
            CreatorId = userId,
            Members = new List<User> { creator, otherUser }
        };
        await repository.AddAsync(personalChat);
        return await repository.UpdateAsync(personalChat);
    }

    public async Task<Message> SendMessageAsync(int userId, int chatId, MessageDTO message)
    {
        await CheckUserIsMemberAsync(userId, chatId);
        Message created = await messageService.CreateAsync(chatId, userId, message);

        // Load sender information for group chats
        User sender = await userService.GetByIdValidatedAsync(userId);
        string senderName = sender.UserName ?? $"{sender.FirstName} {sender.LastName}".Trim();

        await notifier.MessageCreated(new MessageCreatedEvent(
            created.Id,
            created.ChatId,
            created.SenderId,
            senderName,
            sender.AvatarImageId,
            sender.PublicIdentifier,
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

    public async Task<IEnumerable<ChatListItemDTO>> GetUserChatsAsync(int userId)
    {
        var chats = await repository
            .Query()
            .Where(c => c.Members.Any(m => m.Id == userId))
            .Include(c => c.Members)
            .OrderByDescending(c => c.Id)
            .ToListAsync();

        List<int> chatIds = chats.Select(c => c.Id).ToList();

        Dictionary<int, LastMessageDTO?> lastMessagesDict = await messageService.GetLastMessagesForChatsAsync(chatIds);

        return chats.Select(chat => new ChatListItemDTO
        {
            Id = chat.Id,
            Name = chat.Name,
            IsGroup = chat.IsGroup,
            CoverId = chat.CoverId,
            CreatorId = chat.CreatorId,
            UserIds = chat.Members.Select(m => m.Id).ToArray(),
            LastMessage = lastMessagesDict.GetValueOrDefault(chat.Id)
        });
    }

    public async Task DeleteChatAsync(int userId, int chatId)
    {
        ChatModel chat = await repository
            .SnInclude(c => c.Members)
            .SnInclude(c => c.Messages)
            .GetByIdValidatedAsync(chatId);

        // For group chats, only the creator can delete
        // For personal chats, either participant can delete
        if (chat.IsGroup)
        {
            if (chat.CreatorId != userId)
                throw ResponseFactory.Create<ForbiddenResponse>(["Only the chat creator can delete the chat"]);
        }
        else
        {
            // Check if user is a member of the personal chat
            bool isMember = chat.Members != null && chat.Members.Any(m => m.Id == userId);
            if (!isMember)
                throw ResponseFactory.Create<ForbiddenResponse>(["User is not a member of the chat"]);
        }

        // Delete all message reads for messages in this chat
        if (chat.Messages != null && chat.Messages.Any())
        {
            List<int> messageIds = chat.Messages.Select(m => m.Id).ToList();
            foreach (int messageId in messageIds)
            {
                await messageReadService.DeleteByMessageIdAsync(messageId);
            }
        }

        // Delete the chat (messages will be deleted by cascade)
        await DeleteAsync(chat);

        await notifier.ChatDeleted(new ChatDeletedEvent(chatId, userId));
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
            .SnInclude(c => c.Members)
            .SnInclude(c => c.Admins)
            .SnInclude(c => c.Creator)
            .GetByIdValidatedAsync(chatId);

        if (!chat.IsGroup && blockIfPrivate)
            throw ResponseFactory.Create<ForbiddenResponse>(["This cannot be done in the personal chat"]);

        HashSet<int> userIds = chat.Members.Select(u => u.Id).ToHashSet();
        return userIds.Contains(userId)
            ? chat
            : throw ResponseFactory.Create<ForbiddenResponse>(["User is not a member of the chat"]);
    }

    private async Task CheckCanMessageUserAsync(int senderId, int recipientId)
    {
        User recipient = await userService.GetByIdValidatedAsync(recipientId);
        
        var recipientWithSettings = await userRepository
            .SnInclude(u => u.Settings)
            .ThenInclude(s => s.UserPrivacy)
            .GetByIdValidatedAsync(recipientId);
        
        if (recipientWithSettings?.Settings?.UserPrivacy == null)
            throw ResponseFactory.Create<BadRequestResponse>(["Recipient privacy settings not found"]);

        int whichCanMessageId = recipientWithSettings.Settings.UserPrivacy.WhichCanMessageId;

        if (whichCanMessageId == 1) 
            return;

        if (whichCanMessageId == 3)
            throw ResponseFactory.Create<ForbiddenResponse>(["This user does not accept messages"]);

        if (whichCanMessageId == 2) 
        {
            IEnumerable<User> mutualFollows = await userFollowService.GetMutualFollowsAsync(recipientId);
            bool isMutualFollow = mutualFollows.Any(u => u.Id == senderId);
            if (!isMutualFollow)
                throw ResponseFactory.Create<ForbiddenResponse>(["You can only message users who follow you back"]);
        }
    }
}