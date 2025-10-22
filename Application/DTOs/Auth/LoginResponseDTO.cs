namespace Application.DTOs.Auth;

    public record LoginResponseDTO(string accessToken,string refreshToken,int sessionId) {}

