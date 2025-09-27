using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.UserCore;
using Entities.Models;
using Entities.Models.UserCore;

namespace Application.Services.UserCore
{
    public class UserStateService : IUserStateService
    {
        private readonly IUserStateRepository _repository;

        public UserStateService(IUserStateRepository repository)
        {
            _repository = repository;
        }

        public Task<UserState> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<UserState>> GetAllAsync() => throw new NotImplementedException();
        public Task<UserState> CreateAsync(UserState entity) => throw new NotImplementedException();
        public Task<UserState> UpdateAsync(UserState entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

