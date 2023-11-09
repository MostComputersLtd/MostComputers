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

namespace MOSTComputers.Services.ProductRegister.Services;

internal sealed class ProductImageFileNameInfoService : IProductImageFileNameInfoService
{
    public ProductImageFileNameInfoService(
        IProductImageFileNameInfoRepository imageFileNameInfoRepository,
        IValidator<ProductImageFileNameInfoCreateRequest>? createRequestValidator = null,
        IValidator<ProductImageFileNameInfoUpdateRequest>? updateRequestValidator = null)
    {
        _imageFileNameInfoRepository = imageFileNameInfoRepository;
        _createRequestValidator = createRequestValidator;
        _updateRequestValidator = updateRequestValidator;
    }

    private readonly IProductImageFileNameInfoRepository _imageFileNameInfoRepository;
    private readonly IValidator<ProductImageFileNameInfoCreateRequest>? _createRequestValidator;
    private readonly IValidator<ProductImageFileNameInfoUpdateRequest>? _updateRequestValidator;

    public IEnumerable<ProductImageFileNameInfo> GetAll()
    {
        return _imageFileNameInfoRepository.GetAll();
    }

    public IEnumerable<ProductImageFileNameInfo> GetAllForProduct(uint productId)
    {
        return _imageFileNameInfoRepository.GetAllForProduct(productId);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Insert(ProductImageFileNameInfoCreateRequest createRequest,
        IValidator<ProductImageFileNameInfoCreateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(createRequest, validator, _createRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        return _imageFileNameInfoRepository.Insert(createRequest);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ProductImageFileNameInfoUpdateRequest updateRequest,
        IValidator<ProductImageFileNameInfoUpdateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(updateRequest, validator, _updateRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        return _imageFileNameInfoRepository.Update(updateRequest);
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