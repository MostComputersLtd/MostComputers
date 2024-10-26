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
using MOSTComputers.Utils.OneOf;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;
using static MOSTComputers.Services.ProductRegister.StaticUtilities.ImageUtils;

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

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpsertFirstAndAllImagesForProduct(
        int productId,
        List<ImageAndImageFileNameUpsertRequest> imageAndFileNameUpsertRequests,
        List<ProductImage>? oldProductImages = null)
    {
        imageAndFileNameUpsertRequests = OrderImageAndImageFileNameUpsertRequests(imageAndFileNameUpsertRequests);

        ImageAndImageFileNameUpsertRequest? productFirstImageUpsertRequest = null;

        for (int i = 0; i < imageAndFileNameUpsertRequests.Count; i++)
        {
            ImageAndImageFileNameUpsertRequest imageAndFileNameInfoUpsertRequest = imageAndFileNameUpsertRequests[i];

            ProductImageUpsertRequest? image = imageAndFileNameInfoUpsertRequest.ProductImageUpsertRequest;

            if (image is null) continue;

            ProductImage? imageInOldProduct = oldProductImages?.Find(
                img => img.Id == image.OriginalImageId);

            if (imageInOldProduct is null)
            {
                ServiceProductImageCreateRequest productImageCreateRequest = new()
                {
                    ProductId = productId,
                    ImageData = imageAndFileNameInfoUpsertRequest.ImageData,
                    ImageContentType = imageAndFileNameInfoUpsertRequest.ImageContentType,
                    HtmlData = image.HtmlData,
                };

                OneOf<int, ValidationResult, UnexpectedFailureResult> imageInsertResult
                    = InsertInAllImages(productImageCreateRequest);

                int imageId = -1;

                bool isImageInsertSuccessful = imageInsertResult.Match(
                    id =>
                    {
                        imageId = id;

                        return true;
                    },
                    validationResult => false,
                    unexpectedFailureResult => false);

                if (!isImageInsertSuccessful)
                {
                    return imageInsertResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
                        id => new Success(),
                        validationResult => validationResult,
                        unexpectedFailureResult => unexpectedFailureResult);
                }

                image.OriginalImageId = imageId;

                if (productFirstImageUpsertRequest is null
                    && imageAndFileNameInfoUpsertRequest.ProductImageUpsertRequest is not null)
                {
                    productFirstImageUpsertRequest = imageAndFileNameInfoUpsertRequest;
                }

                continue;
            }

            if (productFirstImageUpsertRequest is null
                && imageAndFileNameInfoUpsertRequest.ProductImageUpsertRequest is not null)
            {
                productFirstImageUpsertRequest = imageAndFileNameInfoUpsertRequest;
            }

            if (CompareByteArrays(imageAndFileNameInfoUpsertRequest.ImageData, imageInOldProduct.ImageData)
                && imageAndFileNameInfoUpsertRequest.ImageContentType == imageInOldProduct.ImageContentType
                && image.HtmlData == imageInOldProduct.HtmlData)
            {
                oldProductImages?.Remove(imageInOldProduct);

                continue;
            }

            ServiceProductImageUpdateRequest productImageUpdateRequest = new()
            {
                Id = image.OriginalImageId!.Value,
                ImageData = imageAndFileNameInfoUpsertRequest.ImageData,
                ImageContentType = imageAndFileNameInfoUpsertRequest.ImageContentType,
                HtmlData = image.HtmlData,
            };

            OneOf<Success, ValidationResult, UnexpectedFailureResult> imageUpdateResult
                = UpdateInAllImages(productImageUpdateRequest);

            bool isImageUpdateSuccessful = imageUpdateResult.Match(
                success => true,
                validationResult => false,
                unexpectedFailureResult => false);

            if (!isImageUpdateSuccessful) return imageUpdateResult;

            oldProductImages?.Remove(imageInOldProduct);
        }

        ProductImage? oldProductFirstImage = GetFirstImageForProduct(productId);

        if (productFirstImageUpsertRequest is not null)
        {
            if (oldProductFirstImage is null)
            {
                OneOf<Success, ValidationResult, UnexpectedFailureResult> insertFirstImageResult = InsertInFirstImages(new()
                {
                    ProductId = productId,
                    ImageData = productFirstImageUpsertRequest.ImageData,
                    ImageContentType = productFirstImageUpsertRequest.ImageContentType,
                    HtmlData = productFirstImageUpsertRequest.ProductImageUpsertRequest!.HtmlData,
                });

                bool isFirstImageInsertSuccessful = insertFirstImageResult.Match(
                    success => true,
                    validationResult => false,
                    unexpectedFailureResult => false);

                if (!isFirstImageInsertSuccessful) return insertFirstImageResult;
            }
            else
            {
                OneOf<Success, ValidationResult, UnexpectedFailureResult> updateFirstImageResult = UpdateInFirstImages(new()
                {
                    ProductId = productId,
                    ImageData = productFirstImageUpsertRequest.ImageData,
                    ImageContentType = productFirstImageUpsertRequest.ImageContentType,
                    HtmlData = productFirstImageUpsertRequest.ProductImageUpsertRequest!.HtmlData,
                });

                bool isFirstImageInsertSuccessful = updateFirstImageResult.Match(
                    success => true,
                    validationResult => false,
                    unexpectedFailureResult => false);

                if (!isFirstImageInsertSuccessful) return updateFirstImageResult;
            }
        }
        else if (oldProductFirstImage is not null)
        {
            bool isFirstImageDeleted = DeleteInFirstImagesByProductId(productId);

            if (!isFirstImageDeleted) return new UnexpectedFailureResult();
        }

        if (oldProductImages is not null
            && oldProductImages.Count > 0)
        {
            foreach (ProductImage oldImageToBeRemoved in oldProductImages)
            {
                bool imageDeleteResult = DeleteInAllImagesById(oldImageToBeRemoved.Id);

                if (!imageDeleteResult) return new UnexpectedFailureResult();
            }
        }

        return new Success();
    }

    public OneOf<bool, ValidationResult, UnexpectedFailureResult> UpdateHtmlDataInAllImagesById(int imageId, string htmlData)
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

        OneOf<bool, UnexpectedFailureResult> updateHtmlDataInAllImagesResult = _productImageRepository.UpdateHtmlDataInAllImagesById(imageId, htmlData);

        return updateHtmlDataInAllImagesResult.Map<bool, ValidationResult, UnexpectedFailureResult>();
    }

    public OneOf<bool, ValidationResult, UnexpectedFailureResult> UpdateHtmlDataInFirstImagesByProductId(int productId, string htmlData)
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
            = _productImageRepository.UpdateHtmlDataInFirstImagesByProductId(productId, htmlData);

        return updateHtmlDataInFirstImagesResult.Map<bool, ValidationResult, UnexpectedFailureResult>();
    }

    public OneOf<bool, ValidationResult, UnexpectedFailureResult> UpdateHtmlDataInFirstAndAllImagesByProductId(int productId, string htmlData)
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
            = _productImageRepository.UpdateHtmlDataInFirstAndAllImagesByProductId(productId, htmlData);

        return updateHtmlDataInAllAndFirstImagesResult.Map<bool, ValidationResult, UnexpectedFailureResult>();
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