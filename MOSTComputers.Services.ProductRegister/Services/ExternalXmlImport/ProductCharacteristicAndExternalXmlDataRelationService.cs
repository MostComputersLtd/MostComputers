using MOSTComputers.Models.Product.Models.ExternalXmlImport;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.DataAccess.ExternalXmlImport.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ExternalXmlImport;
using MOSTComputers.Services.ProductRegister.Services.ExternalXmlImport.Contracts;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.ExternalXmlImport;
internal sealed class ProductCharacteristicAndExternalXmlDataRelationService : IProductCharacteristicAndExternalXmlDataRelationService
{
    public ProductCharacteristicAndExternalXmlDataRelationService(
        IProductCharacteristicAndExternalXmlDataRelationRepository productCharacteristicAndExternalXmlDataRelationRepository)
    {
        _productCharacteristicAndExternalXmlDataRelationRepository = productCharacteristicAndExternalXmlDataRelationRepository;
    }

    private readonly IProductCharacteristicAndExternalXmlDataRelationRepository _productCharacteristicAndExternalXmlDataRelationRepository;

    public async Task<List<ProductCharacteristicAndExternalXmlDataRelation>> GetAllAsync()
    {
        return await _productCharacteristicAndExternalXmlDataRelationRepository.GetAllAsync();
    }

    public async Task<List<ProductCharacteristicAndExternalXmlDataRelation>> GetAllWithSameCategoryIdAsync(int categoryId)
    {
        return await _productCharacteristicAndExternalXmlDataRelationRepository.GetAllWithSameCategoryIdAsync(categoryId);
    }

    public async Task<List<ProductCharacteristicAndExternalXmlDataRelation>> GetAllWithSameCategoryIdsAsync(IEnumerable<int> categoryIds)
    {
        return await _productCharacteristicAndExternalXmlDataRelationRepository.GetAllWithSameCategoryIdsAsync(categoryIds);
    }

    public async Task<List<ProductCharacteristicAndExternalXmlDataRelation>> GetAllWithSameCategoryIdAndXmlNameAsync(int categoryId, string xmlName)
    {
        return await _productCharacteristicAndExternalXmlDataRelationRepository.GetAllWithSameCategoryIdAndXmlNameAsync(categoryId, xmlName);
    }

    public async Task<List<ProductCharacteristicAndExternalXmlDataRelation>> GetAllWithCharacteristicIdAsync(int characteristicId)
    {
        return await _productCharacteristicAndExternalXmlDataRelationRepository.GetAllWithCharacteristicIdAsync(characteristicId);
    }

    public async Task<ProductCharacteristicAndExternalXmlDataRelation?> GetByIdAsync(int id)
    {
        return await _productCharacteristicAndExternalXmlDataRelationRepository.GetByIdAsync(id);
    }

    public async Task<OneOf<Success, UnexpectedFailureResult>> UpsertByCharacteristicIdAsync(ProductCharacteristicAndExternalXmlDataRelationUpsertRequest createRequest)
    {
        return await _productCharacteristicAndExternalXmlDataRelationRepository.UpsertByCharacteristicIdAsync(createRequest);
    }

    public async Task<bool> DeleteAllWithSameCategoryIdAsync(int categoryId)
    {
        return await _productCharacteristicAndExternalXmlDataRelationRepository.DeleteAllWithSameCategoryIdAsync(categoryId);
    }

    public async Task<bool> DeleteAllWithSameCategoryIdAndXmlNameAsync(int categoryId, string xmlName)
    {
        return await _productCharacteristicAndExternalXmlDataRelationRepository.DeleteAllWithSameCategoryIdAndXmlNameAsync(categoryId, xmlName);
    }

    public async Task<bool> DeleteByCharacteristicIdAsync(int characteristicId)
    {
        return await _productCharacteristicAndExternalXmlDataRelationRepository.DeleteByCharacteristicIdAsync(characteristicId);
    }

    public async Task<bool> DeleteByIdAsync(int id)
    {
        return await _productCharacteristicAndExternalXmlDataRelationRepository.DeleteByIdAsync(id);
    }
}