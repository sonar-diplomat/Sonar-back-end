namespace Application.DTOs.Search;

public class SearchPlaylistsResultDTO
{
    public int Total { get; set; }
    public required IEnumerable<PlaylistSearchItemDTO> Items { get; set; }
}

