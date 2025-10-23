using Application.Abstractions.Interfaces.Repository.Music;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.DTOs;
using Entities.Models.Music;

namespace Application.Services.Music;

public class AlbumService(
    IAlbumRepository repository,
    IVisibilityStateService visibilityStateService,
    IImageFileService imageFileService) : GenericService<Album>(repository), IAlbumService
{
    public async Task<Album> UploadAsync(UploadAlbumDTO dto, int distributorId)
    {
        

        Album album = new()
        {
            Name = dto.Name,
            Cover = await imageFileService.UploadFileAsync(dto.Cover),
            VisibilityStateId = (await visibilityStateService.CreateDefaultAsync()).Id
        };

        return await repository.AddAsync(album);
    }

    public async Task<Album> UpdateNameAsync(int albumId, string newName)
    {
        Album album = await GetByIdValidatedAsync(albumId);
        album.Name = newName;
        return await repository.UpdateAsync(album);
    }
}