using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.Chat;
using Entities.Models;
using Entities.Models.Chat;

namespace Application.Services.Chat
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _repository;

        public MessageService(IMessageRepository repository)
        {
            _repository = repository;
        }

        public Task<Message> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<Message>> GetAllAsync() => throw new NotImplementedException();
        public Task<Message> CreateAsync(Message message) => throw new NotImplementedException();
        public Task<Message> UpdateAsync(Message message) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

