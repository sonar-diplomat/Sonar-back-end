using System.Globalization;
using Application.Abstractions.Interfaces.Repository.Chat;
using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.DTOs.Chat;
using Application.Response;
using Entities.Models.Chat;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Chat;

public class MessageService(
    IMessageRepository repository,
    IMessageReadService messageReadService,
    IUserRepository userRepository
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

        // Load all MessageRead records for these messages
        List<int> messageIds = raw.Select(x => x.Id).ToList();
        var messageReads = await messageReadService.GetReadRecordsByMessageIdsAsync(messageIds);

        // Group reads by message ID
        var readsByMessageId = messageReads
            .GroupBy(mr => mr.MessageId)
            .ToDictionary(g => g.Key, g => g.ToList());

        // Load sender information for all unique senders
        List<int> uniqueSenderIds = raw.Select(x => x.SenderId).Distinct().ToList();
        var senders = await userRepository
            .Query()
            .Where(u => uniqueSenderIds.Contains(u.Id))
            .Select(u => new
            {
                u.Id,
                u.UserName,
                u.FirstName,
                u.LastName,
                u.AvatarImageId,
                u.PublicIdentifier
            })
            .ToListAsync();

        var senderDict = senders.ToDictionary(s => s.Id, s => new
        {
            Name = s.UserName ?? $"{s.FirstName} {s.LastName}".Trim(),
            AvatarImageId = s.AvatarImageId,
            PublicIdentifier = s.PublicIdentifier
        });

        // Map to DTOs with read records and sender information
        var dtos = raw.Select(x =>
        {
            var senderInfo = senderDict.GetValueOrDefault(x.SenderId);
            return new MessageDTO
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                SenderId = x.SenderId,
                SenderName = senderInfo?.Name,
                SenderAvatarImageId = senderInfo?.AvatarImageId,
                SenderPublicIdentifier = senderInfo?.PublicIdentifier,
                TextContent = x.TextContent,
                ReplyMessageId = x.ReplyMessageId,
                ReadBy = readsByMessageId.GetValueOrDefault(x.Id)
            };
        }).ToList();

        return new CursorPageDTO<MessageDTO>
        {
            Items = dtos,
            NextCursor = nextCursor
        };
    }

    public async Task<Dictionary<int, LastMessageDTO?>> GetLastMessagesForChatsAsync(IEnumerable<int> chatIds)
    {
        List<int> chatIdsList = chatIds.ToList();
        if (chatIdsList.Count == 0)
            return new Dictionary<int, LastMessageDTO?>();

        var lastMessages = await repository
            .Query()
            .Where(m => chatIdsList.Contains(m.ChatId))
            .GroupBy(m => m.ChatId)
            .Select(g => new
            {
                ChatId = g.Key,
                LastMessage = g.OrderByDescending(m => m.CreatedAt)
                    .ThenByDescending(m => m.Id)
                    .Select(m => new
                    {
                        m.TextContent,
                        m.CreatedAt
                    })
                    .FirstOrDefault()
            })
            .ToListAsync();

        var result = new Dictionary<int, LastMessageDTO?>();
        
        foreach (int chatId in chatIdsList)
        {
            var lastMsg = lastMessages.FirstOrDefault(lm => lm.ChatId == chatId);
            result[chatId] = lastMsg?.LastMessage != null
                ? new LastMessageDTO
                {
                    TextContent = lastMsg.LastMessage.TextContent,
                    CreatedAt = lastMsg.LastMessage.CreatedAt
                }
                : null;
        }

        return result;
    }
}