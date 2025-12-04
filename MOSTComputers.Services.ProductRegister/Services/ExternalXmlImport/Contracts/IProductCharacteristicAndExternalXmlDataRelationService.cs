using MOSTComputers.Models.Product.Models.ExternalXmlImport;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ExternalXmlImport;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.ExternalXmlImport.Contracts;
public interface IProductCharacteristicAndExternalXmlDataRelationService
{
    Task<List<ProductCharacteristicAndExternalXmlDataRelation>> GetAllAsync();
    Task<List<ProductCharacteristicAndExternalXmlDataRelation>> GetAllWithSameCategoryIdAsync(int categoryId);
    Task<List<ProductCharacteristicAndExternalXmlDataRelation>> GetAllWithSameCategoryIdAndXmlNameAsync(int categoryId, string xmlName);
    Task<List<ProductCharacteristicAndExternalXmlDataRelation>> GetAllWithSameCategoryIdsAsync(IEnumerable<int> categoryIds);
    Task<List<ProductCharacteristicAndExternalXmlDataRelation>> GetAllWithCharacteristicIdAsync(int characteristicId);
    Task<ProductCharacteristicAndExternalXmlDataRelation?> GetByIdAsync(int id);
    Task<OneOf<Success, UnexpectedFailureResult>> UpsertByCharacteristicIdAsync(ProductCharacteristicAndExternalXmlDataRelationUpsertRequest createRequest);
    Task<bool> DeleteAllWithSameCategoryIdAsync(int categoryId);
    Task<bool> DeleteAllWithSameCategoryIdAndXmlNameAsync(int categoryId, string xmlName);
    Task<bool> DeleteByCharacteristicIdAsync(int characteristicId);
    Task<bool> DeleteByIdAsync(int id);
}