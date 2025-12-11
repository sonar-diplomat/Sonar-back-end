using Analytics.API;
using Application.DTOs;
using Application.DTOs.Recommendations;
using Application.Response;
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

        var items = response.Collections.Select(c => new PopularCollectionDTO
        {
            CollectionId = c.CollectionId,
            CollectionType = (int)c.CollectionType,
            Score = c.Score,
            Plays = c.Plays,
            Likes = c.Likes,
            Adds = c.Adds
        }).ToList();

        throw ResponseFactory.Create<OkResponse<IEnumerable<PopularCollectionDTO>>>(items);
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
            UserId = user.Id.ToString(),
            Limit = limit,
            Cursor = cursor ?? string.Empty
        });

        var items = response.Collections.Select(c => new RecentCollectionDTO
        {
            CollectionId = c.CollectionId,
            CollectionType = (int)c.CollectionType,
            LastPlayedAtUtc = c.LastPlayedAt.ToDateTime().ToUniversalTime()
        }).ToList();

        var page = new CursorPageDTO<RecentCollectionDTO>
        {
            Items = items,
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
            UserId = user.Id.ToString(),
            Limit = limit,
            Cursor = cursor ?? string.Empty
        });

        var items = response.Tracks.Select(t => new RecentTrackDTO
        {
            TrackId = t.TrackId,
            LastPlayedAtUtc = t.LastPlayedAt.ToDateTime().ToUniversalTime(),
            ContextId = string.IsNullOrWhiteSpace(t.ContextId) ? null : t.ContextId,
            ContextType = (int)t.ContextType
        }).ToList();

        var page = new CursorPageDTO<RecentTrackDTO>
        {
            Items = items,
            NextCursor = string.IsNullOrWhiteSpace(response.NextCursor) ? null : response.NextCursor
        };

        throw ResponseFactory.Create<OkResponse<CursorPageDTO<RecentTrackDTO>>>(page);
    }
}


