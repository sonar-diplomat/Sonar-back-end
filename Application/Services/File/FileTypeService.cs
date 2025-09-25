using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.File;
using Entities.Models;
using Entities.Models.File;

namespace Application.Services.File
{
    public class FileTypeService : IFileTypeService
    {
        private readonly IFileTypeRepository _repository;

        public FileTypeService(IFileTypeRepository repository)
        {
            _repository = repository;
        }

        public Task<FileType> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<FileType>> GetAllAsync() => throw new NotImplementedException();
        public Task<FileType> CreateAsync(FileType fileType) => throw new NotImplementedException();
        public Task<FileType> UpdateAsync(FileType fileType) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

