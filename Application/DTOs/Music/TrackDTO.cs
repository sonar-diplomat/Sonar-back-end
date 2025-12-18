namespace Application.DTOs.Music;

public class TrackDTO
{
    public required int Id { get; set; }
    public required string Title { get; set; }
    public required int DurationInSeconds { get; set; }
    public required bool IsExplicit { get; set; }
    public required bool DrivingDisturbingNoises { get; set; }
    public required int CoverId { get; set; }
    public required int AudioFileId { get; set; }
    public required IEnumerable<AuthorDTO> Artists { get; set; }
    public required GenreDTO Genre { get; set; }
    public IEnumerable<MoodTagDTO> MoodTags { get; set; } = new List<MoodTagDTO>();
}