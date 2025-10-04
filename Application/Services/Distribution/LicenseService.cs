using Application.Abstractions.Interfaces.Repository.Distribution;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.Distribution;

namespace Application.Services.Distribution
{
    public class LicenseService(ILicenseRepository repository) : ILicenseService
    {


        public Task<License> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<License>> GetAllAsync() => throw new NotImplementedException();
        public Task<License> CreateAsync(License entity) => throw new NotImplementedException();
        public Task<License> UpdateAsync(License entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

