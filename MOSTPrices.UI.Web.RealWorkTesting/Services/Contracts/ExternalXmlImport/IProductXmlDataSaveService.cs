using FluentValidation.Results;
using OneOf;
using OneOf.Types;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models.ExternalXmlImport;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models;

namespace MOSTComputers.UI.Web.RealWorkTesting.Services.Contracts.ExternalXmlImport;
public interface IProductXmlDataSaveService
{
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpsertProductPropertiesFromXmlPropertiesBasedOnRelationData(IEnumerable<XmlProduct> xmlProducts, List<ProductCharacteristicAndExternalXmlDataRelation> characteristicAndPropertyRelations);
    Task<OneOf<Success, ValidationResult, UnexpectedFailureResult, NotSupportedFileTypeResult, DirectoryNotFoundResult>> UpsertImageFileNamesAndFilesFromXmlDataAsync(IEnumerable<XmlProduct> xmlProducts, bool insertFiles = false);
    Task<OneOf<Success, ValidationResult, InvalidXmlResult, UnexpectedFailureResult>> UpsertTestingImagesFromXmlDataAsync(IEnumerable<XmlProduct> xmlProducts);
}