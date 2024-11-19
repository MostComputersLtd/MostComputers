using FluentValidation;
using OneOf.Types;
using OneOf;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Mapping;

using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts.ExternalXmlImport;
using MOSTComputers.Services.ProductRegister.Models.ExternalXmlImport.ProductImageFileNameInfo;
using MOSTComputers.Models.Product.Models.ExternalXmlImport;
using MOSTComputers.Services.ProductRegister.Services.Contracts.ExternalXmlImport;
using MOSTComputers.Services.DAL.Models.Requests.ExternalXmlImport.ProductImageFileNameInfo;

namespace MOSTComputers.Services.ProductRegister.Services.ExternalXmlImport;

internal sealed class XmlImportProductImageFileNameInfoService : IXmlImportProductImageFileNameInfoService
{
    public XmlImportProductImageFileNameInfoService(
        IXmlImportProductImageFileNameInfoRepository imageFileNameInfoRepository,
        ProductMapper mapper,
        IValidator<XmlImportServiceProductImageFileNameInfoCreateRequest>? createRequestValidator = null,
        IValidator<XmlImportServiceProductImageFileNameInfoByImageNumberUpdateRequest>? updateRequestByImageNumberValidator = null,
        IValidator<XmlImportServiceProductImageFileNameInfoByFileNameUpdateRequest>? updateRequestByFileNameValidator = null)
    {
        _imageFileNameInfoRepository = imageFileNameInfoRepository;
        _mapper = mapper;
        _createRequestValidator = createRequestValidator;
        _updateRequestValidator = updateRequestByImageNumberValidator;
        _updateRequestByFileNameValidator = updateRequestByFileNameValidator;
    }

    private readonly IXmlImportProductImageFileNameInfoRepository _imageFileNameInfoRepository;
    private readonly ProductMapper _mapper;
    private readonly IValidator<XmlImportServiceProductImageFileNameInfoCreateRequest>? _createRequestValidator;
    private readonly IValidator<XmlImportServiceProductImageFileNameInfoByImageNumberUpdateRequest>? _updateRequestValidator;
    private readonly IValidator<XmlImportServiceProductImageFileNameInfoByFileNameUpdateRequest>? _updateRequestByFileNameValidator;

    public IEnumerable<XmlImportProductImageFileNameInfo> GetAll()
    {
        return _imageFileNameInfoRepository.GetAll();
    }

    public IEnumerable<XmlImportProductImageFileNameInfo> GetAllInProduct(int productId)
    {
        if (productId <= 0) return Enumerable.Empty<XmlImportProductImageFileNameInfo>();

        return _imageFileNameInfoRepository.GetAllInProduct(productId);
    }

    public int? GetHighestImageNumber(int productId)
    {
        if (productId <= 0) return null;

        return _imageFileNameInfoRepository?.GetHighestImageNumber(productId);
    }

    public XmlImportProductImageFileNameInfo? GetByProductIdAndImageNumber(int productId, int imageNumber)
    {
        if (productId <= 0 || imageNumber <= 0) return null;

        return _imageFileNameInfoRepository.GetByProductIdAndImageNumber(productId, imageNumber);
    }

    public XmlImportProductImageFileNameInfo? GetByFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) return null;

        return _imageFileNameInfoRepository.GetByFileName(fileName);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Insert(XmlImportServiceProductImageFileNameInfoCreateRequest createRequest,
        IValidator<XmlImportServiceProductImageFileNameInfoCreateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(createRequest, validator, _createRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        XmlImportProductImageFileNameInfoCreateRequest createRequestInternal = _mapper.Map(createRequest);

        createRequestInternal.Active = createRequest.Active ?? false;

        return _imageFileNameInfoRepository.Insert(createRequestInternal);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByImageNumber(
        XmlImportServiceProductImageFileNameInfoByImageNumberUpdateRequest updateRequest,
        IValidator<XmlImportServiceProductImageFileNameInfoByImageNumberUpdateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(updateRequest, validator, _updateRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        XmlImportProductImageFileNameInfoByImageNumberUpdateRequest updateRequestInternal = _mapper.Map(updateRequest);

        updateRequestInternal.Active = updateRequest.Active ?? false;

        return _imageFileNameInfoRepository.UpdateByImageNumber(updateRequestInternal);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByFileName(
        XmlImportServiceProductImageFileNameInfoByFileNameUpdateRequest updateRequest,
        IValidator<XmlImportServiceProductImageFileNameInfoByFileNameUpdateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(updateRequest, validator, _updateRequestByFileNameValidator);

        if (!validationResult.IsValid) return validationResult;

        XmlImportProductImageFileNameInfoByFileNameUpdateRequest updateRequestInternal = _mapper.Map(updateRequest);

        updateRequestInternal.Active = updateRequest.Active ?? false;

        return _imageFileNameInfoRepository.UpdateByFileName(updateRequestInternal);
    }

    public bool DeleteAllForProductId(int productId)
    {
        if (productId <= 0) return false;

        return _imageFileNameInfoRepository.DeleteAllForProductId(productId);
    }

    public bool DeleteByProductIdAndDisplayOrder(int productId, int displayOrder)
    {
        if (productId <= 0 || displayOrder <= 0) return false;

        return _imageFileNameInfoRepository.DeleteByProductIdAndDisplayOrder(productId, displayOrder);
    }

    public bool DeleteByProductIdAndImageNumber(int productId, int imageNumber)
    {
        if (productId <= 0 || imageNumber <= 0) return false;

        return _imageFileNameInfoRepository.DeleteByProductIdAndImageNumber(productId, imageNumber);
    }
}