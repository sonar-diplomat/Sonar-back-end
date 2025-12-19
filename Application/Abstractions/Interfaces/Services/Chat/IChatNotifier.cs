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
    Task MessageUpdated(MessageUpdatedEvent payload);
    
    Task ChatDeleted(ChatDeletedEvent payload);
}

public sealed record MessageCreatedEvent(
    int Id,
    int ChatId,
    int SenderId,
    string SenderName,
    int SenderAvatarImageId,
    string SenderPublicIdentifier,
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

public sealed record MessageUpdatedEvent(
    int ChatId,
    int MessageId,
    int SenderId,
    string TextContent,
    DateTime UpdatedAtUtc
);

public sealed record ChatNameUpdatedEvent(int ChatId, string Name);

public sealed record ChatCoverUpdatedEvent(int ChatId, int CoverId);

public sealed record UserAddedToChatEvent(int ChatId, int UserId);

public sealed record UserRemovedFromChatEvent(int ChatId, int UserId);

public sealed record ChatDeletedEvent(int ChatId, int InitiatorId);