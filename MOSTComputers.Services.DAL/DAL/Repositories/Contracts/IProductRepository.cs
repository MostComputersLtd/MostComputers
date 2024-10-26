using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models.Validation;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts;

public interface IProductRepository
{
    IEnumerable<Product> GetAll_WithManifacturerAndCategory();
    IEnumerable<Product> GetAll_WithManifacturerAndCategory_WhereSearchNameContainsSubstring(string substring);
    IEnumerable<Product> GetAll_WithManifacturerAndCategoryAndFirstImage_ByIds(List<int> ids);
    IEnumerable<Product> GetAll_WithManifacturerAndCategoryAndProperties_ByIds(List<int> ids);
    IEnumerable<Product> GetAll_WithManifacturerAndCategory_ByIds(List<int> ids);
    IEnumerable<Product> GetFirstBetweenStartAndEnd_WithCategoryAndManifacturer(int start, uint end);
    IEnumerable<Product> GetFirstInRange_WithManifacturerAndCategory_WhereNameContainsSubstring(int start, uint end, string subString);
    IEnumerable<Product> GetFirstInRange_WithManifacturerAndCategoryAndStatuses_WhereAllConditionsAreMet(int start, uint end, ProductConditionalSearchRequest productConditionalSearchRequest);
    Product? GetById_WithManifacturerAndCategoryAndFirstImage(int id);
    Product? GetById_WithManifacturerAndCategoryAndImages(int id);
    Product? GetById_WithManifacturerAndCategoryAndProperties(int id);
    Product? GetProductWithHighestId_WithManifacturerAndCategory();
    OneOf<int, ValidationResult, UnexpectedFailureResult> Insert(ProductCreateRequest createRequest);
    OneOf<Success, UnexpectedFailureResult> Update(ProductUpdateRequest updateRequest);
    bool Delete(int id);
    IEnumerable<Product> GetAll_WithManifacturerAndCategory_WhereSearchStringMatchesAllSearchStringParts(string searchStringParts);
    IEnumerable<Product> GetFirstInRange_WithManifacturerAndCategory_WhereSearchStringMatchesAllSearchStringParts(int start, uint end, string searchStringParts);
    IEnumerable<Product> GetAllInCategory_WithManifacturerAndCategory(int categoryId);
}