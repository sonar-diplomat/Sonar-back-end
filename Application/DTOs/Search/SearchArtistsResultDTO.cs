namespace Application.DTOs.Search;

public class SearchArtistsResultDTO
{
    public int Total { get; set; }
    public required IEnumerable<ArtistSearchItemDTO> Items { get; set; }
}

