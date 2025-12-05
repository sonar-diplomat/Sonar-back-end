using Application.Abstractions.Interfaces.Services;
using Application.DTOs.Search;
using Application.Response;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Sonar.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SearchController(
    UserManager<User> userManager,
    ISearchService searchService)
    : BaseController(userManager)
{
    /// <summary>
    /// Универсальный поиск по всем типам контента.
    /// </summary>
    /// <param name="query">Поисковый запрос.</param>
    /// <param name="category">Категория поиска: "All" | "Tracks" | "Albums" | "Playlists" | "Artists" | "Users" | "Creators".</param>
    /// <param name="limit">Количество результатов на категорию (по умолчанию: 20).</param>
    /// <param name="offset">Смещение для пагинации (по умолчанию: 0).</param>
    /// <returns>SearchResultDTO с результатами по всем категориям.</returns>
    /// <response code="200">Поиск выполнен успешно.</response>
    /// <response code="400">Неверные параметры запроса.</response>
    /// <response code="401">Требуется авторизация (для некоторых категорий может быть опционально).</response>
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
            // Пользователь не авторизован - это нормально для публичного поиска
        }

        SearchResultDTO result = await searchService.SearchAsync(query, category, limit, offset, userId);
        throw ResponseFactory.Create<OkResponse<SearchResultDTO>>(result, ["Search completed successfully"]);
    }

    /// <summary>
    /// Поиск треков.
    /// </summary>
    /// <param name="query">Поисковый запрос.</param>
    /// <param name="limit">Количество результатов (по умолчанию: 20).</param>
    /// <param name="offset">Смещение для пагинации (по умолчанию: 0).</param>
    /// <returns>SearchTracksResultDTO с результатами поиска треков.</returns>
    /// <response code="200">Поиск выполнен успешно.</response>
    /// <response code="400">Неверные параметры запроса.</response>
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
            // Пользователь не авторизован
        }

        SearchTracksResultDTO result = await searchService.SearchTracksAsync(query, limit, offset, userId);
        throw ResponseFactory.Create<OkResponse<SearchTracksResultDTO>>(result, ["Tracks search completed successfully"]);
    }

    /// <summary>
    /// Поиск альбомов.
    /// </summary>
    /// <param name="query">Поисковый запрос.</param>
    /// <param name="limit">Количество результатов (по умолчанию: 20).</param>
    /// <param name="offset">Смещение для пагинации (по умолчанию: 0).</param>
    /// <returns>SearchAlbumsResultDTO с результатами поиска альбомов.</returns>
    /// <response code="200">Поиск выполнен успешно.</response>
    /// <response code="400">Неверные параметры запроса.</response>
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
            // Пользователь не авторизован
        }

        SearchAlbumsResultDTO result = await searchService.SearchAlbumsAsync(query, limit, offset, userId);
        throw ResponseFactory.Create<OkResponse<SearchAlbumsResultDTO>>(result, ["Albums search completed successfully"]);
    }

    /// <summary>
    /// Поиск плейлистов.
    /// </summary>
    /// <param name="query">Поисковый запрос.</param>
    /// <param name="limit">Количество результатов (по умолчанию: 20).</param>
    /// <param name="offset">Смещение для пагинации (по умолчанию: 0).</param>
    /// <returns>SearchPlaylistsResultDTO с результатами поиска плейлистов.</returns>
    /// <response code="200">Поиск выполнен успешно.</response>
    /// <response code="400">Неверные параметры запроса.</response>
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
            // Пользователь не авторизован
        }

        SearchPlaylistsResultDTO result = await searchService.SearchPlaylistsAsync(query, limit, offset, userId);
        throw ResponseFactory.Create<OkResponse<SearchPlaylistsResultDTO>>(result, ["Playlists search completed successfully"]);
    }

    /// <summary>
    /// Поиск артистов.
    /// </summary>
    /// <param name="query">Поисковый запрос.</param>
    /// <param name="limit">Количество результатов (по умолчанию: 20).</param>
    /// <param name="offset">Смещение для пагинации (по умолчанию: 0).</param>
    /// <returns>SearchArtistsResultDTO с результатами поиска артистов.</returns>
    /// <response code="200">Поиск выполнен успешно.</response>
    /// <response code="400">Неверные параметры запроса.</response>
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
            // Пользователь не авторизован
        }

        SearchArtistsResultDTO result = await searchService.SearchArtistsAsync(query, limit, offset, userId);
        throw ResponseFactory.Create<OkResponse<SearchArtistsResultDTO>>(result, ["Artists search completed successfully"]);
    }

    /// <summary>
    /// Поиск пользователей.
    /// </summary>
    /// <param name="query">Поисковый запрос.</param>
    /// <param name="limit">Количество результатов (по умолчанию: 20).</param>
    /// <param name="offset">Смещение для пагинации (по умолчанию: 0).</param>
    /// <returns>SearchUsersResultDTO с результатами поиска пользователей.</returns>
    /// <response code="200">Поиск выполнен успешно.</response>
    /// <response code="400">Неверные параметры запроса.</response>
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
            // Пользователь не авторизован
        }

        SearchUsersResultDTO result = await searchService.SearchUsersAsync(query, limit, offset, userId);
        throw ResponseFactory.Create<OkResponse<SearchUsersResultDTO>>(result, ["Users search completed successfully"]);
    }

    /// <summary>
    /// Получение предложений для автодополнения поискового запроса.
    /// </summary>
    /// <param name="query">Частичный поисковый запрос.</param>
    /// <param name="limit">Количество предложений (по умолчанию: 10).</param>
    /// <returns>Список предложений для автодополнения.</returns>
    /// <response code="200">Предложения получены успешно.</response>
    /// <response code="400">Неверные параметры запроса.</response>
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
    /// Получение популярных поисковых запросов.
    /// </summary>
    /// <param name="limit">Количество популярных запросов (по умолчанию: 10).</param>
    /// <returns>Список популярных поисковых запросов.</returns>
    /// <response code="200">Популярные запросы получены успешно.</response>
    [HttpGet("popular")]
    [ProducesResponseType(typeof(OkResponse<IEnumerable<string>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPopularQueries([FromQuery] int limit = 10)
    {
        IEnumerable<string> popularQueries = await searchService.GetPopularQueriesAsync(limit);
        throw ResponseFactory.Create<OkResponse<IEnumerable<string>>>(popularQueries, ["Popular queries retrieved successfully"]);
    }
}

