using Application.Abstractions.Interfaces.Repository.Chat;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.DTOs.Chat;
using Application.Extensions;
using Application.Response;
using Entities.Models.Chat;
using Entities.Models.File;
using Microsoft.AspNetCore.Http;
using ChatModel = Entities.Models.Chat.Chat;

namespace Application.Services.Chat;

public class ChatService(
    IChatRepository repository,
    IImageFileService imageFileService,
    IMessageService messageService,
    IMessageReadService messageReadService
) : GenericService<ChatModel>(repository), IChatService
{
    public async Task UpdateChatCoverAsync(int userId, int chatId, IFormFile file)
    {
        ChatModel chat = await CheckUserIsMemberAsync(userId, chatId);
        ImageFile image = await imageFileService.UploadFileAsync(file);
        chat.CoverId = image.Id;
        await repository.UpdateAsync(chat);
    }

    public async Task UpdateChatNameAsync(int userId, int chatId, string newName)
    {
        ChatModel chat = await CheckUserIsMemberAsync(userId, chatId);
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
        ChatModel chat = await repository.Include(c => c.Users)
            .Include(c => c.Cover).GetByIdValidatedAsync(chatId);
        ChatDTO dto = new()
        {
            Name = chat.Name,
            Cover = chat.Cover,
            UserIds = chat.Users.Select(u => u.Id).ToArray()
        };
        return dto;
    }

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
            await messageReadService.AddRangeAsync(newReads);
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

    public async Task<Message> SendMessageAsync(int userId, int chatId, MessageDTO message)
    {
        chatId = (await CheckUserIsMemberAsync(userId, chatId)).Id;

        return await messageService.CreateAsync(userId, chatId, message);
    }

    private async Task<ChatModel> CheckUserIsMemberAsync(int userId, int chatId)
    {
        ChatModel chat = await repository.Include(c => c.Users).GetByIdValidatedAsync(chatId);
        HashSet<int> userIds = chat.Users.Select(u => u.Id).ToHashSet();
        return userIds.Contains(userId)
            ? chat
            : throw ResponseFactory.Create<ForbiddenResponse>(["User is not a member of the chat"]);
    }
}