using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.Music;
using Entities.Models;
using Entities.Models.Music;

namespace Application.Services.Music
{
    public class BlendService : IBlendService
    {
        private readonly IBlendRepository _repository;

        public BlendService(IBlendRepository repository)
        {
            _repository = repository;
        }

        public Task<Blend> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<Blend>> GetAllAsync() => throw new NotImplementedException();
        public Task<Blend> CreateAsync(Blend blend) => throw new NotImplementedException();
        public Task<Blend> UpdateAsync(Blend blend) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

