using MOSTComputers.Models.Product.Models;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Utils.ProductImageFileNameUtils;

public static class ProductImageFileNameUtils
{
    public static string? GetTemporaryFileNameWithoutExtension(int productId, int imageNumber)
    {
        if (productId <= 0
            || imageNumber <= 0) return null;

        return $"{productId}-{imageNumber}";
    }

    public static string? GetTemporaryFileNameFromFileNameInfoAndContentType(ProductImageFileNameInfo productImageFileNameInfo, string imageContentType)
    {
        if (productImageFileNameInfo.ProductId <= 0
            || productImageFileNameInfo.ImageNumber <= 0
            || string.IsNullOrWhiteSpace(imageContentType)) return null;

        string? fileExtensionFromContentType = GetImageFileExtensionFromContentType(imageContentType);

        if (fileExtensionFromContentType is null) return null;

        return $"{productImageFileNameInfo.ProductId}-{productImageFileNameInfo.ImageNumber}.{fileExtensionFromContentType}";
    }

    public static string? GetTemporaryIdFromFileNameInfoAndContentType(int productId, int imageNumber, string imageContentType)
    {
        if (productId <= 0
            || imageNumber <= 0
            || string.IsNullOrWhiteSpace(imageContentType)) return null;

        string? fileExtensionFromContentType = GetImageFileExtensionFromContentType(imageContentType);

        if (fileExtensionFromContentType is null) return null;

        return $"{productId}-{imageNumber}.{fileExtensionFromContentType}";
    }

    public static string? GetImageFileNameFromImageData(int imageId, string imageContentType)
    {
        if (imageId < 0) return null;

        string? fileExtension = GetImageFileExtensionFromContentType(imageContentType);

        if (fileExtension is null) return null;

        return $"{imageId}.{fileExtension}";
    }

    public static (string? imageIdOrTemporaryId, string? imageFileExtension) GetImageDataFromImageFileName(string imageFileName)
    {
        int indexOfDot = imageFileName.IndexOf('.');

        if (indexOfDot < 0) return (null, null);

        string imageIdOrTempId = imageFileName[..indexOfDot];

        string imageFileExtension = imageFileName[(indexOfDot + 1)..];

        return (imageIdOrTempId, imageFileExtension);
    }

    public static string? GetImageFileExtensionFromContentType(string imageContentType)
    {
        string? fileExtension = imageContentType switch
        {
            "image/jpeg" => "jpeg",
            "image/jpg" => "jpg",
            "image/png" => "png",
            _ => null
        };

        if (fileExtension is null)
        {
            int indexOfSlash = imageContentType.IndexOf('/');

            if (indexOfSlash < 0) return null;

            return imageContentType[(indexOfSlash + 1)..];
        }

        return fileExtension;
    }

    public static string? GetImageContentTypeFromFileExtension(string fileExtension)
    {
        if (fileExtension.StartsWith('.'))
        {
            fileExtension = fileExtension[1..];
        }

        string? contentType = fileExtension switch
        {
            "jpeg" => "image/jpeg",
            "jpg" => "image/jpg",
            "png" => "image/png",
            _ => null
        };

        return contentType;
    }
}