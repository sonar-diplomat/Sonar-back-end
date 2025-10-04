using Application.Abstractions.Interfaces.Repository.UserExperience;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.UserExperience;

namespace Application.Services.UserExperience
{
    public class GiftService(IGiftRepository repository) : IGiftService
    {

        public Task<Gift> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<Gift>> GetAllAsync() => throw new NotImplementedException();
        public Task<Gift> CreateAsync(Gift entity) => throw new NotImplementedException();
        public Task<Gift> UpdateAsync(Gift entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

