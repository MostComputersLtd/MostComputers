using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.ProductImage;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
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
    OneOf<int, ValidationResult, UnexpectedFailureResult> InsertInAllImagesAndImageFileNameInfos(ServiceProductImageCreateRequest createRequest, int? displayOrder = null, IValidator<ServiceProductImageCreateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertInFirstImages(ServiceProductFirstImageCreateRequest createRequest, IValidator<ServiceProductFirstImageCreateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateInAllImages(ServiceProductImageUpdateRequest updateRequest, IValidator<ServiceProductImageUpdateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateInFirstImages(ServiceProductFirstImageUpdateRequest updateRequest, IValidator<ServiceProductFirstImageUpdateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpsertFirstAndAllImagesForProduct(int productId, List<ImageAndImageFileNameUpsertRequest> imageAndFileNameUpsertRequests, List<ProductImage>? oldProductImages = null);
    OneOf<bool, ValidationResult, UnexpectedFailureResult> UpdateHtmlDataInAllImagesById(int imageId, string htmlData);
    OneOf<bool, ValidationResult, UnexpectedFailureResult> UpdateHtmlDataInFirstImagesByProductId(int productId, string htmlData);
    OneOf<bool, ValidationResult, UnexpectedFailureResult> UpdateHtmlDataInFirstAndAllImagesByProductId(int productId, string htmlData);
    bool DeleteInFirstImagesByProductId(int productId);
    bool DeleteAllImagesForProduct(int productId);
    bool DeleteInAllImagesById(int id);
    bool DeleteInAllImagesAndImageFilePathInfosById(int id);
}