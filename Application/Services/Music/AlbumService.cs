using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.Music;
using Entities.Models;
using Entities.Models.Music;

namespace Application.Services.Music
{
    public class AlbumService : IAlbumService
    {
        private readonly IAlbumRepository _repository;

        public AlbumService(IAlbumRepository repository)
        {
            _repository = repository;
        }

        public Task<Album> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<Album>> GetAllAsync() => throw new NotImplementedException();
        public Task<Album> CreateAsync(Album album) => throw new NotImplementedException();
        public Task<Album> UpdateAsync(Album album) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

