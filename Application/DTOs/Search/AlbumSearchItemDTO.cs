using Application.DTOs.Music;

namespace Application.DTOs.Search;

public class AlbumSearchItemDTO
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required int CoverId { get; set; }
    public required int TrackCount { get; set; }
    public required IEnumerable<AuthorDTO> Authors { get; set; }
    public string? DistributorName { get; set; }
}

