using Application.Abstractions.Interfaces.Repository.File;
using Application.Abstractions.Interfaces.Services;
using Entities.Models.File;

namespace Application.Services.File
{
    public class FileTypeService(IFileTypeRepository repository) : IFileTypeService
    {
        public Task<FileType> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<FileType>> GetAllAsync() => throw new NotImplementedException();
        public Task<FileType> CreateAsync(FileType fileType) => throw new NotImplementedException();
        public Task<FileType> UpdateAsync(FileType fileType) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
    }
}

