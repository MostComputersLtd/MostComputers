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
using MOSTComputers.Utils.OneOf;

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

    public IEnumerable<ProductImage> GetAllInProduct(int productId)
    {
        if (productId <= 0) return Enumerable.Empty<ProductImage>();

        return _productImageRepository.GetAllInProduct(productId);
    }

    public IEnumerable<ProductImage> GetAllFirstImagesForAllProducts()
    {
        return _productImageRepository.GetAllFirstImagesForAllProducts();
    }

    public IEnumerable<ProductImage> GetAllFirstImagesForSelectionOfProducts(List<int> productIds)
    {
        productIds = RemoveValuesSmallerThanOne(productIds);

        return _productImageRepository.GetFirstImagesForSelectionOfProducts(productIds);
    }

    public ProductImage? GetByIdInAllImages(int id)
    {
        if (id <= 0) return null;

        return _productImageRepository.GetByIdInAllImages(id);
    }

    public ProductImage? GetFirstImageForProduct(int productId)
    {
        if (productId <= 0) return null;

        return _productImageRepository.GetByProductIdInFirstImages(productId);
    }

    public OneOf<int, ValidationResult, UnexpectedFailureResult> InsertInAllImages(ServiceProductImageCreateRequest createRequest,
        IValidator<ServiceProductImageCreateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(createRequest, validator, _createRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        ProductImageCreateRequest createRequestInternal = _productMapper.Map(createRequest);

        createRequestInternal.DateModified = DateTime.Today;

        OneOf<int, UnexpectedFailureResult> result = _productImageRepository.InsertInAllImages(createRequestInternal);

        return result.Match<OneOf<int, ValidationResult, UnexpectedFailureResult>>(
            id => id, unexpectedFailure => unexpectedFailure);
    }

    public OneOf<int, ValidationResult, UnexpectedFailureResult> InsertInAllImagesAndImageFileNameInfos(ServiceProductImageCreateRequest createRequest,
        int? displayOrder = null,
        IValidator<ServiceProductImageCreateRequest>? validator = null)
    {
        if (displayOrder <= 0)
        {
            ValidationResult displayOrderResult = new();

            displayOrderResult.Errors.Add(new(nameof(displayOrder), $"Argument {nameof(displayOrder)} must not be equal to 0"));

            return displayOrderResult;
        }

        ValidationResult validationResult = ValidateTwoValidatorsDefault(createRequest, validator, _createRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        ProductImageCreateRequest createRequestInternal = _productMapper.Map(createRequest);

        createRequestInternal.DateModified = DateTime.Today;

        OneOf<int, UnexpectedFailureResult> result = _productImageRepository.InsertInAllImagesAndImageFileNameInfos(createRequestInternal, displayOrder);

        return result.Match<OneOf<int, ValidationResult, UnexpectedFailureResult>>(
            id => id,
            unexpectedFailureResult => unexpectedFailureResult);
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

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateHtmlDataInAllImagesById(int imageId, string htmlData)
    {
        ValidationResult validationResult = new();

        if (imageId <= 0)
        {
            validationResult.Errors.Add(new(nameof(imageId), "Invalid id"));
        }

        if (string.IsNullOrEmpty(htmlData))
        {
            validationResult.Errors.Add(new(nameof(htmlData), "Cannot be null, empty, or whitespace"));
        }

        if (!validationResult.IsValid) return validationResult;

        OneOf<Success, UnexpectedFailureResult> updateHtmlDataInAllImagesResult = _productImageRepository.UpdateHtmlDataInAllImagesById(imageId, htmlData);

        return updateHtmlDataInAllImagesResult.Map<Success, ValidationResult, UnexpectedFailureResult>();
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateHtmlDataInFirstImagesByProductId(int productId, string htmlData)
    {
        ValidationResult validationResult = new();

        if (productId <= 0)
        {
            validationResult.Errors.Add(new(nameof(productId), "Invalid id"));
        }

        if (string.IsNullOrEmpty(htmlData))
        {
            validationResult.Errors.Add(new(nameof(htmlData), "Cannot be null, empty, or whitespace"));
        }

        if (!validationResult.IsValid) return validationResult;

        OneOf<Success, UnexpectedFailureResult> updateHtmlDataInFirstImagesResult = _productImageRepository.UpdateHtmlDataInFirstImagesByProductId(productId, htmlData);

        return updateHtmlDataInFirstImagesResult.Map<Success, ValidationResult, UnexpectedFailureResult>();
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateHtmlDataInFirstAndAllImagesByProductId(int productId, string htmlData)
    {
        ValidationResult validationResult = new();

        if (productId <= 0)
        {
            validationResult.Errors.Add(new(nameof(productId), "Invalid id"));
        }

        if (string.IsNullOrEmpty(htmlData))
        {
            validationResult.Errors.Add(new(nameof(htmlData), "Cannot be null, empty, or whitespace"));
        }

        if (!validationResult.IsValid) return validationResult;

        OneOf<Success, UnexpectedFailureResult> updateHtmlDataInAllAndFirstImagesResult = _productImageRepository.UpdateHtmlDataInFirstAndAllImagesByProductId(productId, htmlData);

        return updateHtmlDataInAllAndFirstImagesResult.Map<Success, ValidationResult, UnexpectedFailureResult>();
    }

    public bool DeleteInAllImagesById(int id)
    {
        if (id <= 0) return false;

        return _productImageRepository.DeleteInAllImagesById(id);
    }

    public bool DeleteInAllImagesAndImageFilePathInfosById(int id)
    {
        if (id <= 0) return false;

        return _productImageRepository.DeleteInAllImagesAndImageFilePathInfosById(id);
    }

    public bool DeleteAllImagesForProduct(int productId)
    {
        if (productId <= 0) return false;

        return _productImageRepository.DeleteAllWithSameProductIdInAllImages(productId);
    }

    public bool DeleteInFirstImagesByProductId(int productId)
    {
        if (productId <= 0) return false;

        return _productImageRepository.DeleteInFirstImagesByProductId(productId);
    }
}