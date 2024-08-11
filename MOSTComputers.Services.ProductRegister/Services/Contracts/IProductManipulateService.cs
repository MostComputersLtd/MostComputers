using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductImageFileManagement.Models;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts;
public interface IProductManipulateService
{
    Task<OneOf<int, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>> InsertProductWithImagesOnlyInDirectoryAsync(ProductCreateWithoutImagesInDatabaseRequest productWithoutImagesInDBCreateRequest);
    OneOf<Success, ValidationResult, UnexpectedFailureResult, NotSupportedFileTypeResult> UpdateProductFull(ProductFullUpdateRequest productFullUpdateRequest);
    Task<OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>> UpdateProductWithoutSavingImagesInDBAsync(ProductUpdateWithoutImagesInDatabaseRequest productUpdateRequestWithoutImagesInDB);
}