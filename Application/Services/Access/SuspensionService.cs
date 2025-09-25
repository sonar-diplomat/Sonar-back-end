using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.Access;
using Entities.Models;
using Entities.Models.Access;

namespace Application.Services.Access
{
    public class SuspensionService : ISuspensionService
    {
        private readonly ISuspensionRepository _repository;

        public SuspensionService(ISuspensionRepository repository)
        {
            _repository = repository;
        }

        public Task<Suspension> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<Suspension>> GetAllAsync() => throw new NotImplementedException();
        public Task<Suspension> CreateAsync(Suspension entity) => throw new NotImplementedException();
        public Task<Suspension> UpdateAsync(Suspension entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

