using Entities.Models.File;

namespace Application.Abstractions.Interfaces.Repository.File;

public interface IImageFileRepository : IGenericRepository<ImageFile>
{
    Task<ImageFile> GetDefaultAsync();
    Task<ImageFile> GetFavoriteDefaultAsync();
}