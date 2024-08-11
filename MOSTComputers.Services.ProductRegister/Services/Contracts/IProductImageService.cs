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
    IEnumerable<ProductImage> GetAllFirstImagesForSelectionOfProducts(List<int> productIds);
    IEnumerable<ProductImage> GetAllInProduct(int productId);
    ProductImage? GetByIdInAllImages(int id);
    ProductImage? GetFirstImageForProduct(int productId);
    OneOf<int, ValidationResult, UnexpectedFailureResult> InsertInAllImages(ServiceProductImageCreateRequest createRequest, IValidator<ServiceProductImageCreateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertInFirstImages(ServiceProductFirstImageCreateRequest createRequest, IValidator<ServiceProductFirstImageCreateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateInAllImages(ServiceProductImageUpdateRequest updateRequest, IValidator<ServiceProductImageUpdateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateInFirstImages(ServiceProductFirstImageUpdateRequest updateRequest, IValidator<ServiceProductFirstImageUpdateRequest>? validator = null);
    bool DeleteInFirstImagesByProductId(int productId);
    bool DeleteAllImagesForProduct(int productId);
    bool DeleteInAllImagesById(int id);
    OneOf<int, ValidationResult, UnexpectedFailureResult> InsertInAllImagesAndImageFileNameInfos(ServiceProductImageCreateRequest createRequest, int? displayOrder = null, IValidator<ServiceProductImageCreateRequest>? validator = null);
    bool DeleteInAllImagesAndImageFilePathInfosById(int id);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateHtmlDataInFirstAndAllImagesByProductId(int productId, string htmlData);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateHtmlDataInFirstImagesByProductId(int productId, string htmlData);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateHtmlDataInAllImagesById(int imageId, string htmlData);
}