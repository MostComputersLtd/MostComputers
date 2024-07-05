using FluentValidation;
using OneOf.Types;
using OneOf;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using FluentValidation.Results;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Models.Product.Models.Requests.ProductImageFileNameInfo;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileNameInfo;
using MOSTComputers.Services.ProductRegister.Mapping;

using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Services;

internal sealed class ProductImageFileNameInfoService : IProductImageFileNameInfoService
{
    public ProductImageFileNameInfoService(
        IProductImageFileNameInfoRepository imageFileNameInfoRepository,
        ProductMapper mapper,
        IValidator<ServiceProductImageFileNameInfoCreateRequest>? createRequestValidator = null,
        IValidator<ServiceProductImageFileNameInfoByImageNumberUpdateRequest>? updateRequestByImageNumberValidator = null,
        IValidator<ServiceProductImageFileNameInfoByFileNameUpdateRequest>? updateRequestByFileNameValidator = null)
    {
        _imageFileNameInfoRepository = imageFileNameInfoRepository;
        _mapper = mapper;
        _createRequestValidator = createRequestValidator;
        _updateRequestValidator = updateRequestByImageNumberValidator;
        _updateRequestByFileNameValidator = updateRequestByFileNameValidator;
    }

    private readonly IProductImageFileNameInfoRepository _imageFileNameInfoRepository;
    private readonly ProductMapper _mapper;
    private readonly IValidator<ServiceProductImageFileNameInfoCreateRequest>? _createRequestValidator;
    private readonly IValidator<ServiceProductImageFileNameInfoByImageNumberUpdateRequest>? _updateRequestValidator;
    private readonly IValidator<ServiceProductImageFileNameInfoByFileNameUpdateRequest>? _updateRequestByFileNameValidator;

    public IEnumerable<ProductImageFileNameInfo> GetAll()
    {
        return _imageFileNameInfoRepository.GetAll();
    }

    public IEnumerable<ProductImageFileNameInfo> GetAllInProduct(uint productId)
    {
        return _imageFileNameInfoRepository.GetAllForProduct(productId);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Insert(ServiceProductImageFileNameInfoCreateRequest createRequest,
        IValidator<ServiceProductImageFileNameInfoCreateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(createRequest, validator, _createRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        ProductImageFileNameInfoCreateRequest createRequestInternal = _mapper.Map(createRequest);

        createRequestInternal.Active = createRequest.Active ?? false;

        return _imageFileNameInfoRepository.Insert(createRequestInternal);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ServiceProductImageFileNameInfoByImageNumberUpdateRequest updateRequest,
        IValidator<ServiceProductImageFileNameInfoByImageNumberUpdateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(updateRequest, validator, _updateRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        ProductImageFileNameInfoByImageNumberUpdateRequest updateRequestInternal = _mapper.Map(updateRequest);

        updateRequestInternal.Active = updateRequest.Active ?? false;

        return _imageFileNameInfoRepository.UpdateByImageNumber(updateRequestInternal);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByFileName(
        ServiceProductImageFileNameInfoByFileNameUpdateRequest updateRequest,
        IValidator<ServiceProductImageFileNameInfoByFileNameUpdateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(updateRequest, validator, _updateRequestByFileNameValidator);

        if (!validationResult.IsValid) return validationResult;

        ProductImageFileNameInfoByFileNameUpdateRequest updateRequestInternal = _mapper.Map(updateRequest);

        updateRequestInternal.Active = updateRequest.Active ?? false;

        return _imageFileNameInfoRepository.UpdateByFileName(updateRequestInternal);
    }

    public bool DeleteAllForProductId(uint productId)
    {
        return _imageFileNameInfoRepository.DeleteAllForProductId(productId);
    }

    public bool DeleteByProductIdAndDisplayOrder(uint productId, int displayOrder)
    {
        return _imageFileNameInfoRepository.DeleteByProductIdAndDisplayOrder(productId, displayOrder);
    }
}