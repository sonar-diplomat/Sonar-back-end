using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.Access;
using Entities.Models;
using Entities.Models.Access;

namespace Application.Services.Access
{
    public class VisibilityStateService : IVisibilityStateService
    {
        private readonly IVisibilityStateRepository _repository;

        public VisibilityStateService(IVisibilityStateRepository repository)
        {
            _repository = repository;
        }

        public Task<VisibilityState> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<VisibilityState>> GetAllAsync() => throw new NotImplementedException();
        public Task<VisibilityState> CreateAsync(VisibilityState entity) => throw new NotImplementedException();
        public Task<VisibilityState> UpdateAsync(VisibilityState entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

