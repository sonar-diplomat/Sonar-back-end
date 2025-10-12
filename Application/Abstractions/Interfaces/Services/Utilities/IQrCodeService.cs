namespace Application.Abstractions.Interfaces.Services.Utilities;

public interface IQrCodeService
{
    Task<string> GenerateQrCode(string link);
}
