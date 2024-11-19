using FluentValidation;
using OneOf.Types;
using OneOf;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.Validation;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts.ExternalXmlImport;
using MOSTComputers.Models.Product.Models.ExternalXmlImport;
using MOSTComputers.Services.ProductRegister.Services.Contracts.ExternalXmlImport;
using MOSTComputers.Services.DAL.Models.Requests.ExternalXmlImport.ProductProperty;

namespace MOSTComputers.Services.ProductRegister.Services.ExternalXmlImport;

internal sealed class XmlImportProductPropertyService : IXmlImportProductPropertyService
{
    public XmlImportProductPropertyService(
        IXmlImportProductPropertyRepository productPropertyRepository,
        IValidator<XmlImportProductPropertyByCharacteristicIdCreateRequest>? createRequestByIdValidator = null,
        IValidator<XmlImportProductPropertyByCharacteristicNameCreateRequest>? createRequestByNameValidator = null,
        IValidator<XmlImportProductPropertyUpdateByXmlDataRequest>? updateRequestValidator = null)
    {
        _productPropertyRepository = productPropertyRepository;
        _createRequestByIdValidator = createRequestByIdValidator;
        _createRequestByNameValidator = createRequestByNameValidator;
        _updateRequestValidator = updateRequestValidator;
    }

    private readonly IXmlImportProductPropertyRepository _productPropertyRepository;
    private readonly IValidator<XmlImportProductPropertyByCharacteristicIdCreateRequest>? _createRequestByIdValidator;
    private readonly IValidator<XmlImportProductPropertyByCharacteristicNameCreateRequest>? _createRequestByNameValidator;
    private readonly IValidator<XmlImportProductPropertyUpdateByXmlDataRequest>? _updateRequestValidator;

    public IEnumerable<XmlImportProductProperty> GetAllInProduct(int productId)
    {
        if (productId <= 0) return Enumerable.Empty<XmlImportProductProperty>();

        return _productPropertyRepository.GetAllInProduct(productId);
    }

    public XmlImportProductProperty? GetByNameAndProductId(string name, int productId)
    {
        if (productId <= 0) return null;

        return _productPropertyRepository.GetByNameAndProductId(name, productId);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertWithCharacteristicId(
        XmlImportProductPropertyByCharacteristicIdCreateRequest createRequest,
        IValidator<XmlImportProductPropertyByCharacteristicIdCreateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(createRequest, validator, _createRequestByIdValidator);

        if (!validationResult.IsValid) return validationResult;

        OneOf<Success, ValidationResult, UnexpectedFailureResult> result = _productPropertyRepository.InsertWithCharacteristicId(createRequest);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, validationResult => validationResult, unexpectedResult => unexpectedResult);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertWithCharacteristicName(
        XmlImportProductPropertyByCharacteristicNameCreateRequest createRequest,
        IValidator<XmlImportProductPropertyByCharacteristicNameCreateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(createRequest, validator, _createRequestByNameValidator);

        if (!validationResult.IsValid) return validationResult;

        OneOf<Success, ValidationResult, UnexpectedFailureResult> result = _productPropertyRepository.InsertWithCharacteristicName(createRequest);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, validationResult => validationResult, unexpectedResult => unexpectedResult);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByXmlData(XmlImportProductPropertyUpdateByXmlDataRequest updateRequest,
        IValidator<XmlImportProductPropertyUpdateByXmlDataRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(updateRequest, validator, _updateRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        OneOf<Success, ValidationResult, UnexpectedFailureResult> result = _productPropertyRepository.UpdateByXmlData(updateRequest);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, validationResult => validationResult, unexpectedResult => unexpectedResult);
    }

    public bool Delete(int productId, int characteristicId)
    {
        if (productId <= 0 || characteristicId <= 0) return false;

        return _productPropertyRepository.Delete(productId, characteristicId);
    }

    public bool DeleteAllForProduct(int productId)
    {
        if (productId <= 0) return false;

        return _productPropertyRepository.DeleteAllForProduct(productId);
    }

    public bool DeleteAllForCharacteristic(int characteristicId)
    {
        if (characteristicId <= 0) return false;

        return _productPropertyRepository.DeleteAllForCharacteristic(characteristicId);
    }
}