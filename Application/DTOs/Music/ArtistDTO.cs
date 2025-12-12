using Application.DTOs.User;

namespace Application.DTOs.Music;

public class ArtistDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string ArtistName { get; set; }
    public UserResponseDTO? User { get; set; }
}

