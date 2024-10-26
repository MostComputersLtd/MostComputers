using FluentValidation;
using OneOf.Types;
using OneOf;
using FluentValidation.Results;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Models.Product.Models.Requests.ProductImage;
using MOSTComputers.Services.Caching.Services.Contracts;
using MOSTComputers.Services.ProductRegister.StaticUtilities;
using static MOSTComputers.Services.ProductRegister.StaticUtilities.CacheKeyUtils.ProductImage;
using static MOSTComputers.Services.ProductRegister.StaticUtilities.ProductDataCloningUtils;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;

namespace MOSTComputers.Services.ProductRegister.Services.UsingCache;

internal sealed class CachedProductImageService : IProductImageService
{
    public CachedProductImageService(
        ProductImageService productImageService,
        IProductImageFileNameInfoService productImageFileNameInfoService,
        ICache<string> cache)
    {
        _productImageService = productImageService;
        _productImageFileNameInfoService = productImageFileNameInfoService;
        _cache = cache;
    }

    private readonly ProductImageService _productImageService;
    private readonly IProductImageFileNameInfoService _productImageFileNameInfoService;
    private readonly ICache<string> _cache;

    public IEnumerable<ProductImage> GetAllInProduct(int productId)
    {
        if (productId <= 0) return Enumerable.Empty<ProductImage>();

        IEnumerable<ProductImage> productImages = _cache.GetOrAdd(GetInAllImagesByProductIdKey(productId),
            () => _productImageService.GetAllInProduct(productId));

        return CloneAll(productImages);
    }

    public IEnumerable<ProductImage> GetAllFirstImagesForAllProducts()
    {
        return _productImageService.GetAllFirstImagesForAllProducts();
    }

    public IEnumerable<ProductImage> GetAllFirstImagesForSelectionOfProducts(List<int> productIds)
    {
        productIds = RemoveValuesSmallerThanOne(productIds);

        List<ProductImage>? cachedImages = null;

        for (int i = 0; i < productIds.Count; i++)
        {
            int productId = productIds[i];

            ProductImage? cachedproductFirstImage
                = _cache.GetValueOrDefault<ProductImage>(GetInFirstImagesByIdKey(productId));

            if (cachedproductFirstImage is not null)
            {
                cachedImages ??= new();

                ProductImage firstImageClone = Clone(cachedproductFirstImage);

                cachedImages.Add(firstImageClone);

                productIds.RemoveAt(i);

                i--;
            }
        }

        if (productIds.Count <= 0)
        {
            if (cachedImages is not null)
            {
                return cachedImages;
            }

            return Enumerable.Empty<ProductImage>();
        }

        IEnumerable<ProductImage> firstImages = _productImageService.GetAllFirstImagesForSelectionOfProducts(productIds);

        foreach (ProductImage image in firstImages)
        {
            ProductImage imageCloneForCache = Clone(image);
            _cache.Add(GetInFirstImagesByIdKey(image.Id), imageCloneForCache);
        }

        if (cachedImages is not null)
        {
            cachedImages.AddRange(firstImages);

            return cachedImages;
        }

        return firstImages;
    }

    public ProductImage? GetByIdInAllImages(int id)
    {
        if (id <= 0) return null;

        ProductImage? productImageWithId = _cache.GetOrAdd(GetInAllImagesByIdKey(id),
            () => _productImageService.GetByIdInAllImages(id));

        if (productImageWithId is null) return null;

        return Clone(productImageWithId);
    }

    public ProductImage? GetFirstImageForProduct(int productId)
    {
        if (productId <= 0) return null;

        ProductImage? productFirstImage = _cache.GetOrAdd(GetInFirstImagesByIdKey(productId),
            () => _productImageService.GetFirstImageForProduct(productId));

        if (productFirstImage is null) return null;

        return Clone(productFirstImage);
    }

    public OneOf<int, ValidationResult, UnexpectedFailureResult> InsertInAllImages(ServiceProductImageCreateRequest createRequest,
        IValidator<ServiceProductImageCreateRequest>? validator = null)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> result = _productImageService.InsertInAllImages(createRequest, validator);

        int? imageIdFromResult = result.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        if (imageIdFromResult is not null)
        {
            _cache.Evict(GetInAllImagesByIdKey(imageIdFromResult.Value));

            _cache.Evict(GetInAllImagesByProductIdKey(createRequest.ProductId));

            _cache.Evict(CacheKeyUtils.ForProduct.GetByIdKey(createRequest.ProductId));
        }

