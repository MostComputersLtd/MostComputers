using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.ExternalXmlImport;
using MOSTComputers.Models.Product.Models.ExternalXmlImport.Requests.ProductProperty;
using MOSTComputers.Models.Product.Models.Validation;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts.ExternalXmlImport;
public interface IXmlImportProductPropertyService
{
    bool Delete(int productId, int characteristicId);
    bool DeleteAllForCharacteristic(int characteristicId);
    bool DeleteAllForProduct(int productId);
    IEnumerable<XmlImportProductProperty> GetAllInProduct(int productId);
    XmlImportProductProperty? GetByNameAndProductId(string name, int productId);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertWithCharacteristicId(XmlImportProductPropertyByCharacteristicIdCreateRequest createRequest, IValidator<XmlImportProductPropertyByCharacteristicIdCreateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertWithCharacteristicName(XmlImportProductPropertyByCharacteristicNameCreateRequest createRequest, IValidator<XmlImportProductPropertyByCharacteristicNameCreateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByXmlData(XmlImportProductPropertyUpdateByXmlDataRequest updateRequest, IValidator<XmlImportProductPropertyUpdateByXmlDataRequest>? validator = null);
}