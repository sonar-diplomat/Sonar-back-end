using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.Client;
using Entities.Models;
using Entities.Models.ClientSettings;

namespace Application.Services.ClientSettings
{
    public class ThemeService : IThemeService
    {
        private readonly IThemeRepository _repository;

        public ThemeService(IThemeRepository repository)
        {
            _repository = repository;
        }

        public Task<Theme> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<Theme>> GetAllAsync() => throw new NotImplementedException();
        public Task<Theme> CreateAsync(Theme entity) => throw new NotImplementedException();
        public Task<Theme> UpdateAsync(Theme entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

