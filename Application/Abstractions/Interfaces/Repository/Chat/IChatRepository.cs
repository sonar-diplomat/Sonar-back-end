using ChatModel = Entities.Models.Chat.Chat;

namespace Application.Abstractions.Interfaces.Repository.Chat;

public interface IChatRepository : IGenericRepository<ChatModel>
{
}