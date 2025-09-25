using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.Distribution;
using Entities.Models;
using Entities.Models.Distribution;

namespace Application.Services.Distribution
{
    public class ArtistService : IArtistService
    {
        private readonly IArtistRepository _repository;

        public ArtistService(IArtistRepository repository)
        {
            _repository = repository;
        }

        public Task<Artist> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<Artist>> GetAllAsync() => throw new NotImplementedException();
        public Task<Artist> CreateAsync(Artist entity) => throw new NotImplementedException();
        public Task<Artist> UpdateAsync(Artist entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

