using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace MOSTComputers.Services.ProductImageFileManagement.Utils;
internal static class ImageUtils
{
    internal static IImageEncoder? TryDetectDecoder(this Image image, string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath)) return null;

        string ext = Path.GetExtension(filePath);

        image.Configuration.ImageFormatsManager.TryFindFormatByFileExtension(ext, out IImageFormat? format);

        if (format is null) return null;

        IImageEncoder? encoder = image.Configuration.ImageFormatsManager.GetEncoder(format);

        if (encoder is null) return null;

        return encoder;
    }
}