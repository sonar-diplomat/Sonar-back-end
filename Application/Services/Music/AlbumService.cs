using Application.Abstractions.Interfaces.Repository;
using Application.Abstractions.Interfaces.Repository.Music;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.DTOs.Music;
using Application.Extensions;
using Entities.Models.Music;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Music;

public class AlbumService(
    IAlbumRepository repository,
    IGenericRepository<Album> genericRepository,
    IVisibilityStateService visibilityStateService,
    IImageFileService imageFileService,
    IAlbumArtistService albumArtistService,
    IArtistService artistService ) : CollectionService<Album>(genericRepository), IAlbumService
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
                ArtistId = (await artistService.GetByNameAsync(authorPseudonym))?.Id
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

    public async Task<Album> GetValidatedIncludeTracksAsync(int id)
    {
        return await repository.Include(a => a.Tracks).GetByIdValidatedAsync(id);
    }

    public async Task<Album> GetValidatedIncludeVisibilityStateAsync(int id)
    {
        return await repository.Include(a => a.VisibilityState).GetByIdValidatedAsync(id);
    }

    public async Task UpdateAlbumCoverAsync(int albumId, IFormFile newCover)
    {
        Album album = await GetByIdValidatedAsync(albumId);
        album.Cover = await imageFileService.UploadFileAsync(newCover);
        await repository.UpdateAsync(album);
    }
}