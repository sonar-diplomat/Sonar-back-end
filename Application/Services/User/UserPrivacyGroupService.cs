using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.User;
using Entities.Models;
using Entities.Models.User;

namespace Application.Services.User
{
    public class UserPrivacyGroupService : IUserPrivacyGroupService
    {
        private readonly IUserPrivacyGroupRepository _repository;

        public UserPrivacyGroupService(IUserPrivacyGroupRepository repository)
        {
            _repository = repository;
        }

        public Task<UserPrivacyGroup> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<UserPrivacyGroup>> GetAllAsync() => throw new NotImplementedException();
        public Task<UserPrivacyGroup> CreateAsync(UserPrivacyGroup entity) => throw new NotImplementedException();
        public Task<UserPrivacyGroup> UpdateAsync(UserPrivacyGroup entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

