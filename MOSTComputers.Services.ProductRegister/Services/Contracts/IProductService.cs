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
    Product? GetByIdWithFirstImage(uint id);
    Product? GetByIdWithProps(uint id);
    IEnumerable<Product> GetSelectionWithFirstImage(List<uint> ids);
    Product? GetSelectionWithImages(uint id);
    IEnumerable<Product> GetSelectionWithoutImagesAndProps(List<uint> ids);
    IEnumerable<Product> GetSelectionWithProps(List<uint> ids);
    OneOf<uint, ValidationResult, UnexpectedFailureResult> Insert(ProductCreateRequest createRequest, IValidator<ProductCreateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ProductUpdateRequest updateRequest, IValidator<ProductUpdateRequest>? validator = null);
    bool Delete(uint id);
}