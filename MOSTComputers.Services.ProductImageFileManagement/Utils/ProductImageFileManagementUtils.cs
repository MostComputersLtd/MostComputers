using MOSTComputers.Services.ProductImageFileManagement.Models;

namespace MOSTComputers.Services.ProductImageFileManagement.Utils;

public static class ProductImageFileManagementUtils
{
    public static AllowedImageFileType? GetAllowedImageFileTypeFromFileExtension(string extensionWithoutDot)
    {
        return extensionWithoutDot switch
        {
            "jpg" or "JPG" => AllowedImageFileType.JPG,
            "jpeg" or "JPEG" => AllowedImageFileType.JPEG,
            "png" or "PNG" => AllowedImageFileType.PNG,
            _ => null
        };
    }

    public static AllowedImageFileType? GetAllowedImageFileTypeFromContentType(string contentType)
    {
        return contentType switch
        {
            "image/jpg" => AllowedImageFileType.JPG,
            "image/jpeg" => AllowedImageFileType.JPEG,
            "image/png" => AllowedImageFileType.PNG,
            _ => null
        };
    }

    public static string? GetFileExtensionWithoutDot(string fullFileName)
    {
        string? fileExtension = Path.GetExtension(fullFileName);

        if (string.IsNullOrWhiteSpace(fileExtension)) return null;

        return fileExtension[1..];
    }
}