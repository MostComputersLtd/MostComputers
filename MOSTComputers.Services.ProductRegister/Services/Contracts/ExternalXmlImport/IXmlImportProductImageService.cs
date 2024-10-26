using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ExternalXmlImport;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Models.ExternalXmlImport.ProductImage;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts.ExternalXmlImport;
public interface IXmlImportProductImageService
{
    IEnumerable<XmlImportProductImage> GetAllFirstImagesForAllProducts();
    IEnumerable<XmlImportProductImage> GetAllFirstImagesForSelectionOfProducts(List<int> productIds);
    IEnumerable<XmlImportProductImage> GetAllInProduct(int productId);
    XmlImportProductImage? GetByIdInAllImages(int id);
    XmlImportProductImage? GetFirstImageForProduct(int productId);
    OneOf<int, ValidationResult, UnexpectedFailureResult> UpsertInAllImages(XmlImportServiceProductImageCreateRequest createRequest, IValidator<XmlImportServiceProductImageCreateRequest>? validator = null);
    OneOf<int, ValidationResult, UnexpectedFailureResult> InsertInAllImagesAndImageFileNameInfos(XmlImportServiceProductImageCreateRequest createRequest, int? displayOrder = null, IValidator<XmlImportServiceProductImageCreateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertInFirstImages(XmlImportServiceProductFirstImageCreateRequest createRequest, IValidator<XmlImportServiceProductFirstImageCreateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpsertFirstAndAllImagesForProduct(int productId, List<ImageAndImageFileNameUpsertRequest> imageAndFileNameUpsertRequests, List<XmlImportProductImage>? oldProductImages = null);
    OneOf<bool, ValidationResult, UnexpectedFailureResult> UpdateHtmlDataInAllImagesById(int imageId, string htmlData);
    OneOf<bool, ValidationResult, UnexpectedFailureResult> UpdateHtmlDataInFirstAndAllImagesByProductId(int productId, string htmlData);
    OneOf<bool, ValidationResult, UnexpectedFailureResult> UpdateHtmlDataInFirstImagesByProductId(int productId, string htmlData);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateInAllImages(XmlImportServiceProductImageUpdateRequest updateRequest, IValidator<XmlImportServiceProductImageUpdateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateInFirstImages(XmlImportServiceProductFirstImageUpdateRequest updateRequest, IValidator<XmlImportServiceProductFirstImageUpdateRequest>? validator = null);
    bool DeleteAllImagesForProduct(int productId);
    bool DeleteInAllImagesAndImageFilePathInfosById(int id);
    bool DeleteInAllImagesById(int id);
    bool DeleteInFirstImagesByProductId(int productId);
}