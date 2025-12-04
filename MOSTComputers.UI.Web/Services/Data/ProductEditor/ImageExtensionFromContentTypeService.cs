using FluentValidation.Results;
using OneOf;

using static MOSTComputers.Utils.Files.ContentTypeUtils;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.UI.Web.Services.Data.ProductEditor.Contracts;

namespace MOSTComputers.UI.Web.Services.Data.ProductEditor;

internal sealed class ImageExtensionFromContentTypeService : IImageExtensionFromContentTypeService
{
    public OneOf<string, ValidationResult> GetFileExtensionFromImageContentType(string? imageContentType)
    {
        if (!IsImageContentType(imageContentType))
        {
            ValidationFailure validationFailure = new(nameof(ProductImage.ImageContentType), "Image content type is invalid");

            return new ValidationResult([validationFailure]);
        }

        string? fileName = TEMP__GetImageFileExtensionFromImageData(imageContentType);

        if (fileName is null)
        {
            ValidationFailure validationFailure = new(nameof(ProductImage.ImageContentType), "File name is invalid");

            return new ValidationResult([validationFailure]);
        }

        string fileExtension = Path.GetExtension(fileName);

        if (fileExtension is null)
        {
            ValidationFailure validationFailure = new(nameof(ProductImage.ImageContentType), "File name is invalid");

            return new ValidationResult([validationFailure]);
        }

        return fileExtension;
    }

    private static string? TEMP__GetImageFileExtensionFromImageData(string imageContentType)
    {
        if (imageContentType == "image/jpeg") return ".jpeg";

        List<string> possibleFileExtensions = GetPossibleExtensionsFromContentType(imageContentType);

        string? extensionToUse = possibleFileExtensions.FirstOrDefault();

        return extensionToUse;
    }
}