        return result;
    }

    public OneOf<int, ValidationResult, UnexpectedFailureResult> InsertInAllImagesAndImageFileNameInfos(
        ServiceProductImageCreateRequest createRequest,
        int? displayOrder = null,
        IValidator<ServiceProductImageCreateRequest>? validator = null)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> result = _productImageService.InsertInAllImagesAndImageFileNameInfos(createRequest, displayOrder, validator);

        int? imageIdFromResult = result.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        if (imageIdFromResult is not null)
        {
            _cache.Evict(GetInAllImagesByIdKey(imageIdFromResult.Value));

            int productId = createRequest.ProductId;

            int? highestImageNumber = _productImageFileNameInfoService.GetHighestImageNumber(productId);

            if (highestImageNumber is not null)
            {
                EvictAllIndividualCachedImageFileNames(productId, highestImageNumber);
            }

            _cache.Evict(GetInAllImagesByProductIdKey(productId));

            _cache.Evict(CacheKeyUtils.ProductImageFileNameInfo.GetByProductIdKey(productId));



            _cache.Evict(CacheKeyUtils.ForProduct.GetByIdKey(productId));
        }

        return result;
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertInFirstImages(ServiceProductFirstImageCreateRequest createRequest,
        IValidator<ServiceProductFirstImageCreateRequest>? validator = null)
    {
        OneOf<Success, ValidationResult, UnexpectedFailureResult> result = _productImageService.InsertInFirstImages(createRequest, validator);

        bool successFromResult = result.Match(
            success => true,
            _ => false,
            _ => false);

        if (successFromResult)
        {
            _cache.Evict(GetInFirstImagesByIdKey(createRequest.ProductId));

            _cache.Evict(CacheKeyUtils.ForProduct.GetByIdKey(createRequest.ProductId));
        }

        return result;
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateInAllImages(ServiceProductImageUpdateRequest updateRequest,
        IValidator<ServiceProductImageUpdateRequest>? validator = null)
    {
        OneOf<Success, ValidationResult, UnexpectedFailureResult> result = _productImageService.UpdateInAllImages(updateRequest);

        bool successFromResult = result.Match(
           success => true,
           _ => false,
           _ => false);

        if (successFromResult)
        {
            ProductImage? updatedOrOldProductImage = GetByIdInAllImages(updateRequest.Id);

            _cache.Evict(GetInAllImagesByIdKey(updateRequest.Id));

            if (updatedOrOldProductImage is null
                || updatedOrOldProductImage.ProductId is null) return result;

            _cache.Evict(GetInAllImagesByProductIdKey(updatedOrOldProductImage.ProductId.Value));

            _cache.Evict(CacheKeyUtils.ForProduct.GetByIdKey(updatedOrOldProductImage.ProductId.Value));
        }

        return result;
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateInFirstImages(ServiceProductFirstImageUpdateRequest updateRequest,
        IValidator<ServiceProductFirstImageUpdateRequest>? validator = null)
    {
        OneOf<Success, ValidationResult, UnexpectedFailureResult> result = _productImageService.UpdateInFirstImages(updateRequest);

        bool successFromResult = result.Match(
            success => true,
            validationResult => false,
            unexpectedFailureResult => false);

        if (successFromResult)
        {
            _cache.Evict(GetInFirstImagesByIdKey(updateRequest.ProductId));

            _cache.Evict(CacheKeyUtils.ForProduct.GetByIdKey(updateRequest.ProductId));
        }

        return result;
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpsertFirstAndAllImagesForProduct(
        int productId, List<ImageAndImageFileNameUpsertRequest> imageAndFileNameUpsertRequests, List<ProductImage>? oldProductImages = null)
    {
        IEnumerable<ProductImage> existingImages = GetAllInProduct(productId);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> upsertImagesInProductResult
            = _productImageService.UpsertFirstAndAllImagesForProduct(productId, imageAndFileNameUpsertRequests, oldProductImages);

        bool successFromResult = upsertImagesInProductResult.Match(
            success => true,
            validationResult => false,
            unexpectedFailureResult => false);

        if (successFromResult)
        {
            _cache.Evict(GetInFirstImagesByIdKey(productId));
            _cache.Evict(GetInAllImagesByProductIdKey(productId));

            foreach (ProductImage existingImage in existingImages)
            {
                _cache.Evict(GetInAllImagesByIdKey(productId));
            }

            _cache.Evict(CacheKeyUtils.ForProduct.GetByIdKey(productId));
        }

        return upsertImagesInProductResult;
    }

    public OneOf<bool, ValidationResult, UnexpectedFailureResult> UpdateHtmlDataInAllImagesById(int imageId, string htmlData)
    {
        OneOf<bool, ValidationResult, UnexpectedFailureResult> updateHtmlDataInAllImagesResult
            = _productImageService.UpdateHtmlDataInAllImagesById(imageId, htmlData);

        bool successFromResult = updateHtmlDataInAllImagesResult.Match(
            isSuccessful => isSuccessful,
            _ => false,
            _ => false);

        if (successFromResult)
        {
            _cache.Evict(GetInAllImagesByIdKey(imageId));

            ProductImage? productImage = _productImageService.GetByIdInAllImages(imageId);

            if (productImage?.ProductId is null) return updateHtmlDataInAllImagesResult;

            _cache.Evict(GetInAllImagesByProductIdKey(productImage.ProductId.Value));

            _cache.Evict(CacheKeyUtils.ForProduct.GetByIdKey(productImage.ProductId.Value));
        }

        return updateHtmlDataInAllImagesResult;
    }

    public OneOf<bool, ValidationResult, UnexpectedFailureResult> UpdateHtmlDataInFirstImagesByProductId(int productId, string htmlData)
    {
        OneOf<bool, ValidationResult, UnexpectedFailureResult> updateHtmlDataInFirstImagesResult
            = _productImageService.UpdateHtmlDataInFirstImagesByProductId(productId, htmlData);

        bool successFromResult = updateHtmlDataInFirstImagesResult.Match(
            isSuccessful => isSuccessful,
            _ => false,
            _ => false);

        if (successFromResult)
        {
            _cache.Evict(GetInFirstImagesByIdKey(productId));

            _cache.Evict(CacheKeyUtils.ForProduct.GetByIdKey(productId));
        }

        return updateHtmlDataInFirstImagesResult;
    }

    public OneOf<bool, ValidationResult, UnexpectedFailureResult> UpdateHtmlDataInFirstAndAllImagesByProductId(int productId, string htmlData)
    {
        OneOf<bool, ValidationResult, UnexpectedFailureResult> updateHtmlDataInFirstAndAllImagesResult
            = _productImageService.UpdateHtmlDataInFirstAndAllImagesByProductId(productId, htmlData);

        bool successFromResult = updateHtmlDataInFirstAndAllImagesResult.Match(
            isSuccessful => isSuccessful,
            _ => false,
            _ => false);

        if (successFromResult)
        {
            string getInAllImagesByProductIdKey = GetInAllImagesByProductIdKey(productId);

            IEnumerable<ProductImage>? cachedProductImages = _cache.GetValueOrDefault<IEnumerable<ProductImage>>(getInAllImagesByProductIdKey);

            if (cachedProductImages is not null)
            {
                foreach (ProductImage productImage in cachedProductImages)
                {
                    _cache.Evict(GetInAllImagesByIdKey(productImage.Id));
                }
            }

            _cache.Evict(getInAllImagesByProductIdKey);

            _cache.Evict(GetInFirstImagesByIdKey(productId));

            _cache.Evict(CacheKeyUtils.ForProduct.GetByIdKey(productId));
        }

        return updateHtmlDataInFirstAndAllImagesResult;
    }

    public bool DeleteInAllImagesById(int id)
    {
        if (id <= 0) return false;

        ProductImage? productImage = GetByIdInAllImages(id);

        if (productImage is null) return false;

        bool success = _productImageService.DeleteInAllImagesById(id);

        if (success)
        {
            _cache.Evict(GetInAllImagesByIdKey(id));

            if (productImage.ProductId is null) return success;

            _cache.Evict(GetInAllImagesByProductIdKey(productImage.ProductId.Value));

            _cache.Evict(CacheKeyUtils.ForProduct.GetByIdKey(productImage.ProductId.Value));
        }

        return success;
    }

    public bool DeleteInAllImagesAndImageFilePathInfosById(int id)
    {
        if (id <= 0) return false;

        ProductImage? productImage = GetByIdInAllImages(id);

        if (productImage is null) return false;

        bool success = _productImageService.DeleteInAllImagesAndImageFilePathInfosById(id);

        if (success)
        {
            _cache.Evict(GetInAllImagesByIdKey(id));

            if (productImage.ProductId is null) return success;

            int productId = productImage.ProductId.Value;

            int? highestImageNumber = _productImageFileNameInfoService.GetHighestImageNumber(productId);

            if (highestImageNumber is not null)
            {
                EvictAllIndividualCachedImageFileNames(productId, highestImageNumber);
            }

            _cache.Evict(GetInAllImagesByProductIdKey(productId));

            _cache.Evict(CacheKeyUtils.ProductImageFileNameInfo.GetByProductIdKey(productId));

            _cache.Evict(CacheKeyUtils.ForProduct.GetByIdKey(productId));
        }

        return success;
    }

    public bool DeleteAllImagesForProduct(int productId)
    {
        if (productId <= 0) return false;

        IEnumerable<ProductImage> imagesForProduct = GetAllInProduct(productId);

        if (!imagesForProduct.Any()) return false;

        bool success = _productImageService.DeleteAllImagesForProduct(productId);

        if (success)
        {
            foreach (ProductImage image in imagesForProduct)
            {
                _cache.Evict(GetInAllImagesByIdKey(image.Id));
            }

            _cache.Evict(GetInAllImagesByProductIdKey(productId));

            _cache.Evict(CacheKeyUtils.ForProduct.GetByIdKey(productId));
        }

        return success;
    }

    public bool DeleteInFirstImagesByProductId(int productId)
    {
        if (productId <= 0) return false;

        bool success = _productImageService.DeleteInFirstImagesByProductId(productId);

        if (success)
        {
            _cache.Evict(GetInFirstImagesByIdKey(productId));

            _cache.Evict(CacheKeyUtils.ForProduct.GetByIdKey(productId));
        }

        return success;
    }
    private void EvictAllIndividualCachedImageFileNames(int productId, int? highestImageNumber)
    {
        if (highestImageNumber is not null)
        {
            for (int i = 1; i <= highestImageNumber.Value; i++)
            {
                _cache.Evict(CacheKeyUtils.ProductImageFileNameInfo.GetByProductIdAndImageNumberKey(productId, i));
            }
        }
    }
}