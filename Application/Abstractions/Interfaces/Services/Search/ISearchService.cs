using Application.DTOs.Search;

namespace Application.Abstractions.Interfaces.Services;

public interface ISearchService
{
    Task<SearchResultDTO> SearchAsync(string query, string? category = null, int limit = 20, int offset = 0, int? userId = null);
    Task<SearchTracksResultDTO> SearchTracksAsync(string query, int limit = 20, int offset = 0, int? userId = null);
    Task<SearchAlbumsResultDTO> SearchAlbumsAsync(string query, int limit = 20, int offset = 0, int? userId = null);
    Task<SearchPlaylistsResultDTO> SearchPlaylistsAsync(string query, int limit = 20, int offset = 0, int? userId = null);
    Task<SearchArtistsResultDTO> SearchArtistsAsync(string query, int limit = 20, int offset = 0, int? userId = null);
    Task<SearchUsersResultDTO> SearchUsersAsync(string query, int limit = 20, int offset = 0, int? userId = null);
    Task<IEnumerable<string>> GetSuggestionsAsync(string query, int limit = 10);
    Task<IEnumerable<string>> GetPopularQueriesAsync(int limit = 10);
}

