using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.Client;
using Entities.Models;
using Entities.Models.ClientSettings;

namespace Application.Services.ClientSettings
{
    public class NotificationTypeService : INotificationTypeService
    {
        private readonly INotificationTypeRepository _repository;

        public NotificationTypeService(INotificationTypeRepository repository)
        {
            _repository = repository;
        }

        public Task<NotificationType> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<NotificationType>> GetAllAsync() => throw new NotImplementedException();
        public Task<NotificationType> CreateAsync(NotificationType entity) => throw new NotImplementedException();
        public Task<NotificationType> UpdateAsync(NotificationType entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

