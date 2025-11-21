using Entities.Models.Library;
using Entities.Models.Music;

namespace Application.Abstractions.Interfaces.Services;

public interface ILibraryService : IGenericService<Library>
{
    Task<Library> CreateDefaultAsync();
    Task<Folder> GetRootFolderByLibraryIdValidatedAsync(int libraryId);
    Task<Playlist> GetFavoritesPlaylistByLibraryIdValidatedAsync(int libraryId);
}