using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.File;
using Entities.Models;

namespace Application.Services.File
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _repository;

        public FileService(IFileRepository repository)
        {
            _repository = repository;
        }

        public Task<Entities.Models.File.File> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<Entities.Models.File.File>> GetAllAsync() => throw new NotImplementedException();
        public Task<Entities.Models.File.File> CreateAsync(Entities.Models.File.File file) => throw new NotImplementedException();
        public Task<Entities.Models.File.File> UpdateAsync(Entities.Models.File.File file) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

