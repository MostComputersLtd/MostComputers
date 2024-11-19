using FluentValidation;
using FluentValidation.Results;
using OneOf;
using OneOf.Types;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Mapping;
using MOSTComputers.Utils.OneOf;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts.ExternalXmlImport;
using MOSTComputers.Services.ProductRegister.Models.ExternalXmlImport.ProductImage;
using MOSTComputers.Models.Product.Models.ExternalXmlImport;
using MOSTComputers.Services.ProductRegister.Services.Contracts.ExternalXmlImport;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;
using static MOSTComputers.Services.ProductRegister.StaticUtilities.ImageUtils;
using MOSTComputers.Services.DAL.Models.Requests.ExternalXmlImport.ProductImage;

namespace MOSTComputers.Services.ProductRegister.Services.ExternalXmlImport;

internal sealed class XmlImportProductImageService : IXmlImportProductImageService
{
    public XmlImportProductImageService(
        IXmlImportProductImageRepository productImageRepository,
        ProductMapper productMapper,
        IValidator<XmlImportServiceProductImageCreateRequest>? createRequestValidator = null,
        IValidator<XmlImportServiceProductImageUpdateRequest>? updateRequestValidator = null,
        IValidator<XmlImportServiceProductFirstImageCreateRequest>? firstImageCreateRequestValidator = null,
        IValidator<XmlImportServiceProductFirstImageUpdateRequest>? firstImageUpdateRequestValidator = null)
    {
        _productImageRepository = productImageRepository;
        _productMapper = productMapper;
        _createRequestValidator = createRequestValidator;
        _updateRequestValidator = updateRequestValidator;
        _firstImageCreateRequestValidator = firstImageCreateRequestValidator;
        _firstImageUpdateRequestValidator = firstImageUpdateRequestValidator;
    }

    private readonly IXmlImportProductImageRepository _productImageRepository;
    private readonly ProductMapper _productMapper;
    private readonly IValidator<XmlImportServiceProductImageCreateRequest>? _createRequestValidator;
    private readonly IValidator<XmlImportServiceProductImageUpdateRequest>? _updateRequestValidator;
    private readonly IValidator<XmlImportServiceProductFirstImageCreateRequest>? _firstImageCreateRequestValidator;
    private readonly IValidator<XmlImportServiceProductFirstImageUpdateRequest>? _firstImageUpdateRequestValidator;

    public IEnumerable<XmlImportProductImage> GetAllInProduct(int productId)
    {
        if (productId <= 0) return Enumerable.Empty<XmlImportProductImage>();

        return _productImageRepository.GetAllInProduct(productId);
    }

    public IEnumerable<XmlImportProductImage> GetAllFirstImagesForAllProducts()
    {
        return _productImageRepository.GetAllFirstImagesForAllProducts();
    }

    public IEnumerable<XmlImportProductImage> GetAllFirstImagesForSelectionOfProducts(List<int> productIds)
    {
        productIds = RemoveValuesSmallerThanOne(productIds);

        return _productImageRepository.GetFirstImagesForSelectionOfProducts(productIds);
    }

    public XmlImportProductImage? GetByIdInAllImages(int id)
    {
        if (id <= 0) return null;

        return _productImageRepository.GetByIdInAllImages(id);
    }

    public XmlImportProductImage? GetFirstImageForProduct(int productId)
    {
        if (productId <= 0) return null;

        return _productImageRepository.GetByProductIdInFirstImages(productId);
    }

