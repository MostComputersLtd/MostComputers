using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.Models;
using MOSTComputers.Services.DAL.Models.Requests.Product;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
internal interface IProductRepository
{
    bool Delete(uint id);
    IEnumerable<Product> GetAll_WithManifacturerAndCategory();
    IEnumerable<Product> GetAll_WithManifacturerAndCategoryAndFirstImage_ByIds(List<uint> ids);
    IEnumerable<Product> GetAll_WithManifacturerAndCategoryAndProperties_ByIds(List<uint> ids);
    IEnumerable<Product> GetAll_WithManifacturerAndCategory_ByIds(List<uint> ids);
    Product? GetById_WithManifacturerAndCategoryAndFirstImage(uint id);
    Product? GetById_WithManifacturerAndCategoryAndImages(uint id);
    Product? GetById_WithManifacturerAndCategoryAndProperties(uint id);
    OneOf<Success, ValidationResult> Insert(ProductCreateRequest createRequest, IValidator<ProductCreateRequest>? validator = null);
    OneOf<Success, ValidationResult> Update(ProductUpdateRequest createRequest, IValidator<ProductUpdateRequest>? validator = null);
}