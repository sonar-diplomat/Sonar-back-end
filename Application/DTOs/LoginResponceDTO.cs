namespace Application.DTOs;

public record LoginResponceDTO(string accessToken, string refreshToken, int sessionId)
{
}