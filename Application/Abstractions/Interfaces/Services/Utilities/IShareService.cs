namespace Application.Abstractions.Interfaces.Services.Utilities;

public interface IShareService
{
    Task<string> GenerateQrCode(string link);
    Task<string> GenerateLinkAsync<T>(int entityId);
}
