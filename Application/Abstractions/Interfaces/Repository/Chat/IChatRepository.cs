using ChatModel = Entities.Models.Chat.Chat;

namespace Application.Abstractions.Interfaces.Repository.Chat;

public interface IChatRepository : IGenericRepository<ChatModel>
{
    Task<ChatModel?> FindPersonalChatBetweenUsersAsync(int userId1, int userId2);
}
