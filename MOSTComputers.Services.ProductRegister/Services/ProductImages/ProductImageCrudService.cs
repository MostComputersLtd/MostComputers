using FluentValidation;
using FluentValidation.Results;
using OneOf;
using OneOf.Types;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductImage;
using MOSTComputers.Services.ProductRegister.Mapping;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FirstImage;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using MOSTComputers.Utils.OneOf;

using static MOSTComputers.Services.ProductRegister.Utils.ValidationUtils;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.ProductImages;
using MOSTComputers.Services.DataAccess.Products.DataAccess.ProductImages.Contracts;

namespace MOSTComputers.Services.ProductRegister.Services.ProductImages;
internal class ProductImageCrudService : IProductImageCrudService
{
    public ProductImageCrudService(
        IProductImageRepository productImageRepository,
        ProductMapper productMapper,
        IValidator<ServiceProductImageCreateRequest>? createRequestValidator,
        IValidator<ServiceProductImageUpdateRequest>? updateRequestValidator,
        IValidator<ProductImageUpsertRequest>? upsertRequestValidator,
        IValidator<ServiceProductFirstImageCreateRequest>? firstImageCreateRequestValidator,
        IValidator<ServiceProductFirstImageUpdateRequest>? firstImageUpdateRequestValidator,
        IValidator<ServiceProductFirstImageUpsertRequest>? firstImageUpsertRequestValidator)
    {
        _productImageRepository = productImageRepository;
        _productMapper = productMapper;

        _createRequestValidator = createRequestValidator;
        _updateRequestValidator = updateRequestValidator;
        _upsertRequestValidator = upsertRequestValidator;
        _firstImageCreateRequestValidator = firstImageCreateRequestValidator;
        _firstImageUpdateRequestValidator = firstImageUpdateRequestValidator;
        _firstImageUpsertRequestValidator = firstImageUpsertRequestValidator;
    }

    private readonly IValidator<ServiceProductImageCreateRequest>? _createRequestValidator;
    private readonly IValidator<ServiceProductImageUpdateRequest>? _updateRequestValidator;
    private readonly IValidator<ProductImageUpsertRequest>? _upsertRequestValidator;

    private readonly IValidator<ServiceProductFirstImageCreateRequest>? _firstImageCreateRequestValidator;
    private readonly IValidator<ServiceProductFirstImageUpdateRequest>? _firstImageUpdateRequestValidator;
    private readonly IValidator<ServiceProductFirstImageUpsertRequest>? _firstImageUpsertRequestValidator;

    private readonly IProductImageRepository _productImageRepository;
    private readonly ProductMapper _productMapper;

    public int GetMinimumImagesAllInsertIdForLocalApplication()
    {
        return _productImageRepository.GetMinimumImagesAllInsertIdForLocalApplication();
    }

    public async Task<List<IGrouping<int, ProductImageData>>> GetAllWithoutFileDataAsync()
    {
        return await _productImageRepository.GetAllWithoutFileDataAsync();
    }

    public async Task<List<IGrouping<int, ProductImage>>> GetAllInProductsAsync(IEnumerable<int> productIds)
    {
        return await _productImageRepository.GetAllInProductsAsync(productIds);
    }

    public async Task<List<ProductImage>> GetAllInProductAsync(int productId)
    {
        return await _productImageRepository.GetAllInProductAsync(productId);
    }

    public async Task<ProductImage?> GetByIdInAllImagesAsync(int id)
    {
        return await _productImageRepository.GetByIdInAllImagesAsync(id);
    }

    public async Task<List<ProductImage>> GetAllFirstImagesForAllProductsAsync()
    {
        return await _productImageRepository.GetAllFirstImagesForAllProductsAsync();
    }

    public async Task<List<ProductImageData>> GetAllFirstImagesWithoutFileDataForAllProductsAsync()
    {
        return await _productImageRepository.GetAllFirstImagesWithoutFileDataForAllProductsAsync();
    }

    public async Task<List<ProductImage>> GetFirstImagesForSelectionOfProductsAsync(IEnumerable<int> productIds)
    {
        return await _productImageRepository.GetFirstImagesForSelectionOfProductsAsync(productIds);
    }

    public async Task<List<ProductImageData>> GetFirstImagesWithoutFileDataForSelectionOfProductsAsync(IEnumerable<int> productIds)
    {
        return await _productImageRepository.GetFirstImagesWithoutFileDataForSelectionOfProductsAsync(productIds);
    }

    public async Task<ProductImage?> GetByProductIdInFirstImagesAsync(int productId)
    {
        return await _productImageRepository.GetByProductIdInFirstImagesAsync(productId);
    }

    public async Task<List<ProductImagesForProductCountData>> GetCountOfAllInProductsAsync(IEnumerable<int> productIds)
    {
        return await _productImageRepository.GetCountOfAllInProductsAsync(productIds);
    }

    public async Task<int> GetCountOfAllInProductAsync(int productId)
    {
        return await _productImageRepository.GetCountOfAllInProductAsync(productId);
    }

