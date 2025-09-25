using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.Distribution;
using Entities.Models;
using Entities.Models.Distribution;

namespace Application.Services.Distribution
{
    public class DistributorService : IDistributorService
    {
        private readonly IDistributorRepository _repository;

        public DistributorService(IDistributorRepository repository)
        {
            _repository = repository;
        }

        public Task<Distributor> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<Distributor>> GetAllAsync() => throw new NotImplementedException();
        public Task<Distributor> CreateAsync(Distributor entity) => throw new NotImplementedException();
        public Task<Distributor> UpdateAsync(Distributor entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

