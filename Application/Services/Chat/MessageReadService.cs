using Application.Abstractions.Interfaces.Repository.Chat;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.Chat;

namespace Application.Services.Chat;

public class MessageReadService(IMessageReadRepository repository)
    : GenericService<MessageRead>(repository), IMessageReadService
{
}