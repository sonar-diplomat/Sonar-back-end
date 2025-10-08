using Application.Abstractions.Interfaces.Repository.Client;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.ClientSettings;

namespace Application.Services.ClientSettings;

public class NotificationTypeService(INotificationTypeRepository repository)
    : GenericService<NotificationType>(repository), INotificationTypeService
{
}
