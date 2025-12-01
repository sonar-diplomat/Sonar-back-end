using Application.Abstractions.Interfaces.Repository.Music;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.DTOs.Music;
using Application.Extensions;
using Application.Response;
using Entities.Models.Music;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services.Music;

public class AlbumService(
    IAlbumRepository repository,
    IVisibilityStateService visibilityStateService,
    IImageFileService imageFileService,
    IAlbumArtistService albumArtistService,
    IArtistService artistService,
    ILibraryService libraryService,
    IFolderService folderService,
    IServiceProvider serviceProvider
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
        Album album = await repository.SnInclude(a => a.VisibilityState).GetByIdValidatedAsync(albumId);
        album.VisibilityStateId = newVisibilityState;
        await repository.UpdateAsync(album);
    }

    public async Task<Album> GetValidatedIncludeTracksAsync(int id)
    {
        return await repository.SnInclude(a => a.Tracks).GetByIdValidatedAsync(id);
    }

    public async Task<Album> GetValidatedIncludeVisibilityStateAsync(int id)
    {
        return await repository
            .SnInclude(a => a.VisibilityState)
            .SnThenInclude(vs => vs.Status)
            .GetByIdValidatedAsync(id);
    }

    public async Task<Album> GetValidatedIncludeDistributorAsync(int id)
    {
        return await repository.SnInclude(a => a.Distributor).GetByIdValidatedAsync(id);
    }

    public async Task<Album> GetValidatedIncludeAlbumArtistsAsync(int id)
    {
        return await repository
            .Query()
            .Include(a => a.AlbumArtists)
            .ThenInclude(aa => aa.Artist)
            .FirstOrDefaultAsync(a => a.Id == id) 
            ?? throw ResponseFactory.Create<NotFoundResponse>([$"{nameof(Album)} not found"]);
    }

    public async Task UpdateCoverAsync(int albumId, int imageId)
    {
        Album album = await repository.SnInclude(a => a.Tracks).GetByIdValidatedAsync(albumId);

        album.CoverId = imageId;
        foreach (var track in album.Tracks) { track.CoverId = imageId; }

        await repository.UpdateAsync(album);
    }

    public async Task<IEnumerable<Album>> GetAlbumsByDistributorIdAsync(int distributorId)
    {
        return await repository.GetByDistributorIdAsync(distributorId);
    }

    public async Task<IEnumerable<TrackDTO>> GetAlbumTracksAsync(int albumId)
    {
        await GetByIdValidatedAsync(albumId);
        
        List<Track> tracks = await repository.GetTracksFromAlbumAsync(albumId);

        // Convert to DTOs
        List<TrackDTO> trackDTOs = tracks
            .Select(t => new TrackDTO
            {
                Id = t.Id,
                Title = t.Title,
                DurationInSeconds = (int)(t.Duration?.TotalSeconds ?? 0),
                IsExplicit = t.IsExplicit,
                DrivingDisturbingNoises = t.DrivingDisturbingNoises,
                CoverId = t.CoverId,
                AudioFileId = t.LowQualityAudioFileId,
                Artists = t.TrackArtists?.Select(ta => new AuthorDTO
                {
                    Pseudonym = ta.Pseudonym,
                    ArtistId = ta.ArtistId
                }).ToList() ?? new List<AuthorDTO>()
            })
            .ToList();

        return trackDTOs;
    }

    public async Task<IEnumerable<Track>> GetAlbumTracksWithVisibilityStateAsync(int albumId)
    {
        await GetByIdValidatedAsync(albumId);
        return await repository.GetTracksFromAlbumAsync(albumId);
    }

    /// <summary>
    /// Удаляет альбом и все его треки каскадно.
    /// </summary>
    public override async Task DeleteAsync(int id)
    {
        Album album = await GetValidatedIncludeTracksAsync(id);
        
        // Удаляем все треки альбома перед удалением альбома
        if (album.Tracks != null && album.Tracks.Any())
        {
            // Используем IServiceProvider для ленивой загрузки ITrackService,
            // чтобы избежать циклической зависимости при создании объектов
            ITrackService trackService = serviceProvider.GetRequiredService<ITrackService>();
            
            foreach (Track track in album.Tracks.ToList())
            {
                await trackService.DeleteAsync(track);
            }
        }
        
        // Удаляем сам альбом
        await base.DeleteAsync(id);
    }
}