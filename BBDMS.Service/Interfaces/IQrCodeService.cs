namespace BBDMS.Service.Interfaces
{
    public interface IQrCodeService
    {
        string GeneratePngBase64(string payload, int pixelsPerModule = 10);
        byte[] GeneratePngBytes(string payload, int pixelsPerModule = 10);
    }
}
