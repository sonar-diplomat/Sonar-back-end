namespace Application.DTOs.Search;

public class SearchTracksResultDTO
{
    public int Total { get; set; }
    public required IEnumerable<TrackSearchItemDTO> Items { get; set; }
}

