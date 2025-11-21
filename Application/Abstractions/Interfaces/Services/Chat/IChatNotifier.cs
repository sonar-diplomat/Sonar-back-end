namespace Application.Abstractions.Interfaces.Services;

public interface IChatNotifier
{
    Task MessageCreated(MessageCreatedEvent payload);
    Task MessagesRead(MessagesReadEvent payload);

    Task ChatNameUpdated(ChatNameUpdatedEvent payload);
    Task ChatCoverUpdated(ChatCoverUpdatedEvent payload);

    Task UserAdded(UserAddedToChatEvent payload);
    Task UserRemoved(UserRemovedFromChatEvent payload);

    Task MessageDeleted(MessageDeletedEvent payload);
}

public sealed record MessageCreatedEvent(
    int Id,
    int ChatId,
    int SenderId,
    string TextContent,
    int? ReplyMessageId,
    DateTime CreatedAtUtc
);

public sealed record MessagesReadEvent(
    int ChatId,
    int UserId,
    IReadOnlyCollection<int> MessageIds,
    DateTime ReadAtUtc
);

public sealed record MessageDeletedEvent(
    int ChatId,
    int MessageId,
    int InitiatorId
);

public sealed record ChatNameUpdatedEvent(int ChatId, string Name);

public sealed record ChatCoverUpdatedEvent(int ChatId, int CoverId);

public sealed record UserAddedToChatEvent(int ChatId, int UserId);

public sealed record UserRemovedFromChatEvent(int ChatId, int UserId);