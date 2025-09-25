using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.Distribution;
using Entities.Models;
using Entities.Models.Distribution;

namespace Application.Services.Distribution
{
    public class LicenseService : ILicenseService
    {
        private readonly ILicenseRepository _repository;

        public LicenseService(ILicenseRepository repository)
        {
            _repository = repository;
        }

        public Task<License> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<License>> GetAllAsync() => throw new NotImplementedException();
        public Task<License> CreateAsync(License entity) => throw new NotImplementedException();
        public Task<License> UpdateAsync(License entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

