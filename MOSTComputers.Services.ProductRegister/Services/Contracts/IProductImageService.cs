using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.ProductImage;
using MOSTComputers.Models.Product.Models.Validation;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts;

public interface IProductImageService
{
    IEnumerable<ProductImage> GetAllFirstImagesForAllProducts();
    IEnumerable<ProductImage> GetAllFirstImagesForAllProducts(List<uint> productIds);
    IEnumerable<ProductImage> GetAllInProduct(uint productId);
    ProductImage? GetByIdInAllImages(uint id);
    ProductImage? GetFirstImageForProduct(uint productId);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertInAllImages(ServiceProductImageCreateRequest createRequest, IValidator<ServiceProductImageCreateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertInFirstImages(ServiceProductFirstImageCreateRequest createRequest, IValidator<ServiceProductFirstImageCreateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateInAllImages(ServiceProductImageUpdateRequest updateRequest, IValidator<ServiceProductImageUpdateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateInFirstImages(ServiceProductFirstImageUpdateRequest updateRequest, IValidator<ServiceProductFirstImageUpdateRequest>? validator = null);
    bool DeleteInFirstImagesByProductId(uint productId);
    bool DeleteAllImagesForProduct(uint productId);
    bool DeleteInAllImagesById(uint id);
}