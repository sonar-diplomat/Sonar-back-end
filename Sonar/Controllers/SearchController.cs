using Analytics.API;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs.Search;
using Application.Response;
using Entities.Models.UserCore;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Sonar.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SearchController(
    UserManager<User> userManager,
    ISearchService searchService,
    Analytics.API.Analytics.AnalyticsClient analyticsClient,
    ILogger<SearchController> logger)
    : BaseController(userManager)
{
    /// <summary>
    /// Universal search across all content types.
    /// </summary>
    /// <param name="query">Search query.</param>
    /// <param name="category">Search category: "all" | "tracks" | "album" | "playlist" | "artist" | "users".</param>
    /// <param name="limit">Number of results per category (default: 20).</param>
    /// <param name="offset">Pagination offset (default: 0).</param>
    /// <returns>SearchResultDTO with results across all categories.</returns>
    /// <response code="200">Search completed successfully.</response>
    /// <response code="400">Invalid request parameters.</response>
    /// <response code="401">Authorization required (optional for some categories).</response>
    [HttpGet]
    [ProducesResponseType(typeof(OkResponse<SearchResultDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Search(
        [FromQuery] string query,
        [FromQuery] string? category = null,
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0)
    {
        if (string.IsNullOrWhiteSpace(query))
            throw ResponseFactory.Create<BadRequestResponse>(["Query parameter is required"]);

        int? userId = null;
        try
        {
            User? user = await userManager.GetUserAsync(User);
            userId = user?.Id;
        }
        catch
        {
            // User is not authorized - this is normal for public search
        }

        SearchResultDTO result = await searchService.SearchAsync(query, category, limit, offset, userId);

        // Send Search event to Analytics (only for authorized users)
        if (userId.HasValue)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await analyticsClient.AddUserEventAsync(new UserEventRequest
                    {
                        UserId = userId.Value,
                        EventType = EventType.Search,
                        ContextType = ContextType.ContextSearch,
                        Payload = JsonSerializer.Serialize(new { query, category }),
                        Timestamp = Timestamp.FromDateTime(DateTime.UtcNow)
                    });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to send Search event to Analytics");
                }
            });
        }

        throw ResponseFactory.Create<OkResponse<SearchResultDTO>>(result, ["Search completed successfully"]);
    }

    /// <summary>
    /// Search tracks.
    /// </summary>
    /// <param name="query">Search query.</param>
    /// <param name="limit">Number of results (default: 20).</param>
    /// <param name="offset">Pagination offset (default: 0).</param>
    /// <returns>SearchTracksResultDTO with track search results.</returns>
    /// <response code="200">Search completed successfully.</response>
    /// <response code="400">Invalid request parameters.</response>
    [HttpGet("tracks")]
    [ProducesResponseType(typeof(OkResponse<SearchTracksResultDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchTracks(
        [FromQuery] string query,
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0)
    {
        if (string.IsNullOrWhiteSpace(query))
            throw ResponseFactory.Create<BadRequestResponse>(["Query parameter is required"]);

        int? userId = null;
        try
        {
            User? user = await userManager.GetUserAsync(User);
            userId = user?.Id;
        }
        catch
        {
            // User is not authorized
        }

        SearchTracksResultDTO result = await searchService.SearchTracksAsync(query, limit, offset, userId);

        // Send Search event to Analytics (only for authorized users)
        if (userId.HasValue)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await analyticsClient.AddUserEventAsync(new UserEventRequest
                    {
                        UserId = userId.Value,
                        EventType = EventType.Search,
                        ContextType = ContextType.ContextSearch,
                        Payload = JsonSerializer.Serialize(new { query, category = "tracks" }),
                        Timestamp = Timestamp.FromDateTime(DateTime.UtcNow)
                    });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to send SearchTracks event to Analytics");
                }
            });
        }

        throw ResponseFactory.Create<OkResponse<SearchTracksResultDTO>>(result, ["Tracks search completed successfully"]);
    }

    /// <summary>
    /// Search albums.
    /// </summary>
    /// <param name="query">Search query.</param>
    /// <param name="limit">Number of results (default: 20).</param>
    /// <param name="offset">Pagination offset (default: 0).</param>
    /// <returns>SearchAlbumsResultDTO with album search results.</returns>
    /// <response code="200">Search completed successfully.</response>
    /// <response code="400">Invalid request parameters.</response>
    [HttpGet("albums")]
    [ProducesResponseType(typeof(OkResponse<SearchAlbumsResultDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchAlbums(
        [FromQuery] string query,
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0)
    {
        if (string.IsNullOrWhiteSpace(query))
            throw ResponseFactory.Create<BadRequestResponse>(["Query parameter is required"]);

        int? userId = null;
        try
        {
            User? user = await userManager.GetUserAsync(User);
            userId = user?.Id;
        }
        catch
        {
            // User is not authorized
        }

        SearchAlbumsResultDTO result = await searchService.SearchAlbumsAsync(query, limit, offset, userId);

        // Send Search event to Analytics (only for authorized users)
        if (userId.HasValue)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await analyticsClient.AddUserEventAsync(new UserEventRequest
                    {
                        UserId = userId.Value,
                        EventType = EventType.Search,
                        ContextType = ContextType.ContextSearch,
                        Payload = JsonSerializer.Serialize(new { query, category = "albums" }),
                        Timestamp = Timestamp.FromDateTime(DateTime.UtcNow)
                    });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to send SearchAlbums event to Analytics");
                }
            });
        }

        throw ResponseFactory.Create<OkResponse<SearchAlbumsResultDTO>>(result, ["Albums search completed successfully"]);
    }

    /// <summary>
    /// Search playlists.
    /// </summary>
    /// <param name="query">Search query.</param>
    /// <param name="limit">Number of results (default: 20).</param>
    /// <param name="offset">Pagination offset (default: 0).</param>
    /// <returns>SearchPlaylistsResultDTO with playlist search results.</returns>
    /// <response code="200">Search completed successfully.</response>
    /// <response code="400">Invalid request parameters.</response>
    [HttpGet("playlists")]
    [ProducesResponseType(typeof(OkResponse<SearchPlaylistsResultDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchPlaylists(
        [FromQuery] string query,
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0)
    {
        if (string.IsNullOrWhiteSpace(query))
            throw ResponseFactory.Create<BadRequestResponse>(["Query parameter is required"]);

        int? userId = null;
        try
        {
            User? user = await userManager.GetUserAsync(User);
            userId = user?.Id;
        }
        catch
        {
            // User is not authorized
        }

        SearchPlaylistsResultDTO result = await searchService.SearchPlaylistsAsync(query, limit, offset, userId);

        if (userId.HasValue)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await analyticsClient.AddUserEventAsync(new UserEventRequest
                    {
                        UserId = userId.Value,
                        EventType = EventType.Search,
                        ContextType = ContextType.ContextSearch,
                        Payload = JsonSerializer.Serialize(new { query, category = "playlists" }),
                        Timestamp = Timestamp.FromDateTime(DateTime.UtcNow)
                    });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to send SearchPlaylists event to Analytics");
                }
            });
        }

        throw ResponseFactory.Create<OkResponse<SearchPlaylistsResultDTO>>(result, ["Playlists search completed successfully"]);
    }

    /// <summary>
    /// Search artists.
    /// </summary>
    /// <param name="query">Search query.</param>
    /// <param name="limit">Number of results (default: 20).</param>
    /// <param name="offset">Pagination offset (default: 0).</param>
    /// <returns>SearchArtistsResultDTO with artist search results.</returns>
    /// <response code="200">Search completed successfully.</response>
    /// <response code="400">Invalid request parameters.</response>
    [HttpGet("artists")]
    [ProducesResponseType(typeof(OkResponse<SearchArtistsResultDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchArtists(
        [FromQuery] string query,
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0)
    {
        if (string.IsNullOrWhiteSpace(query))
            throw ResponseFactory.Create<BadRequestResponse>(["Query parameter is required"]);

        int? userId = null;
        try
        {
            User? user = await userManager.GetUserAsync(User);
            userId = user?.Id;
        }
        catch
        {
        }

        SearchArtistsResultDTO result = await searchService.SearchArtistsAsync(query, limit, offset, userId);

        if (userId.HasValue)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await analyticsClient.AddUserEventAsync(new UserEventRequest
                    {
                        UserId = userId.Value,
                        EventType = EventType.Search,
                        ContextType = ContextType.ContextSearch,
                        Payload = JsonSerializer.Serialize(new { query, category = "artists" }),
                        Timestamp = Timestamp.FromDateTime(DateTime.UtcNow)
                    });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to send SearchArtists event to Analytics");
                }
            });
        }

        throw ResponseFactory.Create<OkResponse<SearchArtistsResultDTO>>(result, ["Artists search completed successfully"]);
    }

    /// <summary>
    /// Search users.
    /// </summary>
    /// <param name="query">Search query.</param>
    /// <param name="limit">Number of results (default: 20).</param>
    /// <param name="offset">Pagination offset (default: 0).</param>
    /// <returns>SearchUsersResultDTO with user search results.</returns>
    /// <response code="200">Search completed successfully.</response>
    /// <response code="400">Invalid request parameters.</response>
    [HttpGet("users")]
    [ProducesResponseType(typeof(OkResponse<SearchUsersResultDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchUsers(
        [FromQuery] string query,
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0)
    {
        if (string.IsNullOrWhiteSpace(query))
            throw ResponseFactory.Create<BadRequestResponse>(["Query parameter is required"]);

        int? userId = null;
        try
        {
            User? user = await userManager.GetUserAsync(User);
            userId = user?.Id;
        }
        catch
        {
            // User is not authorized
        }

        SearchUsersResultDTO result = await searchService.SearchUsersAsync(query, limit, offset, userId);

        // Send Search event to Analytics (only for authorized users)
        if (userId.HasValue)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await analyticsClient.AddUserEventAsync(new UserEventRequest
                    {
                        UserId = userId.Value,
                        EventType = EventType.Search,
                        ContextType = ContextType.ContextSearch,
                        Payload = JsonSerializer.Serialize(new { query, category = "users" }),
                        Timestamp = Timestamp.FromDateTime(DateTime.UtcNow)
                    });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to send SearchUsers event to Analytics");
                }
            });
        }

        throw ResponseFactory.Create<OkResponse<SearchUsersResultDTO>>(result, ["Users search completed successfully"]);
    }

    /// <summary>
    /// Get suggestions for search query autocomplete.
    /// </summary>
    /// <param name="query">Partial search query.</param>
    /// <param name="limit">Number of suggestions (default: 10).</param>
    /// <returns>List of autocomplete suggestions.</returns>
    /// <response code="200">Suggestions retrieved successfully.</response>
    /// <response code="400">Invalid request parameters.</response>
    [HttpGet("suggestions")]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<string>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSuggestions(
        [FromQuery] string query,
        [FromQuery] int limit = 10)
    {
        if (string.IsNullOrWhiteSpace(query))
            throw ResponseFactory.Create<BadRequestResponse>(["Query parameter is required"]);

        IEnumerable<string> suggestions = await searchService.GetSuggestionsAsync(query, limit);
        throw ResponseFactory.Create<OkResponse<IEnumerable<string>>>(suggestions, ["Suggestions retrieved successfully"]);
    }

    /// <summary>
    /// Get popular search queries.
    /// </summary>
    /// <param name="limit">Number of popular queries (default: 10).</param>
    /// <returns>List of popular search queries.</returns>
    /// <response code="200">Popular queries retrieved successfully.</response>
    [HttpGet("popular")]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<string>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPopularQueries([FromQuery] int limit = 10)
    {
        IEnumerable<string> popularQueries = await searchService.GetPopularQueriesAsync(limit);
        throw ResponseFactory.Create<OkResponse<IEnumerable<string>>>(popularQueries, ["Popular queries retrieved successfully"]);
    }
}