    public async Task<bool> DoesProductImageExistAsync(int imageId)
    {
        return await _productImageRepository.DoesProductImageExistAsync(imageId);
    }

    public async Task<List<ProductFirstImageExistsForProductData>> DoProductsHaveImagesInFirstImagesAsync(IEnumerable<int> productIds)
    {
        return await _productImageRepository.DoProductsHaveImagesInFirstImagesAsync(productIds);
    }

    public async Task<bool> DoesProductHaveImageInFirstImagesAsync(int productId)
    {
        return await _productImageRepository.DoesProductHaveImageInFirstImagesAsync(productId);
    }

    public async Task<ProductImageData?> GetByIdInAllImagesWithoutFileDataAsync(int id)
    {
        return await _productImageRepository.GetByIdInAllImagesWithoutFileDataAsync(id);
    }

    public async Task<List<ProductImageData>> GetAllInProductWithoutFileDataAsync(int productId)
    {
        return await _productImageRepository.GetAllInProductWithoutFileDataAsync(productId);
    }

    public async Task<List<IGrouping<int, ProductImageData>>> GetAllInProductsWithoutFileDataAsync(IEnumerable<int> productIds)
    {
        return await _productImageRepository.GetAllInProductsWithoutFileDataAsync(productIds);
    }

    public async Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> InsertInAllImagesAsync(ServiceProductImageCreateRequest createRequest)
    {
        ValidationResult validationResult = ValidateDefault(_createRequestValidator, createRequest);

        if (!validationResult.IsValid) return validationResult;

        ProductImageCreateRequest createRequestInternal = _productMapper.Map(createRequest);

        createRequestInternal.DateModified = DateTime.Today;

        OneOf<int, UnexpectedFailureResult> result = await _productImageRepository.InsertInAllImagesAsync(createRequestInternal);

        return result.Match<OneOf<int, ValidationResult, UnexpectedFailureResult>>(
            id => id, unexpectedFailure => unexpectedFailure);
    }

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> InsertInFirstImagesAsync(ServiceProductFirstImageCreateRequest createRequest)
    {
        ValidationResult validationResult = ValidateDefault(_firstImageCreateRequestValidator, createRequest);

        if (!validationResult.IsValid) return validationResult;

        ProductFirstImageCreateRequest createRequestInternal = _productMapper.Map(createRequest);

        createRequestInternal.DateModified = DateTime.Today;

        OneOf<Success, UnexpectedFailureResult> result = await _productImageRepository.InsertInFirstImagesAsync(createRequestInternal);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, unexpectedFailure => unexpectedFailure);
    }

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpdateInAllImagesAsync(ServiceProductImageUpdateRequest updateRequest)
    {
        ValidationResult validationResult = ValidateDefault(_updateRequestValidator, updateRequest);

        if (!validationResult.IsValid) return validationResult;

        ProductImageUpdateRequest updateRequestInternal = _productMapper.Map(updateRequest);

        updateRequestInternal.DateModified = DateTime.Today;

        OneOf<Success, UnexpectedFailureResult> result = await _productImageRepository.UpdateInAllImagesAsync(updateRequestInternal);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, unexpectedFailure => unexpectedFailure);
    }

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpdateInFirstImagesAsync(ServiceProductFirstImageUpdateRequest updateRequest)
    {
        ValidationResult validationResult = ValidateDefault(_firstImageUpdateRequestValidator, updateRequest);

        if (!validationResult.IsValid) return validationResult;

        ProductFirstImageUpdateRequest updateRequestInternal = _productMapper.Map(updateRequest);

        updateRequestInternal.DateModified = DateTime.Today;

        OneOf<Success, UnexpectedFailureResult> result = await _productImageRepository.UpdateInFirstImagesAsync(updateRequestInternal);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, unexpectedFailure => unexpectedFailure);
    }

    public async Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> UpsertInAllImagesAsync(ProductImageUpsertRequest productImageUpsertRequest)
    {
        ValidationResult validationResult = ValidateDefault(_upsertRequestValidator, productImageUpsertRequest);

        if (!validationResult.IsValid) return validationResult;

        if (productImageUpsertRequest.ExistingImageId is null)
        {
            ServiceProductImageCreateRequest productImageCreateRequest = new()
            {
                ProductId = productImageUpsertRequest.ProductId,
                ImageContentType = productImageUpsertRequest.ImageContentType,
                ImageData = productImageUpsertRequest.ImageData,
                HtmlData = productImageUpsertRequest.HtmlData,
            };

            return await InsertInAllImagesAsync(productImageCreateRequest);
        }

        ServiceProductImageUpdateRequest productImageUpdateRequest = new()
        {
            Id = productImageUpsertRequest.ExistingImageId.Value,
            ImageContentType = productImageUpsertRequest.ImageContentType,
            ImageData = productImageUpsertRequest.ImageData,
            HtmlData = productImageUpsertRequest.HtmlData,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> updateImageResult = await UpdateInAllImagesAsync(productImageUpdateRequest);

        return updateImageResult.Match<OneOf<int, ValidationResult, UnexpectedFailureResult>>(
            success => productImageUpsertRequest.ExistingImageId.Value,
            validationResult => validationResult,
            unexpectedFailureResult => unexpectedFailureResult);
    }

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertInFirstImagesAsync(ServiceProductFirstImageUpsertRequest upsertRequest)
    {
        ValidationResult validationResult = ValidateDefault(_firstImageUpsertRequestValidator, upsertRequest);

        if (!validationResult.IsValid) return validationResult;

        ProductImage? existingFirstImage = await GetByProductIdInFirstImagesAsync(upsertRequest.ProductId);

        if (existingFirstImage is null)
        {
            ServiceProductFirstImageCreateRequest productFirstImageCreateRequest = new()
            {
                ProductId = upsertRequest.ProductId,
                ImageContentType = upsertRequest.ImageContentType,
                ImageData = upsertRequest.ImageData,
                HtmlData = upsertRequest.HtmlData,
            };

            return await InsertInFirstImagesAsync(productFirstImageCreateRequest);
        }

        ServiceProductFirstImageUpdateRequest productFirstImageUpdateRequest = new()
        {
            ProductId = upsertRequest.ProductId,
            ImageContentType = upsertRequest.ImageContentType,
            ImageData = upsertRequest.ImageData,
            HtmlData = upsertRequest.HtmlData,
        };

        return await UpdateInFirstImagesAsync(productFirstImageUpdateRequest);
    }

    public async Task<OneOf<bool, ValidationResult, UnexpectedFailureResult>> UpdateHtmlDataInAllImagesByIdAsync(int imageId, string htmlData)
    {
        ValidationResult validationResult = new();

        if (imageId <= 0)
        {
            validationResult.Errors.Add(new(nameof(imageId), "Invalid id"));
        }

        if (string.IsNullOrWhiteSpace(htmlData))
        {
            validationResult.Errors.Add(new(nameof(htmlData), "Cannot be null, empty, or whitespace"));
        }

        if (!validationResult.IsValid) return validationResult;

        OneOf<bool, UnexpectedFailureResult> updateHtmlDataInAllImagesResult = await _productImageRepository.UpdateHtmlDataInAllImagesByIdAsync(imageId, htmlData);

        return updateHtmlDataInAllImagesResult.Map<bool, ValidationResult, UnexpectedFailureResult>();
    }

    public async Task<OneOf<bool, ValidationResult, UnexpectedFailureResult>> UpdateHtmlDataInFirstImagesByProductIdAsync(int productId, string htmlData)
    {
        ValidationResult validationResult = new();

        if (productId <= 0)
        {
            validationResult.Errors.Add(new(nameof(productId), "Invalid id"));
        }

        if (string.IsNullOrWhiteSpace(htmlData))
        {
            validationResult.Errors.Add(new(nameof(htmlData), "Cannot be null, empty, or whitespace"));
        }

        if (!validationResult.IsValid) return validationResult;

        OneOf<bool, UnexpectedFailureResult> updateHtmlDataInFirstImagesResult
            = await _productImageRepository.UpdateHtmlDataInFirstImagesByProductIdAsync(productId, htmlData);

        return updateHtmlDataInFirstImagesResult.Map<bool, ValidationResult, UnexpectedFailureResult>();
    }

    public async Task<OneOf<bool, ValidationResult, UnexpectedFailureResult>> UpdateHtmlDataInFirstAndAllImagesByProductIdAsync(int productId, string htmlData)
    {
        ValidationResult validationResult = new();

        if (productId <= 0)
        {
            validationResult.Errors.Add(new(nameof(productId), "Invalid id"));
        }

        if (string.IsNullOrWhiteSpace(htmlData))
        {
            validationResult.Errors.Add(new(nameof(htmlData), "Cannot be null, empty, or whitespace"));
        }

        if (!validationResult.IsValid) return validationResult;

        OneOf<bool, UnexpectedFailureResult> updateHtmlDataInAllAndFirstImagesResult
            = await _productImageRepository.UpdateHtmlDataInFirstAndAllImagesByProductIdAsync(productId, htmlData);

        return updateHtmlDataInAllAndFirstImagesResult.Map<bool, ValidationResult, UnexpectedFailureResult>();
    }

    public async Task<bool> DeleteAllInAllImagesByProductIdAsync(int productId)
    {
        if (productId <= 0) return false;

        return await _productImageRepository.DeleteAllInAllImagesByProductIdAsync(productId);
    }

    public async Task<bool> DeleteInAllImagesByIdAsync(int id)
    {
        if (id <= 0) return false;

        return await _productImageRepository.DeleteInAllImagesByIdAsync(id);
    }

    public async Task<bool> DeleteInFirstImagesByProductIdAsync(int productId)
    {
        if (productId <= 0) return false;

        return await _productImageRepository.DeleteInFirstImagesByProductIdAsync(productId);
    }
}