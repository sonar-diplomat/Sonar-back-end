using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.User;
using Entities.Models;
using Entities.Models.User;

namespace Application.Services.User
{
    public class UserSessionService : IUserSessionService
    {
        private readonly IUserSessionRepository _repository;

        public UserSessionService(IUserSessionRepository repository)
        {
            _repository = repository;
        }

        public Task<UserSession> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<UserSession>> GetAllAsync() => throw new NotImplementedException();
        public Task<UserSession> CreateAsync(UserSession entity) => throw new NotImplementedException();
        public Task<UserSession> UpdateAsync(UserSession entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

