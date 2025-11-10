using Application.Abstractions.Interfaces.Repository.Music;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.DTOs.Music;
using Application.Extensions;
using Entities.Models.Music;

namespace Application.Services.Music;

public class AlbumService(
    IAlbumRepository repository,
    IVisibilityStateService visibilityStateService,
    IImageFileService imageFileService,
    IAlbumArtistService albumArtistService,
    IArtistService artistService,
    ILibraryService libraryService,
    IFolderService folderService
) : CollectionService<Album>(repository, libraryService, folderService), IAlbumService
{
    public async Task<Album> UploadAsync(UploadAlbumDTO dto, int distributorId)
    {
        Album album = new()
        {
            Name = dto.Name,
            Cover = await imageFileService.UploadFileAsync(dto.Cover),
            VisibilityStateId = (await visibilityStateService.CreateDefaultAsync()).Id,
            DistributorId = distributorId
        };

        album = await repository.AddAsync(album);

        foreach (string authorPseudonym in dto.Authors)
        {
            AlbumArtist albumArtist = new()
            {
                Pseudonym = authorPseudonym,
                AlbumId = album.Id,
                ArtistId = (await artistService.GetByNameAsync(authorPseudonym))?.Id
            };
            await albumArtistService.CreateAsync(albumArtist);
        }

        return album;
    }

    public async Task<Album> UpdateNameAsync(int albumId, string newName)
    {
        Album album = await GetByIdValidatedAsync(albumId);
        album.Name = newName;
        return await repository.UpdateAsync(album);
    }

    public async Task UpdateVisibilityStateAsync(int albumId, int newVisibilityState)
    {
        Album album = await repository.Include(a => a.VisibilityState).GetByIdValidatedAsync(albumId);
        album.VisibilityStateId = newVisibilityState;
        await repository.UpdateAsync(album);
    }

    public async Task<Album> GetValidatedIncludeTracksAsync(int id)
    {
        return await repository.Include(a => a.Tracks).GetByIdValidatedAsync(id);
    }

    public async Task<Album> GetValidatedIncludeVisibilityStateAsync(int id)
    {
        return await repository.Include(a => a.VisibilityState).GetByIdValidatedAsync(id);
    }
}