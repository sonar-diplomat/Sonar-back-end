using ChatModel = Entities.Models.Chat.Chat;

namespace Application.Abstractions.Interfaces.Repository.Chat;

public interface IChatRepository : IGenericRepository<ChatModel>
{
    /// <summary>
    /// Reload and mutate an existing chat under a database lock. Returns only after
    /// commit; callers must send notifications outside the callback.
    /// </summary>
    Task<TResult> ExecuteLockedAsync<TResult>(int chatId, Func<Task<TResult>> operation);

    Task<ChatModel?> FindPersonalChatBetweenUsersAsync(int userId1, int userId2);
}
