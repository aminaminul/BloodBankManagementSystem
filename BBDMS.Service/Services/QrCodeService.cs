using System;
using BBDMS.Service.Interfaces;
using QRCoder;

namespace BBDMS.Service.Services
{
    public class QrCodeService : IQrCodeService
    {
        public string GeneratePngBase64(string payload, int pixelsPerModule = 10)
        {
            var bytes = GeneratePngBytes(payload, pixelsPerModule);
            return $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
        }

        public byte[] GeneratePngBytes(string payload, int pixelsPerModule = 10)
        {
            if (string.IsNullOrWhiteSpace(payload))
            {
                payload = "BBDMS";
            }

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.M);
            var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(pixelsPerModule);
        }
    }
}
