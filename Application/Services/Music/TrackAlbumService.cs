using Application.Abstractions.Interfaces.Repository.Music;
using Application.Abstractions.Interfaces.Services;
using Application.Extensions;
using Application.Response;
using Entities.Models.Music;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Music;

public class TrackAlbumService(
    ITrackRepository trackRepository,
    IAlbumRepository albumRepository
) : ITrackAlbumService
{
    public async Task<Album> ValidateTrackBelongsToDistributorAsync(int trackId, int distributorId)
    {
        // Получаем трек с коллекциями
        Track? trackWithCollections = await trackRepository
            .Query()
            .Include(t => t.Collections)
            .FirstOrDefaultAsync(t => t.Id == trackId);

        if (trackWithCollections == null)
            throw ResponseFactory.Create<NotFoundResponse>([$"{nameof(Track)} not found"]);

        // Находим альбом трека
        Album? album = trackWithCollections.Collections?.OfType<Album>().FirstOrDefault();
        if (album == null)
            throw ResponseFactory.Create<BadRequestResponse>(["Track must belong to an album"]);

        // Получаем альбом с информацией о дистрибьюторе
        Album albumWithDistributor = await albumRepository
            .SnInclude(a => a.Distributor)
            .GetByIdValidatedAsync(album.Id);

        // Проверяем принадлежность дистрибьютору
        if (albumWithDistributor.DistributorId != distributorId)
            throw ResponseFactory.Create<UnauthorizedResponse>(["Track does not belong to your distributor"]);

        return albumWithDistributor;
    }

    public async Task DeleteAlbumTracksAsync(int albumId)
    {
        // Получаем треки альбома
        List<Track> tracks = await albumRepository.GetTracksFromAlbumAsync(albumId);

        // Удаляем все треки
        if (tracks.Any())
        {
            await trackRepository.RemoveRangeAsync(tracks);
        }
    }
}

