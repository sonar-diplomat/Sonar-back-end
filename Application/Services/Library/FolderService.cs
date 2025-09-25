using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Repository.Library;
using Entities.Models;
using Entities.Models.Library;

namespace Application.Services.Library
{
    public class FolderService : IFolderService
    {
        private readonly IFolderRepository _repository;

        public FolderService(IFolderRepository repository)
        {
            _repository = repository;
        }

        public Task<Folder> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<Folder>> GetAllAsync() => throw new NotImplementedException();
        public Task<Folder> CreateAsync(Folder entity) => throw new NotImplementedException();
        public Task<Folder> UpdateAsync(Folder entity) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

