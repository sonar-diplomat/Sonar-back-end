using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.Library;
using Entities.Models;

namespace Application.Services.Library
{
    public class LibraryService : ILibraryService
    {
        private readonly ILibraryRepository _repository;

        public LibraryService(ILibraryRepository repository)
        {
            _repository = repository;
        }

        public Task<Entities.Models.Library.Library> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<Entities.Models.Library.Library>> GetAllAsync() => throw new NotImplementedException();
        public Task<Entities.Models.Library.Library> CreateAsync(Entities.Models.Library.Library entity) => throw new NotImplementedException();
        public Task<Entities.Models.Library.Library> UpdateAsync(Entities.Models.Library.Library entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