    public OneOf<int, ValidationResult, UnexpectedFailureResult> UpsertInAllImages(XmlImportServiceProductImageCreateRequest createRequest,
        IValidator<XmlImportServiceProductImageCreateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(createRequest, validator, _createRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        XmlImportProductImageUpsertRequest createRequestInternal = _productMapper.Map(createRequest);

        createRequestInternal.DateModified = DateTime.Today;

        OneOf<int, UnexpectedFailureResult> result = _productImageRepository.UpsertInAllImages(createRequestInternal);

        return result.Match<OneOf<int, ValidationResult, UnexpectedFailureResult>>(
            id => id, unexpectedFailure => unexpectedFailure);
    }

    public OneOf<int, ValidationResult, UnexpectedFailureResult> InsertInAllImagesAndImageFileNameInfos(
        XmlImportServiceProductImageCreateRequest createRequest,
        int? displayOrder = null,
        IValidator<XmlImportServiceProductImageCreateRequest>? validator = null)
    {
        if (displayOrder <= 0)
        {
            ValidationResult displayOrderResult = new();

            displayOrderResult.Errors.Add(new(nameof(displayOrder), $"Argument {nameof(displayOrder)} must not be equal to 0"));

            return displayOrderResult;
        }

        ValidationResult validationResult = ValidateTwoValidatorsDefault(createRequest, validator, _createRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        XmlImportProductImageUpsertRequest createRequestInternal = _productMapper.Map(createRequest);

        createRequestInternal.DateModified = DateTime.Today;

        OneOf<int, UnexpectedFailureResult> result = _productImageRepository.InsertInAllImagesAndImageFileNameInfos(createRequestInternal, displayOrder);

        return result.Match<OneOf<int, ValidationResult, UnexpectedFailureResult>>(
            id => id,
            unexpectedFailureResult => unexpectedFailureResult);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertInFirstImages(XmlImportServiceProductFirstImageCreateRequest createRequest,
        IValidator<XmlImportServiceProductFirstImageCreateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(createRequest, validator, _firstImageCreateRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        XmlImportProductFirstImageCreateRequest createRequestInternal = _productMapper.Map(createRequest);

        createRequestInternal.DateModified = DateTime.Today;

        OneOf<Success, UnexpectedFailureResult> result = _productImageRepository.InsertInFirstImages(createRequestInternal);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, unexpectedFailure => unexpectedFailure);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateInAllImages(XmlImportServiceProductImageUpdateRequest updateRequest,
        IValidator<XmlImportServiceProductImageUpdateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(updateRequest, validator, _updateRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        XmlImportProductImageUpdateRequest updateRequestInternal = _productMapper.Map(updateRequest);

        updateRequestInternal.DateModified = DateTime.Today;

        OneOf<Success, UnexpectedFailureResult> result = _productImageRepository.UpdateInAllImages(updateRequestInternal);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, unexpectedFailure => unexpectedFailure);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateInFirstImages(XmlImportServiceProductFirstImageUpdateRequest updateRequest,
        IValidator<XmlImportServiceProductFirstImageUpdateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(updateRequest, validator, _firstImageUpdateRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        XmlImportProductFirstImageUpdateRequest updateRequestInternal = _productMapper.Map(updateRequest);

        updateRequestInternal.DateModified = DateTime.Today;

        OneOf<Success, UnexpectedFailureResult> result = _productImageRepository.UpdateInFirstImages(updateRequestInternal);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, unexpectedFailure => unexpectedFailure);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpsertFirstAndAllImagesForProduct(
        int productId,
        List<ImageAndImageFileNameUpsertRequest> imageAndFileNameUpsertRequests,
        List<XmlImportProductImage>? oldProductImages = null)
    {
        imageAndFileNameUpsertRequests = OrderImageAndImageFileNameUpsertRequests(imageAndFileNameUpsertRequests);

        ImageAndImageFileNameUpsertRequest? productFirstImageUpsertRequest = null;

        for (int i = 0; i < imageAndFileNameUpsertRequests.Count; i++)
        {
            ImageAndImageFileNameUpsertRequest imageAndFileNameInfoUpsertRequest = imageAndFileNameUpsertRequests[i];

            ProductImageUpsertRequest? image = imageAndFileNameInfoUpsertRequest.ProductImageUpsertRequest;

            if (image is null) continue;

            XmlImportProductImage? imageInOldProduct = oldProductImages?.Find(
                img => img.Id == image.OriginalImageId);

            if (imageInOldProduct is null)
            {
                XmlImportServiceProductImageCreateRequest productImageCreateRequest = new()
                {
                    Id = image.OriginalImageId,
                    ProductId = productId,
                    ImageData = imageAndFileNameInfoUpsertRequest.ImageData,
                    ImageContentType = imageAndFileNameInfoUpsertRequest.ImageContentType,
                    HtmlData = image.HtmlData,
                };

                OneOf<int, ValidationResult, UnexpectedFailureResult> imageInsertResult
                    = UpsertInAllImages(productImageCreateRequest);

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

            XmlImportServiceProductImageUpdateRequest productImageUpdateRequest = new()
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

        XmlImportProductImage? oldProductFirstImage = GetFirstImageForProduct(productId);

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
            foreach (XmlImportProductImage oldImageToBeRemoved in oldProductImages)
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