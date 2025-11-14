using Application.Abstractions.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;

namespace Sonar.Hubs;

public sealed class ChatNotifier(IHubContext<ChatHub> hub) : IChatNotifier
{
    public Task MessageCreated(MessageCreatedEvent payload)
    {
        return hub.Clients.Group(G(payload.ChatId)).SendAsync("message.created", payload);
    }

    public Task MessagesRead(MessagesReadEvent payload)
    {
        return hub.Clients.Group(G(payload.ChatId)).SendAsync("message.read", payload);
    }

    public Task ChatNameUpdated(ChatNameUpdatedEvent payload)
    {
        return hub.Clients.Group(G(payload.ChatId)).SendAsync("chat.name.updated", payload);
    }

    public Task ChatCoverUpdated(ChatCoverUpdatedEvent payload)
    {
        return hub.Clients.Group(G(payload.ChatId)).SendAsync("chat.cover.updated", payload);
    }

    public Task UserAdded(UserAddedToChatEvent payload)
    {
        return hub.Clients.Group(G(payload.ChatId)).SendAsync("chat.user.added", payload);
    }

    public Task UserRemoved(UserRemovedFromChatEvent payload)
    {
        return hub.Clients.Group(G(payload.ChatId)).SendAsync("chat.user.removed", payload);
    }

    public Task MessageDeleted(MessageDeletedEvent payload)
    {
        return hub.Clients.Group(G(payload.ChatId)).SendAsync("message.deleted", payload);
    }

    private static string G(int chatId)
    {
        return $"chat:{chatId}";
    }
}