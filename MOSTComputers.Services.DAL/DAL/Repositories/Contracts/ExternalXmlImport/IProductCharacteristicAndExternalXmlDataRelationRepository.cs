using MOSTComputers.Models.Product.Models.ExternalXmlImport;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.Models.Requests.ExternalXmlImport;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts.ExternalXmlImport;
public interface IProductCharacteristicAndExternalXmlDataRelationRepository
{
    List<ProductCharacteristicAndExternalXmlDataRelation> GetAll();
    List<ProductCharacteristicAndExternalXmlDataRelation> GetAllWithSameCategoryId(int categoryId);
    List<ProductCharacteristicAndExternalXmlDataRelation> GetAllWithSameCategoryIdAndXmlName(int categoryId, string xmlName);
    ProductCharacteristicAndExternalXmlDataRelation? GetByCharacteristicId(int characteristicId);
    ProductCharacteristicAndExternalXmlDataRelation? GetById(int id);
    OneOf<Success, UnexpectedFailureResult> UpsertByCharacteristicId(ProductCharacteristicAndExternalXmlDataRelationUpsertRequest createRequest);
    bool DeleteAllWithSameCategoryId(int categoryId);
    bool DeleteAllWithSameCategoryIdAndXmlName(int categoryId, string xmlName);
    bool DeleteByCharacteristicId(int characteristicId);
    bool DeleteById(int id);
}