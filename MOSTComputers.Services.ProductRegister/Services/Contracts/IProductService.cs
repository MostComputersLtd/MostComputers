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
    IEnumerable<Product> GetAllWhereSearchStringMatches(string subString);
    IEnumerable<Product> GetFirstItemsBetweenStartAndEnd(ProductRangeSearchRequest rangeSearchRequest);
    IEnumerable<Product> GetFirstInRangeWhereSearchStringMatches(ProductRangeSearchRequest productRangeSearchRequest, string subString);
    IEnumerable<Product> GetFirstInRangeWhereNameMatches(ProductRangeSearchRequest productRangeSearchRequest, string subString);
    IEnumerable<Product> GetAllWhereNameMatches(string subString);
    IEnumerable<Product> GetFirstInRangeWhereAllConditionsAreMet(ProductRangeSearchRequest productRangeSearchRequest, ProductConditionalSearchRequest productConditionalSearchRequest);
    Product? GetByIdWithFirstImage(uint id);
    Product? GetByIdWithProps(uint id);
    IEnumerable<Product> GetSelectionWithFirstImage(List<uint> ids);
    Product? GetByIdWithImages(uint id);
    IEnumerable<Product> GetSelectionWithoutImagesAndProps(List<uint> ids);
    IEnumerable<Product> GetSelectionWithProps(List<uint> ids);
    OneOf<uint, ValidationResult, UnexpectedFailureResult> Insert(ProductCreateRequest createRequest, IValidator<ProductCreateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ProductUpdateRequest updateRequest, IValidator<ProductUpdateRequest>? validator = null);
    bool Delete(uint id);
}