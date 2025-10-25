using Application.Abstractions.Interfaces.Repository.Music;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.DTOs;
using Entities.Models.Music;

namespace Application.Services.Music;

public class AlbumService(
    IAlbumRepository repository,
    IVisibilityStateService visibilityStateService,
    IImageFileService imageFileService,
    IAlbumArtistService albumArtistService
) : GenericService<Album>(repository), IAlbumService
{
    public async Task<Album> UploadAsync(UploadAlbumDTO dto, int distributorId)
    {
        Album album = new()
        {
            Name = dto.Name,
            Cover = await imageFileService.UploadFileAsync(dto.Cover),
            VisibilityStateId = (await visibilityStateService.CreateDefaultAsync()).Id
        };

        foreach (string authorPseudonym in dto.Authors)
        {
            AlbumArtist albumArtist = new()
            {
                Pseudonym = authorPseudonym,
                AlbumId = album.Id,
                ArtistId = null // TODO: add artistService method to check if artist exists by name. if one does, add id here
            };
            await albumArtistService.CreateAsync(albumArtist);
        }

        return await repository.AddAsync(album);
    }

    public async Task<Album> UpdateNameAsync(int albumId, string newName)
    {
        Album album = await GetByIdValidatedAsync(albumId);
        album.Name = newName;
        return await repository.UpdateAsync(album);
    }


    // TODO: Implement this method (Include)
    public async Task ChangeVisibilityStateAsync(int albumId, int newVisibilityState)
    {
        Album? album;
        //album.VisibilityStateId = newVisibilityState;


        //await repository.UpdateAsync(album);

        throw new NotImplementedException();
    }
}