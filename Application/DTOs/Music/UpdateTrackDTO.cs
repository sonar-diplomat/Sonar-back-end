namespace Application.DTOs.Music;

public class UpdateTrackDTO
{
    public string? Title { get; set; }
    public bool? IsExplicit { get; set; }
    public bool? DrivingDisturbingNoises { get; set; }
    public int? GenreId { get; set; }
    public IEnumerable<int>? MoodTagIds { get; set; }
}