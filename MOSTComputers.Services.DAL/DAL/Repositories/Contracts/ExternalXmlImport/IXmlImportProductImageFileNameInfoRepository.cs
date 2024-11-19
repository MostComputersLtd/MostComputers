using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.ExternalXmlImport;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.Models.Requests.ExternalXmlImport.ProductImageFileNameInfo;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts.ExternalXmlImport;
public interface IXmlImportProductImageFileNameInfoRepository
{
    int? ImagesInImagesAllForProductCount { get; set; }
    bool IsProductFirstImageInImages { get; set; }

    bool DeleteAllForProductId(int productId);
    bool DeleteByProductIdAndDisplayOrder(int productId, int displayOrder);
    bool DeleteByProductIdAndImageNumber(int productId, int imageNumber);
    IEnumerable<XmlImportProductImageFileNameInfo> GetAll();
    IEnumerable<XmlImportProductImageFileNameInfo> GetAllInProduct(int productId);
    XmlImportProductImageFileNameInfo? GetByFileName(string fileName);
    XmlImportProductImageFileNameInfo? GetByProductIdAndImageNumber(int productId, int imageNumber);
    int? GetHighestImageNumber(int productId);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> Insert(XmlImportProductImageFileNameInfoCreateRequest createRequest);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByFileName(XmlImportProductImageFileNameInfoByFileNameUpdateRequest updateRequest);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByImageNumber(XmlImportProductImageFileNameInfoByImageNumberUpdateRequest updateRequest);
}