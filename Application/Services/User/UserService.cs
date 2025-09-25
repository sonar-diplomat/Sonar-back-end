using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.User;
using Entities.Models;

namespace Application.Services.User
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public Task<Entities.Models.User.User> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<Entities.Models.User.User>> GetAllAsync() => throw new NotImplementedException();
        public Task<Entities.Models.User.User> CreateAsync(Entities.Models.User.User entity) => throw new NotImplementedException();
        public Task<Entities.Models.User.User> UpdateAsync(Entities.Models.User.User entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

