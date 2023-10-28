using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.Models;
using MOSTComputers.Services.DAL.Models.Requests.ProductImage;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts
{
    internal interface IProductImageRepository
    {
        bool DeleteAllWithSameProductIdInAllImages(uint productId);
        bool DeleteInAllImagesById(uint id);
        bool DeleteInFirstImagesByProductId(uint id);
        IEnumerable<ProductImage> GetAllFirstImagesForAllProducts();
        IEnumerable<ProductImage> GetAllInProduct(uint productId);
        ProductImage? GetByIdInAllImages(uint id);
        ProductImage? GetByProductIdInFirstImages(uint productId);
        OneOf<Success, ValidationResult> InsertInAllImages(ProductImageCreateRequest createRequest, IValidator<ProductImageCreateRequest>? validator = null);
        OneOf<Success, ValidationResult> InsertInFirstImages(ProductFirstImageCreateRequest createRequest, IValidator<ProductFirstImageCreateRequest>? validator = null);
        OneOf<Success, ValidationResult> UpdateInAllImages(ProductImageUpdateRequest createRequest, IValidator<ProductImageUpdateRequest>? validator = null);
        OneOf<Success, ValidationResult> UpdateInFirstImages(ProductFirstImageUpdateRequest createRequest, IValidator<ProductFirstImageUpdateRequest>? validator = null);
    }
}