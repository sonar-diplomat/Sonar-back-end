using System.Drawing;
using Application.Abstractions.Interfaces.Services.Utilities;
using Microsoft.Extensions.Configuration;
using QRCoder;
using SysFile = System.IO.File;

namespace Application.Services.Utilities;

public class ShareService(QRCodeGenerator qrGenerator, IConfiguration configuration) : IShareService
{
    private static readonly string SvgLogoContent = "<svg width=\"256\" height=\"256\" viewBox=\"0 0 256 256\" fill=\"none\" xmlns=\"http://www.w3.org/2000/svg\">\n<rect width=\"256\" height=\"256\" rx=\"16\" fill=\"white\"/>\n<path d=\"M197.802 109.374C199.402 108.954 200.371 107.272 199.865 105.716C190.137 75.5727 157.879 57.7472 126.926 66.0293C95.9311 74.3115 76.8961 105.885 83.6342 136.911C83.9711 138.467 83.0867 139.98 81.5707 140.401L58.1981 146.665C56.5978 147.085 55.6292 148.767 56.1345 150.322C65.8626 180.466 98.121 198.25 129.116 189.968C160.111 181.685 179.146 150.07 172.408 119.086C172.071 117.53 172.955 116.017 174.471 115.596L197.844 109.332L197.802 109.374ZM156.237 120.431C149.204 122.323 144.277 128.377 143.308 135.608C141.792 147.38 133.327 157.722 121.241 160.959C106.375 164.953 90.8776 156.881 85.4029 142.881C84.7712 141.242 85.6556 139.392 87.3822 138.929L99.7213 135.608C106.754 133.716 111.681 127.662 112.65 120.431C114.166 108.659 122.631 98.3172 134.717 95.08C149.583 91.086 165.08 99.158 170.555 113.158C171.187 114.797 170.302 116.647 168.576 117.11L156.237 120.431Z\" fill=\"black\"/>\n</svg>\n";
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