using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.Music;
using Entities.Models;
using Entities.Models.Music;

namespace Application.Services.Music
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IPlaylistRepository _repository;

        public PlaylistService(IPlaylistRepository repository)
        {
            _repository = repository;
        }

        public Task<Playlist> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<Playlist>> GetAllAsync() => throw new NotImplementedException();
        public Task<Playlist> CreateAsync(Playlist playlist) => throw new NotImplementedException();
        public Task<Playlist> UpdateAsync(Playlist playlist) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

