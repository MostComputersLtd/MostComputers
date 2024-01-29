using FluentValidation;
using OneOf.Types;
using OneOf;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using FluentValidation.Results;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Models.Product.Models.Requests.ProductImageFileNameInfo;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileNameInfo;
using MOSTComputers.Services.ProductRegister.Mapping;

namespace MOSTComputers.Services.ProductRegister.Services;

internal sealed class ProductImageFileNameInfoService : IProductImageFileNameInfoService
{
    public ProductImageFileNameInfoService(
        IProductImageFileNameInfoRepository imageFileNameInfoRepository,
        ProductMapper mapper,
        IValidator<ServiceProductImageFileNameInfoCreateRequest>? createRequestValidator = null,
        IValidator<ServiceProductImageFileNameInfoUpdateRequest>? updateRequestValidator = null)
    {
        _imageFileNameInfoRepository = imageFileNameInfoRepository;
        _mapper = mapper;
        _createRequestValidator = createRequestValidator;
        _updateRequestValidator = updateRequestValidator;
    }

    private readonly IProductImageFileNameInfoRepository _imageFileNameInfoRepository;
    private readonly ProductMapper _mapper;
    private readonly IValidator<ServiceProductImageFileNameInfoCreateRequest>? _createRequestValidator;
    private readonly IValidator<ServiceProductImageFileNameInfoUpdateRequest>? _updateRequestValidator;

    public IEnumerable<ProductImageFileNameInfo> GetAll()
    {
        return _imageFileNameInfoRepository.GetAll();
    }

    public IEnumerable<ProductImageFileNameInfo> GetAllForProduct(uint productId)
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

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ServiceProductImageFileNameInfoUpdateRequest updateRequest,
        IValidator<ServiceProductImageFileNameInfoUpdateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(updateRequest, validator, _updateRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        ProductImageFileNameInfoUpdateRequest updateRequestInternal = _mapper.Map(updateRequest);

        updateRequestInternal.Active = updateRequest.Active ?? false;

        return _imageFileNameInfoRepository.Update(updateRequestInternal);
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