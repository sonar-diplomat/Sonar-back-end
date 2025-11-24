using Application.DTOs;
using Application.DTOs.Music;
using Entities.Models.Music;
using Microsoft.AspNetCore.Http;

namespace Application.Abstractions.Interfaces.Services;

public interface IPlaylistService : ICollectionService<Playlist>
{
    Task<Playlist> CreatePlaylistAsync(int creatorId, CreatePlaylistDTO dto);
    Task DeleteAsync(int id, int userId);
    Task<Playlist> UpdateNameAsync(int playlistId, int userId, string newName);
    Task UpdatePlaylistCoverAsync(int playlistId, int creatorId, IFormFile newCover);
    Task AddContributorAsync(int playlistId, int contributorId, int userId);
    Task RemoveContributorAsync(int playlistId, int contributorId, int creatorId);
    Task AddTrackToPlaylistAsync(int playlistId, int trackId, int userId);
    Task RemoveTrackFromPlaylistAsync(int playlistId, int trackId, int userId);
    Task<CursorPageDTO<TrackDTO>> GetPlaylistTracksAsync(int playlistId, string? after, int limit);
    Task ImportCollectionToPlaylistAsync<T>(int playlistId, int collectionId, int userId) where T : Collection;
    Task<Playlist> GetByIdWithVisibilityStateAsync(int playlistId);
}