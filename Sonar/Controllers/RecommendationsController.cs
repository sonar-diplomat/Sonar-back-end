using Analytics.API;
using Application;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.DTOs.Recommendations;
using Application.Response;
using Entities.Models.Music;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers;

[Route("api/recommendations")]
[ApiController]
public class RecommendationsController(
    UserManager<User> userManager,
    Recommendations.RecommendationsClient recommendationsClient,
    IAlbumService albumService,
    IPlaylistService playlistService,
    ITrackService trackService,
    ILogger<RecommendationsController> logger)
    : BaseController(userManager)
{
    [HttpGet("popular-collections")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<PopularCollectionDTO>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPopularCollections([FromQuery] int limit = 4)
    {
        limit = Math.Clamp(limit, 1, 50);

        var response = await recommendationsClient.GetPopularCollectionsAsync(new GetPopularCollectionsRequest
        {
            Limit = limit
        });

        var filteredItems = new List<PopularCollectionDTO>();

        foreach (var c in response.Collections)
        {
            // Check visibility state for albums and playlists
            bool isAccessible = false;
            if (c.CollectionType == CollectionType.CollectionAlbum)
            {
                try
                {
                    var album = await albumService.GetValidatedIncludeVisibilityStateAsync(c.CollectionId);
                    if (album?.VisibilityState != null)
                    {
                        var authorIds = album.AlbumArtists?
                            .Where(aa => aa.Artist != null)
                            .Select(aa => aa.Artist!.UserId)
                            .ToList();
                        isAccessible = VisibilityStateValidator.IsAccessible(album.VisibilityState, null, authorIds);
                    }
                }
                catch
                {
                    // Album not found or not accessible, skip it
                    continue;
                }
            }
            else if (c.CollectionType == CollectionType.CollectionPlaylist)
            {
                try
                {
                    var playlist = await playlistService.GetByIdWithVisibilityStateAsync(c.CollectionId);
                    // Skip favorite playlists
                    if (playlist?.Name == "Favorites")
                        continue;
                    
                    if (playlist?.VisibilityState != null)
                    {
                        var authorIds = new[] { playlist.CreatorId };
                        isAccessible = VisibilityStateValidator.IsAccessible(playlist.VisibilityState, null, authorIds);
                    }
                }
                catch
                {
                    // Playlist not found or not accessible, skip it
                    continue;
                }
            }

            if (isAccessible || c.CollectionType == CollectionType.CollectionUnknown)
            {
                filteredItems.Add(new PopularCollectionDTO
                {
                    CollectionId = c.CollectionId,
                    CollectionType = (int)c.CollectionType,
                    Score = c.Score,
                    Plays = c.Plays,
                    Likes = c.Likes,
                    Adds = c.Adds
                });
            }
        }

        throw ResponseFactory.Create<OkResponse<IEnumerable<PopularCollectionDTO>>>(filteredItems);
    }

    [HttpGet("recent-collections")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<CursorPageDTO<RecentCollectionDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetRecentCollections(
        [FromQuery] int limit = 5,
        [FromQuery] string? cursor = null)
    {
        limit = Math.Clamp(limit, 1, 50);
        var user = await GetUserByJwt();

        var response = await recommendationsClient.GetRecentCollectionsAsync(new GetRecentCollectionsRequest
        {
            UserId = user.Id,
            Limit = limit,
            Cursor = cursor ?? string.Empty
        });

        var filteredItems = new List<RecentCollectionDTO>();

        foreach (var c in response.Collections)
        {
            // Check visibility state for albums and playlists
            bool isAccessible = false;
            if (c.CollectionType == CollectionType.CollectionAlbum)
            {
                try
                {
                    var album = await albumService.GetValidatedIncludeVisibilityStateAsync(c.CollectionId, user.Id);
                    if (album?.VisibilityState != null)
                    {
                        var authorIds = album.AlbumArtists?
                            .Where(aa => aa.Artist != null)
                            .Select(aa => aa.Artist!.UserId)
                            .ToList();
                        isAccessible = VisibilityStateValidator.IsAccessible(album.VisibilityState, user.Id, authorIds);
                    }
                }
                catch
                {
                    // Album not found or not accessible, skip it
                    continue;
                }
            }
            else if (c.CollectionType == CollectionType.CollectionPlaylist)
            {
                try
                {
                    var playlist = await playlistService.GetByIdWithVisibilityStateAsync(c.CollectionId);
                    // Skip favorite playlists
                    if (playlist?.Name == "Favorites")
                        continue;
                    
                    if (playlist?.VisibilityState != null)
                    {
                        var authorIds = new[] { playlist.CreatorId };
                        isAccessible = VisibilityStateValidator.IsAccessible(playlist.VisibilityState, user.Id, authorIds);
                    }
                }
                catch
                {
                    // Playlist not found or not accessible, skip it
                    continue;
                }
            }

            if (isAccessible || c.CollectionType == CollectionType.CollectionUnknown)
            {
                filteredItems.Add(new RecentCollectionDTO
                {
                    CollectionId = c.CollectionId,
                    CollectionType = (int)c.CollectionType,
                    LastPlayedAtUtc = c.LastPlayedAt.ToDateTime().ToUniversalTime()
                });
            }
        }

        var page = new CursorPageDTO<RecentCollectionDTO>
        {
            Items = filteredItems,
            NextCursor = string.IsNullOrWhiteSpace(response.NextCursor) ? null : response.NextCursor
        };

        throw ResponseFactory.Create<OkResponse<CursorPageDTO<RecentCollectionDTO>>>(page);
    }

    [HttpGet("recent-tracks")]
    [Authorize]
    [ProducesResponseType(typeof(OkResponse<CursorPageDTO<RecentTrackDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetRecentTracks(
        [FromQuery] int limit = 5,
        [FromQuery] string? cursor = null)
    {
        limit = Math.Clamp(limit, 1, 50);
        var user = await GetUserByJwt();

        var response = await recommendationsClient.GetRecentTracksAsync(new GetRecentTracksRequest
        {
            UserId = user.Id,
            Limit = limit,
            Cursor = cursor ?? string.Empty
        });

        var filteredItems = new List<RecentTrackDTO>();

        foreach (var t in response.Tracks)
        {
            // Check visibility state for tracks
            try
            {
                // GetTrackDtoAsync validates visibility and returns DTO if accessible
                var trackDto = await trackService.GetTrackDtoAsync(t.TrackId, user.Id);
                
                if (trackDto != null)
                {
                    filteredItems.Add(new RecentTrackDTO
                    {
                        TrackId = t.TrackId,
                        LastPlayedAtUtc = t.LastPlayedAt.ToDateTime().ToUniversalTime(),
                        ContextId = t.ContextId == 0 ? null : t.ContextId,
                        ContextType = (int)t.ContextType
                    });
                }
            }
            catch
            {
                // Track not found or not accessible, skip it
                continue;
            }
        }

        var page = new CursorPageDTO<RecentTrackDTO>
        {
            Items = filteredItems,
            NextCursor = string.IsNullOrWhiteSpace(response.NextCursor) ? null : response.NextCursor
        };

        throw ResponseFactory.Create<OkResponse<CursorPageDTO<RecentTrackDTO>>>(page);
    }
}


