using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.Distribution;
using Entities.Models;
using Entities.Models.Distribution;

namespace Application.Services.Distribution
{
    public class DistributorSessionService : IDistributorSessionService
    {
        private readonly IDistributorSessionRepository _repository;

        public DistributorSessionService(IDistributorSessionRepository repository)
        {
            _repository = repository;
        }

        public Task<DistributorSession> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<DistributorSession>> GetAllAsync() => throw new NotImplementedException();
        public Task<DistributorSession> CreateAsync(DistributorSession entity) => throw new NotImplementedException();
        public Task<DistributorSession> UpdateAsync(DistributorSession entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

