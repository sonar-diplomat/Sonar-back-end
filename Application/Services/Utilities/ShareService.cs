using System.Drawing;
using Application.Abstractions.Interfaces.Services.Utilities;
using Microsoft.Extensions.Configuration;
using QRCoder;
using SysFile = System.IO.File;

namespace Application.Services.Utilities;

public class ShareService(QRCodeGenerator qrGenerator, IConfiguration configuration) : IShareService
{
    private static readonly string LogoPath =
        Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../app/data", "icons", "logo.svg"));

    private static readonly string? SvgLogoContent = SysFile.Exists(LogoPath) ? SysFile.ReadAllText(LogoPath) : null;
    private readonly string baseUrl = configuration["FrontEnd-Url"]!;

    public async Task<string> GenerateQrCode(string link)
    {
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(link, QRCodeGenerator.ECCLevel.Q);
        SvgQRCode svgQr = new(qrCodeData);
        SvgQRCode.SvgLogo? logo = null;
        if (!string.IsNullOrWhiteSpace(SvgLogoContent))
            logo = new SvgQRCode.SvgLogo(SvgLogoContent, 20, false);

        string svg = svgQr.GetGraphic(new Size(40, 40), logo: logo);
        return await Task.FromResult(svg);
    }

    public async Task<string> GenerateLinkAsync<T>(int entityId)
    {
        return await Task.FromResult(baseUrl + "/" + typeof(T).Name.ToLower() + "/" + entityId);
    }
}