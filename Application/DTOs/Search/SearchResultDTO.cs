namespace Application.DTOs.Search;

public class SearchResultDTO
{
    public required string Query { get; set; }
    public int TotalResults { get; set; }
    public SearchTracksResultDTO? Tracks { get; set; }
    public SearchAlbumsResultDTO? Albums { get; set; }
    public SearchPlaylistsResultDTO? Playlists { get; set; }
    public SearchArtistsResultDTO? Artists { get; set; }
    public SearchUsersResultDTO? Users { get; set; }
}

