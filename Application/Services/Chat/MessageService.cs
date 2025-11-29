using System.Globalization;
using Application.Abstractions.Interfaces.Repository.Chat;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.DTOs.Chat;
using Entities.Models.Chat;
using Microsoft.EntityFrameworkCore;

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

    public async Task<CursorPageDTO<MessageDTO>> GetMessagesWithCursorAsync(int chatId, int? cursor, int take = 50)
    {
        take = Math.Clamp(take, 1, 200);

        IQueryable<Message> q = repository
            .Query()
            .Where(m => m.ChatId == chatId);

        if (cursor.HasValue)
        {
            var c = await repository.Query()
                .Where(m => m.Id == cursor.Value && m.ChatId == chatId)
                .Select(m => new { m.CreatedAt, m.Id })
                .FirstOrDefaultAsync();

            if (c is null)
                return new CursorPageDTO<MessageDTO>
                {
                    Items = [],
                    NextCursor = null
                };

            q = q.Where(m =>
                m.CreatedAt < c.CreatedAt ||
                (m.CreatedAt == c.CreatedAt && m.Id < c.Id));
        }

        var raw = await q
            .OrderByDescending(m => m.CreatedAt)
            .ThenByDescending(m => m.Id)
            .Take(take + 1)
            .Select(m => new
            {
                m.Id,
                m.CreatedAt,
                DTO = new MessageDTO
                {
                    Id = m.Id,
                    CreatedAt = m.CreatedAt,
                    SenderId = m.SenderId,
                    TextContent = m.TextContent,
                    ReplyMessageId = m.ReplyMessageId
                }
            })
            .ToListAsync();

        bool hasMore = raw.Count > take;
        if (hasMore) raw.RemoveAt(raw.Count - 1);

        raw.Reverse();

        string? nextCursor = null;
        if (hasMore && raw.Count > 0)
            nextCursor = raw.First().Id.ToString(CultureInfo.InvariantCulture);

        return new CursorPageDTO<MessageDTO>
        {
            Items = raw.Select(x => x.DTO),
            NextCursor = nextCursor
        };
    }
}