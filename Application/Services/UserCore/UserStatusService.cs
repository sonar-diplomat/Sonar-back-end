using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.UserCore;
using Entities.Models;
using Entities.Models.UserCore;

namespace Application.Services.UserCore
{
    public class UserStatusService : IUserStatusService
    {
        private readonly IUserStatusRepository _repository;

        public UserStatusService(IUserStatusRepository repository)
        {
            _repository = repository;
        }

        public Task<UserStatus> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<UserStatus>> GetAllAsync() => throw new NotImplementedException();
        public Task<UserStatus> CreateAsync(UserStatus entity) => throw new NotImplementedException();
        public Task<UserStatus> UpdateAsync(UserStatus entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

