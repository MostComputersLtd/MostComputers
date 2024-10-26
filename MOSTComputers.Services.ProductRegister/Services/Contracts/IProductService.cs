using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductImageFileManagement.Models;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts;

public interface IProductService
{
    IEnumerable<Product> GetAllWithoutImagesAndProps();
    IEnumerable<Product> GetAllWhereSearchStringMatches(string searchStringParts);
    IEnumerable<Product> GetAllWhereNameMatches(string subString);
    IEnumerable<Product> GetFirstItemsBetweenStartAndEnd(ProductRangeSearchRequest rangeSearchRequest);
    IEnumerable<Product> GetFirstInRangeWhereSearchStringMatches(ProductRangeSearchRequest productRangeSearchRequest, string subString);
    IEnumerable<Product> GetFirstInRangeWhereNameMatches(ProductRangeSearchRequest productRangeSearchRequest, string subString);
    IEnumerable<Product> GetFirstInRangeWhereAllConditionsAreMet(ProductRangeSearchRequest productRangeSearchRequest, ProductConditionalSearchRequest productConditionalSearchRequest);
    IEnumerable<Product> GetSelectionWithFirstImage(List<int> ids);
    IEnumerable<Product> GetSelectionWithoutImagesAndProps(List<int> ids);
    IEnumerable<Product> GetSelectionWithProps(List<int> ids);
    Product? GetByIdWithFirstImage(int id);
    Product? GetByIdWithProps(int id);
    Product? GetByIdWithImages(int id);
    Product? GetProductFull(int productId);
    Product? GetProductWithHighestId();
    OneOf<int, ValidationResult, UnexpectedFailureResult> Insert(ProductCreateRequest createRequest, IValidator<ProductCreateRequest>? validator = null);
    Task<OneOf<int, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>> InsertWithImagesOnlyInDirectoryAsync(ProductCreateWithoutImagesInDatabaseRequest productWithoutImagesInDBCreateRequest);
    Task<OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>> UpdateProductAndUpdateImagesOnlyInDirectoryAsync(ProductUpdateWithoutImagesInDatabaseRequest productUpdateWithoutImagesInDBRequest);
    Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpdateProductFullAsync(ProductFullUpdateRequest productFullUpdateRequest);
    bool Delete(int id);
    Product? GetProductFullWithHighestId();
    IEnumerable<Product> GetAllInCategoryWithoutImagesAndProps(int categoryId);
}