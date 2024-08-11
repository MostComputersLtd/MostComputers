using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models.Validation;
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
    OneOf<int, ValidationResult, UnexpectedFailureResult> Insert(ProductCreateRequest createRequest, IValidator<ProductCreateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ProductUpdateRequest updateRequest, IValidator<ProductUpdateRequest>? validator = null);
    bool Delete(int id);
    Product? GetProductWithHighestId();
    Product? GetProductFull(int productId);
}