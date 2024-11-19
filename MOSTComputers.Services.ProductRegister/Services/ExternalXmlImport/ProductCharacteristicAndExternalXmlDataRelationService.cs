using MOSTComputers.Models.Product.Models.ExternalXmlImport;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts.ExternalXmlImport;
using MOSTComputers.Services.DAL.Models.Requests.ExternalXmlImport;
using MOSTComputers.Services.ProductRegister.Services.Contracts.ExternalXmlImport;
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

    public List<ProductCharacteristicAndExternalXmlDataRelation> GetAll()
    {
        return _productCharacteristicAndExternalXmlDataRelationRepository.GetAll();
    }

    public List<ProductCharacteristicAndExternalXmlDataRelation> GetAllWithSameCategoryId(int categoryId)
    {
        return _productCharacteristicAndExternalXmlDataRelationRepository.GetAllWithSameCategoryId(categoryId);
    }

    public List<ProductCharacteristicAndExternalXmlDataRelation> GetAllWithSameCategoryIdAndXmlName(int categoryId, string xmlName)
    {
        return _productCharacteristicAndExternalXmlDataRelationRepository.GetAllWithSameCategoryIdAndXmlName(categoryId, xmlName);
    }

    public ProductCharacteristicAndExternalXmlDataRelation? GetByCharacteristicId(int characteristicId)
    {
        return _productCharacteristicAndExternalXmlDataRelationRepository.GetByCharacteristicId(characteristicId);
    }

    public ProductCharacteristicAndExternalXmlDataRelation? GetById(int id)
    {
        return _productCharacteristicAndExternalXmlDataRelationRepository.GetById(id);
    }

    public OneOf<Success, UnexpectedFailureResult> UpsertByCharacteristicId(ProductCharacteristicAndExternalXmlDataRelationUpsertRequest createRequest)
    {
        return _productCharacteristicAndExternalXmlDataRelationRepository.UpsertByCharacteristicId(createRequest);
    }

    public bool DeleteAllWithSameCategoryId(int categoryId)
    {
        return _productCharacteristicAndExternalXmlDataRelationRepository.DeleteAllWithSameCategoryId(categoryId);
    }

    public bool DeleteAllWithSameCategoryIdAndXmlName(int categoryId, string xmlName)
    {
        return _productCharacteristicAndExternalXmlDataRelationRepository.DeleteAllWithSameCategoryIdAndXmlName(categoryId, xmlName);
    }

    public bool DeleteByCharacteristicId(int characteristicId)
    {
        return _productCharacteristicAndExternalXmlDataRelationRepository.DeleteByCharacteristicId(characteristicId);
    }

    public bool DeleteById(int id)
    {
        return _productCharacteristicAndExternalXmlDataRelationRepository.DeleteById(id);
    }
}