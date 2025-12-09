using Application.DTOs.Music;

namespace Application.DTOs.User;

public class QueueDTO
{
    public required int Id { get; set; }
    public required TimeSpan Position { get; set; }
    public int? CollectionId { get; set; }
    public int? CurrentTrackId { get; set; }
    public required IEnumerable<TrackDTO> Tracks { get; set; }
}

