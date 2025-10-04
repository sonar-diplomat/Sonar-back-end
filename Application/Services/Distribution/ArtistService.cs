using Application.Abstractions.Interfaces.Repository.Distribution;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.Distribution;

namespace Application.Services.Distribution
{
    public class ArtistService(IArtistRepository repository) : IArtistService
    {


        public Task<Artist> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<Artist>> GetAllAsync() => throw new NotImplementedException();
        public Task<Artist> CreateAsync(Artist entity) => throw new NotImplementedException();
        public Task<Artist> UpdateAsync(Artist entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

