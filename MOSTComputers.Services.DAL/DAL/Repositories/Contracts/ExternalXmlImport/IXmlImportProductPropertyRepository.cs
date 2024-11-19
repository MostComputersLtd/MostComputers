using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.ExternalXmlImport;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.Models.Requests.ExternalXmlImport.ProductProperty;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts.ExternalXmlImport;
public interface IXmlImportProductPropertyRepository
{
    bool Delete(int productId, int characteristicId);
    bool DeleteAllForCharacteristic(int characteristicId);
    bool DeleteAllForProduct(int productId);
    IEnumerable<XmlImportProductProperty> GetAllInProduct(int productId);
    XmlImportProductProperty? GetByNameAndProductId(string name, int productId);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertWithCharacteristicId(XmlImportProductPropertyByCharacteristicIdCreateRequest createRequest);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertWithCharacteristicName(XmlImportProductPropertyByCharacteristicNameCreateRequest createRequest);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByXmlData(XmlImportProductPropertyUpdateByXmlDataRequest updateRequest);
}