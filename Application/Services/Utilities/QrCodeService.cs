using System.Drawing;
using Application.Abstractions.Interfaces.Services.Utilities;
using QRCoder;
using SysFile = System.IO.File;

namespace Application.Services.Utilities;

public class QrCodeService(QRCodeGenerator qrGenerator) : IQrCodeService
{
    private static readonly string LogoPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../AppData", "icons", "logo.svg"));
    private static readonly string? SvgLogoContent = SysFile.Exists(LogoPath) ? SysFile.ReadAllText(LogoPath) : null;

    public Task<string> GenerateQrCode(string link)
    {
        var qrCodeData = qrGenerator.CreateQrCode(link, QRCodeGenerator.ECCLevel.Q);
        var svgQr = new SvgQRCode(qrCodeData);
        SvgQRCode.SvgLogo? logo = null;
        if (!string.IsNullOrWhiteSpace(SvgLogoContent))
            logo = new SvgQRCode.SvgLogo(SvgLogoContent, 20, false);

        string svg = svgQr.GetGraphic(new Size(40, 40), logo: logo);
        return Task.FromResult(svg);
    }
}
