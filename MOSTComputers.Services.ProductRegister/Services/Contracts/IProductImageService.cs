using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.Models;
using MOSTComputers.Services.DAL.Models.Requests.ProductImage;
using MOSTComputers.Services.DAL.Models.Responses;
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
    OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertInAllImages(ProductImageCreateRequest createRequest, IValidator<ProductImageCreateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertInFirstImages(ProductFirstImageCreateRequest createRequest, IValidator<ProductFirstImageCreateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateInAllImages(ProductImageUpdateRequest updateRequest, IValidator<ProductImageUpdateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateInFirstImages(ProductFirstImageUpdateRequest updateRequest, IValidator<ProductFirstImageUpdateRequest>? validator = null);
    bool DeleteInFirstImagesByProductId(uint productId);
    bool DeleteAllImagesForProduct(uint productId);
    bool DeleteInAllImagesById(uint id);
}