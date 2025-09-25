using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.ClientSettings;

namespace Application.Abstractions.Interfaces.Services
{
    public interface INotificationTypeService
    {
        Task<NotificationType> GetByIdAsync(int id);
        Task<IEnumerable<NotificationType>> GetAllAsync();
        Task<NotificationType> CreateAsync(NotificationType notificationType);
        Task<NotificationType> UpdateAsync(NotificationType notificationType);
        Task<bool> DeleteAsync(int id);
    }
}

