using MOSTComputers.Models.Product.Models;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Utils.ProductImageFileNameUtils;

public static class ProductImageFileNameUtils
{
    public static OneOf<int, (int, int), False> GetImageIdOrTempIdFromImageFileName(string fileName)
    {
        int endIndexOfIdOfImageFromNameOfFileInfo = fileName.IndexOf('.');

        if (endIndexOfIdOfImageFromNameOfFileInfo < 0) return new False();

        string idOfImageAsString = fileName[..endIndexOfIdOfImageFromNameOfFileInfo];

        bool succeededGettingIdFromFileInfoName = int.TryParse(idOfImageAsString, out int idOfImage);

        if (succeededGettingIdFromFileInfoName) return idOfImage;

        int indexOfSeparatorOfTempId = idOfImageAsString.IndexOf('-');

        if (indexOfSeparatorOfTempId < 0) return new False();

        string productIdPartOfTempIdAsString = idOfImageAsString[..indexOfSeparatorOfTempId];

        bool isProductIdParsed = int.TryParse(productIdPartOfTempIdAsString, out int productId);

        if (!isProductIdParsed) return new False();

        string imageNumberPartOfTempIdAsString = idOfImageAsString[(indexOfSeparatorOfTempId + 1)..];

        bool isImageNumberParsed = int.TryParse(imageNumberPartOfTempIdAsString, out int imageNumber);

        if (!isImageNumberParsed) return new False();

        return (productId, imageNumber);
    }

    public static string? GetTemporaryIdFromFileNameInfoAndContentType(ProductImageFileNameInfo productImageFileNameInfo, string imageContentType)
    {
        if (productImageFileNameInfo.ProductId <= 0
            || productImageFileNameInfo.ImageNumber <= 0
            || string.IsNullOrWhiteSpace(imageContentType)) return null;

        string? fileExtensionFromContentType = GetImageFileExtensionFromContentType(imageContentType);

        if (fileExtensionFromContentType is null) return null;

        return $"{productImageFileNameInfo.ProductId}-{productImageFileNameInfo.ImageNumber}.{fileExtensionFromContentType}";
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