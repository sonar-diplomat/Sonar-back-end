using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.Chat;
using Entities.Models;
using Entities.Models.Chat;

namespace Application.Services.Chat
{
    public class MessageReadService : IMessageReadService
    {
        private readonly IMessageReadRepository _repository;

        public MessageReadService(IMessageReadRepository repository)
        {
            _repository = repository;
        }

        public Task<MessageRead> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<MessageRead>> GetAllAsync() => throw new NotImplementedException();
        public Task<MessageRead> CreateAsync(MessageRead entity) => throw new NotImplementedException();
        public Task<MessageRead> UpdateAsync(MessageRead entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

