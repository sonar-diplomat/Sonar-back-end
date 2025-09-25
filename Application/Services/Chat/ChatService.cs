using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.Chat;
using Entities.Models;

namespace Application.Services.Chat
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _repository;

        public ChatService(IChatRepository repository)
        {
            _repository = repository;
        }

        public Task<Entities.Models.Chat.Chat> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<Entities.Models.Chat.Chat>> GetAllAsync() => throw new NotImplementedException();
        public Task<Entities.Models.Chat.Chat> CreateAsync(Entities.Models.Chat.Chat chat) => throw new NotImplementedException();
        public Task<Entities.Models.Chat.Chat> UpdateAsync(Entities.Models.Chat.Chat chat) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

