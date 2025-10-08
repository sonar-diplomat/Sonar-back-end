using Application.Abstractions.Interfaces.Repository.Chat;
using Application.Abstractions.Interfaces.Services;
using ChatModel = Entities.Models.Chat.Chat;

namespace Application.Services.Chat;

public class ChatService(IChatRepository repository) : GenericService<ChatModel>(repository), IChatService
{
}
