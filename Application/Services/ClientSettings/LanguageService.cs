using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.Client;
using Entities.Models;
using Entities.Models.ClientSettings;

namespace Application.Services.ClientSettings
{
    public class LanguageService : ILanguageService
    {
        private readonly ILanguageRepository _repository;

        public LanguageService(ILanguageRepository repository)
        {
            _repository = repository;
        }

        public Task<Language> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<Language>> GetAllAsync() => throw new NotImplementedException();
        public Task<Language> CreateAsync(Language entity) => throw new NotImplementedException();
        public Task<Language> UpdateAsync(Language entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

