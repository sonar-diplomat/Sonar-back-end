using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.Access;
using Entities.Models;
using Entities.Models.Access;

namespace Application.Services.Access
{
    public class VisibilityStatusService : IVisibilityStatusService
    {
        private readonly IVisibilityStatusRepository _repository;

        public VisibilityStatusService(IVisibilityStatusRepository repository)
        {
            _repository = repository;
        }

        public Task<VisibilityStatus> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<VisibilityStatus>> GetAllAsync() => throw new NotImplementedException();
        public Task<VisibilityStatus> CreateAsync(VisibilityStatus entity) => throw new NotImplementedException();
        public Task<VisibilityStatus> UpdateAsync(VisibilityStatus entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

