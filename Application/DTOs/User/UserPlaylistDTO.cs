namespace Application.DTOs.User;

public class UserPlaylistDTO
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required int CoverId { get; set; }
    public required int TrackCount { get; set; }
}

