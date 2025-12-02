using System.Globalization;
using Application.Abstractions.Interfaces.Repository.Chat;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.DTOs.Chat;
using Application.Response;
using Entities.Models.Chat;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Chat;

public class MessageService(
    IMessageRepository repository,
    IMessageReadService messageReadService
) : GenericService<Message>(repository), IMessageService
{
    public async Task<Message> CreateAsync(int chatId, int userId, MessageDTO dto)
    {
        Message message = new()
        {
            ChatId = chatId,
            SenderId = userId,
            TextContent = dto.TextContent,
            CreatedAt = DateTime.UtcNow,
            ReplyMessageId = null
        };
        
        if (dto.ReplyMessageId.HasValue)
        {
            Message reply = await GetByIdValidatedAsync(dto.ReplyMessageId.Value);
            if (reply.ChatId != chatId)
                throw ResponseFactory.Create<BadRequestResponse>("Reply message does not belong to the same chat");
            message.ReplyMessageId = reply.Id;
        }
        
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
                m.SenderId,
                m.TextContent,
                m.ReplyMessageId
            })
            .ToListAsync();

        bool hasMore = raw.Count > take;
        if (hasMore) raw.RemoveAt(raw.Count - 1);

        raw.Reverse();

        string? nextCursor = null;
        if (hasMore && raw.Count > 0)
            nextCursor = raw.First().Id.ToString(CultureInfo.InvariantCulture);

        List<int> messageIds = raw.Select(x => x.Id).ToList();
        var messageReads = await messageReadService.GetReadRecordsByMessageIdsAsync(messageIds);

        var readsByMessageId = messageReads
            .GroupBy(mr => mr.MessageId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var dtos = raw.Select(x => new MessageDTO
        {
            Id = x.Id,
            CreatedAt = x.CreatedAt,
            SenderId = x.SenderId,
            TextContent = x.TextContent,
            ReplyMessageId = x.ReplyMessageId,
            ReadBy = readsByMessageId.GetValueOrDefault(x.Id)
        }).ToList();

        return new CursorPageDTO<MessageDTO>
        {
            Items = dtos,
            NextCursor = nextCursor
        };
    }
}