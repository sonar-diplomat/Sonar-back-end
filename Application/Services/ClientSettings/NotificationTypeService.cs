using Application.Abstractions.Interfaces.Repository.Client;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.ClientSettings;

namespace Application.Services.ClientSettings;

public class NotificationTypeService(INotificationTypeRepository repository) : INotificationTypeService
{
    private readonly INotificationTypeRepository repository = repository;

    public Task<NotificationType> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<NotificationType>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<NotificationType> CreateAsync(NotificationType entity)
    {
        throw new NotImplementedException();
    }

    public Task<NotificationType> UpdateAsync(NotificationType entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}