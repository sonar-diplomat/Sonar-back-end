using Application.Abstractions.Interfaces.Repository.Chat;
using Application.Abstractions.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Sonar.Hubs;

public sealed class ChatNotifier(IHubContext<ChatHub> hub, IChatRepository chats) : IChatNotifier
{
    public Task MessageCreated(MessageCreatedEvent payload)
    {
        return SendToMembers(payload.ChatId, "message.created", payload);
    }

    public Task MessagesRead(MessagesReadEvent payload)
    {
        return SendToMembers(payload.ChatId, "message.read", payload);
    }

    public Task ChatNameUpdated(ChatNameUpdatedEvent payload)
    {
        return SendToMembers(payload.ChatId, "chat.name.updated", payload);
    }

    public Task ChatCoverUpdated(ChatCoverUpdatedEvent payload)
    {
        return SendToMembers(payload.ChatId, "chat.cover.updated", payload);
    }

    public Task UserAdded(UserAddedToChatEvent payload)
    {
        return SendToMembers(payload.ChatId, "chat.user.added", payload);
    }

    public Task UserRemoved(UserRemovedFromChatEvent payload)
    {
        // The removed member receives this control event, but no subsequent content.
        return SendToMembers(payload.ChatId, "chat.user.removed", payload, payload.UserId);
    }

    public Task MessageDeleted(MessageDeletedEvent payload)
    {
        return SendToMembers(payload.ChatId, "message.deleted", payload);
    }

    public Task MessageUpdated(MessageUpdatedEvent payload)
    {
        return SendToMembers(payload.ChatId, "message.updated", payload);
    }

    public Task ChatDeleted(ChatDeletedEvent payload, IReadOnlyCollection<int> memberIds)
    {
        return Send(payload.ChatId, memberIds, "chat.deleted", payload);
    }

    private async Task SendToMembers(int chatId, string name, object payload, int? removedUserId = null)
    {
        // Re-evaluate current membership for every event. An existing connection's
        // stale subscription cannot grant access after database membership is removed.
        var memberIds = await chats.Query().AsNoTracking()
            .Where(chat => chat.Id == chatId)
            .SelectMany(chat => chat.Members.Select(member => member.Id))
            .ToListAsync();
        if (removedUserId.HasValue) memberIds.Add(removedUserId.Value);
        await Send(chatId, memberIds, name, payload);
    }

    private Task Send(int chatId, IEnumerable<int> memberIds, string name, object payload)
    {
        var groups = memberIds.Distinct().Select(id => ChatGroups.ForMember(chatId, id)).ToList();
        return groups.Count == 0 ? Task.CompletedTask : hub.Clients.Groups(groups).SendAsync(name, payload);
    }
}
