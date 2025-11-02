using Application.Abstractions.Interfaces.Repository.Chat;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs.Chat;
using Entities.Models.Chat;

namespace Application.Services.Chat;

public class MessageService(
    IMessageRepository repository
) : GenericService<Message>(repository), IMessageService
{
    public async Task<Message> CreateAsync(int chatId, int userId, MessageDTO dto)
    {
        Message message = new()
        {
            ChatId = chatId,
            SenderId = userId,
            TextContent = dto.TextContent,
            CreatedAt = DateTime.UtcNow
        };
        if (dto.ReplyMessageId == null)
            return await repository.AddAsync(message);
        Message reply = await GetByIdValidatedAsync((int)dto.ReplyMessageId);
        if (reply.ChatId != chatId)
            throw new Exception("Reply message does not belong to the same chat");
        message.ReplyMessageId = reply.Id;
        return await repository.AddAsync(message);
    }

    public async Task<IEnumerable<int>> GetMessagesByIdsAsync(IEnumerable<int> messageIds, int chatId)
    {
        return await repository.GetMessagesByIdsAsync(messageIds, chatId);
    }
}