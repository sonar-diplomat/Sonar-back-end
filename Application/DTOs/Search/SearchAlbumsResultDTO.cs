namespace Application.DTOs.Search;

public class SearchAlbumsResultDTO
{
    public int Total { get; set; }
    public required IEnumerable<AlbumSearchItemDTO> Items { get; set; }
}

