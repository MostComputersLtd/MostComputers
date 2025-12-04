using FluentValidation.Results;
using OneOf;

namespace MOSTComputers.UI.Web.Services.Data.ProductEditor.Contracts;
public interface IImageExtensionFromContentTypeService
{
    OneOf<string, ValidationResult> GetFileExtensionFromImageContentType(string? imageContentType);
}