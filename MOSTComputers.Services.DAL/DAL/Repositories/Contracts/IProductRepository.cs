using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models.Validation;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts;

public interface IProductRepository
{
    IEnumerable<Product> GetAll_WithManifacturerAndCategory();
    IEnumerable<Product> GetAll_WithManifacturerAndCategoryAndFirstImage_ByIds(List<uint> ids);
    IEnumerable<Product> GetAll_WithManifacturerAndCategoryAndProperties_ByIds(List<uint> ids);
    IEnumerable<Product> GetAll_WithManifacturerAndCategory_ByIds(List<uint> ids);
    IEnumerable<Product> GetFirstBetweenStartAndEnd_WithCategoryAndManifacturer(uint start, uint end);
    Product? GetById_WithManifacturerAndCategoryAndFirstImage(uint id);
    Product? GetById_WithManifacturerAndCategoryAndImages(uint id);
    Product? GetById_WithManifacturerAndCategoryAndProperties(uint id);
    OneOf<uint, UnexpectedFailureResult> Insert(ProductCreateRequest createRequest);
    OneOf<Success, UnexpectedFailureResult> Update(ProductUpdateRequest updateRequest);
    bool Delete(uint id);
}