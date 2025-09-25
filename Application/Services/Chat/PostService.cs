using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.Chat;
using Entities.Models;
using Entities.Models.Chat;

namespace Application.Services.Chat
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _repository;

        public PostService(IPostRepository repository)
        {
            _repository = repository;
        }

        public Task<Post> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<Post>> GetAllAsync() => throw new NotImplementedException();
        public Task<Post> CreateAsync(Post post) => throw new NotImplementedException();
        public Task<Post> UpdateAsync(Post post) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

