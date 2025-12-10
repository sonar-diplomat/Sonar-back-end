using Entities.Models.Music;

namespace Application.Abstractions.Interfaces.Services;

public interface ITrackAlbumService
{
    /// <summary>
    /// Validates that a track belongs to the specified distributor through its album.
    /// </summary>
    /// <param name="trackId">The ID of the track to validate.</param>
    /// <param name="distributorId">The ID of the distributor.</param>
    /// <returns>The album with distributor information if validation passes.</returns>
    /// <exception cref="ResponseException">Throws NotFoundResponse if track or album not found, UnauthorizedResponse if track doesn't belong to distributor.</exception>
    Task<Album> ValidateTrackBelongsToDistributorAsync(int trackId, int distributorId);

    /// <summary>
    /// Deletes all tracks from an album.
    /// </summary>
    /// <param name="albumId">The ID of the album.</param>
    Task DeleteAlbumTracksAsync(int albumId);
}

