using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.ExternalXmlImport;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Models.ExternalXmlImport.ProductImageFileNameInfo;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts.ExternalXmlImport;
public interface IXmlImportProductImageFileNameInfoService
{
    bool DeleteAllForProductId(int productId);
    bool DeleteByProductIdAndDisplayOrder(int productId, int displayOrder);
    bool DeleteByProductIdAndImageNumber(int productId, int imageNumber);
    IEnumerable<XmlImportProductImageFileNameInfo> GetAll();
    IEnumerable<XmlImportProductImageFileNameInfo> GetAllInProduct(int productId);
    XmlImportProductImageFileNameInfo? GetByFileName(string fileName);
    XmlImportProductImageFileNameInfo? GetByProductIdAndImageNumber(int productId, int imageNumber);
    int? GetHighestImageNumber(int productId);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> Insert(XmlImportServiceProductImageFileNameInfoCreateRequest createRequest, IValidator<XmlImportServiceProductImageFileNameInfoCreateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByFileName(XmlImportServiceProductImageFileNameInfoByFileNameUpdateRequest updateRequest, IValidator<XmlImportServiceProductImageFileNameInfoByFileNameUpdateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByImageNumber(XmlImportServiceProductImageFileNameInfoByImageNumberUpdateRequest updateRequest, IValidator<XmlImportServiceProductImageFileNameInfoByImageNumberUpdateRequest>? validator = null);
}