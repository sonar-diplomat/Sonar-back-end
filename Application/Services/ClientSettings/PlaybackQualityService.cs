using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.Client;
using Entities.Models;
using Entities.Models.ClientSettings;

namespace Application.Services.ClientSettings
{
    public class PlaybackQualityService : IPlaybackQualityService
    {
        private readonly IPlaybackQualityRepository _repository;

        public PlaybackQualityService(IPlaybackQualityRepository repository)
        {
            _repository = repository;
        }

        public Task<PlaybackQuality> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<PlaybackQuality>> GetAllAsync() => throw new NotImplementedException();
        public Task<PlaybackQuality> CreateAsync(PlaybackQuality entity) => throw new NotImplementedException();
        public Task<PlaybackQuality> UpdateAsync(PlaybackQuality entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

