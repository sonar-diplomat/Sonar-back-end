using Application.Abstractions.Interfaces.Repository.Music;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.Music;

namespace Application.Services.Music
{
    public class BlendService(IBlendRepository repository) : IBlendService
    {

        public Task<Blend> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<Blend>> GetAllAsync() => throw new NotImplementedException();
        public Task<Blend> CreateAsync(Blend blend) => throw new NotImplementedException();
        public Task<Blend> UpdateAsync(Blend blend) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

