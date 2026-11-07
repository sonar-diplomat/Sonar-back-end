namespace Application.DTOs.Auth;

public sealed record TwoFactorChallengeDTO(bool RequiresTwoFactor, string ChallengeToken, DateTimeOffset ExpiresAt);
