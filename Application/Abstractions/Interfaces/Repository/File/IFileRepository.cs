using FileModel = Entities.Models.File.File;

namespace Application.Abstractions.Interfaces.Repository.File;

public interface IFileRepository : IGenericRepository<FileModel>
{
    Task<FileModel> GetDefaultAsync();
}