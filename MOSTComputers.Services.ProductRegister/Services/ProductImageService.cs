using FluentValidation;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using OneOf.Types;
using OneOf;
using FluentValidation.Results;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Models.Product.Models.Requests.ProductImage;
using MOSTComputers.Services.ProductRegister.Mapping;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Services;

internal sealed class ProductImageService : IProductImageService
{
    public ProductImageService(
        IProductImageRepository productImageRepository,
        ProductMapper productMapper,
        IValidator<ServiceProductImageCreateRequest>? createRequestValidator = null,
        IValidator<ServiceProductImageUpdateRequest>? updateRequestValidator = null,
        IValidator<ServiceProductFirstImageCreateRequest>? firstImageCreateRequestValidator = null,
        IValidator<ServiceProductFirstImageUpdateRequest>? firstImageUpdateRequestValidator = null)
    {
        _productImageRepository = productImageRepository;
        _productMapper = productMapper;
        _createRequestValidator = createRequestValidator;
        _updateRequestValidator = updateRequestValidator;
        _firstImageCreateRequestValidator = firstImageCreateRequestValidator;
        _firstImageUpdateRequestValidator = firstImageUpdateRequestValidator;
    }

    private readonly IProductImageRepository _productImageRepository;
    private readonly ProductMapper _productMapper;
    private readonly IValidator<ServiceProductImageCreateRequest>? _createRequestValidator;
    private readonly IValidator<ServiceProductImageUpdateRequest>? _updateRequestValidator;
    private readonly IValidator<ServiceProductFirstImageCreateRequest>? _firstImageCreateRequestValidator;
    private readonly IValidator<ServiceProductFirstImageUpdateRequest>? _firstImageUpdateRequestValidator;

    public IEnumerable<ProductImage> GetAllInProduct(uint productId)
    {
        return _productImageRepository.GetAllInProduct(productId);
    }

    public IEnumerable<ProductImage> GetAllFirstImagesForAllProducts()
    {
        return _productImageRepository.GetAllFirstImagesForAllProducts();
    }

    public IEnumerable<ProductImage> GetAllFirstImagesForSelectionOfProducts(List<uint> productIds)
    {
        return _productImageRepository.GetFirstImagesForSelectionOfProducts(productIds);
    }

    public ProductImage? GetByIdInAllImages(uint id)
    {
        return _productImageRepository.GetByIdInAllImages(id);
    }

    public ProductImage? GetFirstImageForProduct(uint productId)
    {
        return _productImageRepository.GetByProductIdInFirstImages(productId);
    }

    public OneOf<uint, ValidationResult, UnexpectedFailureResult> InsertInAllImages(ServiceProductImageCreateRequest createRequest,
        IValidator<ServiceProductImageCreateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(createRequest, validator, _createRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        ProductImageCreateRequest createRequestInternal = _productMapper.Map(createRequest);

        createRequestInternal.DateModified = DateTime.Today;

        OneOf<uint, UnexpectedFailureResult> result = _productImageRepository.InsertInAllImages(createRequestInternal);

        return result.Match<OneOf<uint, ValidationResult, UnexpectedFailureResult>>(
            id => id, unexpectedFailure => unexpectedFailure);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertInFirstImages(ServiceProductFirstImageCreateRequest createRequest,
        IValidator<ServiceProductFirstImageCreateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(createRequest, validator, _firstImageCreateRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        ProductFirstImageCreateRequest createRequestInternal = _productMapper.Map(createRequest);

        createRequestInternal.DateModified = DateTime.Today;

        OneOf<Success, UnexpectedFailureResult> result = _productImageRepository.InsertInFirstImages(createRequestInternal);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, unexpectedFailure => unexpectedFailure);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateInAllImages(ServiceProductImageUpdateRequest updateRequest,
        IValidator<ServiceProductImageUpdateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(updateRequest, validator, _updateRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        ProductImageUpdateRequest updateRequestInternal = _productMapper.Map(updateRequest);

        updateRequestInternal.DateModified = DateTime.Today;

        OneOf<Success, UnexpectedFailureResult> result = _productImageRepository.UpdateInAllImages(updateRequestInternal);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, unexpectedFailure => unexpectedFailure);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateInFirstImages(ServiceProductFirstImageUpdateRequest updateRequest,
        IValidator<ServiceProductFirstImageUpdateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(updateRequest, validator, _firstImageUpdateRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        ProductFirstImageUpdateRequest updateRequestInternal = _productMapper.Map(updateRequest);

        updateRequestInternal.DateModified = DateTime.Today;

        OneOf<Success, UnexpectedFailureResult> result = _productImageRepository.UpdateInFirstImages(updateRequestInternal);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, unexpectedFailure => unexpectedFailure);
    }

    public bool DeleteInAllImagesById(uint id)
    {
        return _productImageRepository.DeleteInAllImagesById(id);
    }

    public bool DeleteAllImagesForProduct(uint productId)
    {
        return _productImageRepository.DeleteAllWithSameProductIdInAllImages(productId);
    }

    public bool DeleteInFirstImagesByProductId(uint productId)
    {
        return _productImageRepository.DeleteInFirstImagesByProductId(productId);
    }
}