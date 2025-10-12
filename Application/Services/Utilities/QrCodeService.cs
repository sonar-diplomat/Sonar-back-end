using System.Drawing;
using Application.Abstractions.Interfaces.Services.Utilities;
using QRCoder;
using Svg;

namespace Application.Services.Utilities;

public class QrCodeService(
    QRCodeGenerator qrGenerator
) : IQrCodeService
{
    private static SvgDocument? svgDocument = SvgDocument.Open(
        Path.Combine(Directory.GetCurrentDirectory(), "../AppData", "icons", "logo.svg"));

    public async Task<string> GenerateQrCode(string link)
    {
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(link, QRCodeGenerator.ECCLevel.Q);
        SvgQRCode qrCode = new(qrCodeData);


        return qrCode.GetGraphic(new Size(40, 40), logo: new SvgQRCode.SvgLogo(new Bitmap(svgDocument.Draw())));
    }
}
