namespace Application.Abstractions.Interfaces.Service.Utilities
{
    public interface IVerificationService
    {
        Task<string> GenerateVerificationCode(int length = 6);
        Task<bool> ValidateVerificationCode(string code, string inputCode);

        Task<string> GenerateVerificationTokenAsync(int userId);
        Task<bool> ValidateVerificationTokenAsync(string token, int userId);
    }
}
