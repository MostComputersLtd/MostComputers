using MOSTComputers.Services.ProductImageFileManagement.Models;

namespace MOSTComputers.Services.ProductImageFileManagement.Utils;

public static class ProductImageFileManagementUtils
{
    public static AllowedImageFileType? GetAllowedImageFileTypeFromFileExtension(string extension)
    {
        return extension switch
        {
            "jpg" => AllowedImageFileType.JPG,
            "jpeg" => AllowedImageFileType.JPEG,
            "png" => AllowedImageFileType.PNG,
            _ => null
        };
    }

    public static string GetFileExtensionWithoutDot(string fullFileName)
    {
        string fileExtension = Path.GetExtension(fullFileName);

        return fileExtension[1..];
    }
}