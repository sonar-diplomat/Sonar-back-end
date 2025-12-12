using Application.DTOs.Music;

namespace Application.DTOs.User;

public class TopTrackDTO
{
    public required int Id { get; set; }
    public required string Title { get; set; }
    public required int DurationInSeconds { get; set; }
    public required int CoverId { get; set; }
    public required IEnumerable<AuthorDTO> Artists { get; set; }
    public int? AlbumId { get; set; }
    public string? AlbumName { get; set; }
    public long PlayCount { get; set; }
}

