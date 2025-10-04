using Application.Abstractions.Interfaces.Repository.Chat;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.Chat;

namespace Application.Services.Chat
{
    public class MessageService(IMessageRepository repository) : GenericService<Message>(repository), IMessageService
    {

    }
}

