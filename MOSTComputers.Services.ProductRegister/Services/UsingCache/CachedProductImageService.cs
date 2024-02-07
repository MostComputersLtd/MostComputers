using FluentValidation;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using OneOf.Types;
using OneOf;
using FluentValidation.Results;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Models.Product.Models.Requests.ProductImage;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;
using MOSTComputers.Services.Caching.Services.Contracts;
using MOSTComputers.Services.ProductRegister.StaticUtilities;
using static MOSTComputers.Services.ProductRegister.StaticUtilities.CacheKeyUtils.ProductImage;
namespace MOSTComputers.Services.ProductRegister.Services;

internal sealed class CachedProductImageService : IProductImageService
{
    public CachedProductImageService(
        ProductImageService productImageService,
        ICache<string> cache)
    {
        _productImageService = productImageService;
        _cache = cache;
    }

    private readonly ProductImageService _productImageService;
    private readonly ICache<string> _cache;

    public IEnumerable<ProductImage> GetAllInProduct(uint productId)
    {
        return _cache.GetOrAdd(GetInAllImagesByProductIdKey((int)productId),
            () => _productImageService.GetAllInProduct(productId));
    }

    public IEnumerable<ProductImage> GetAllFirstImagesForAllProducts()
    {
        return _productImageService.GetAllFirstImagesForAllProducts();
    }

    public IEnumerable<ProductImage> GetAllFirstImagesForSelectionOfProducts(List<uint> productIds)
    {
        List<ProductImage>? cachedImages = null;

        for (int i = 0; i < productIds.Count; i++)
        {
            uint productId = productIds[i];

            ProductImage? cachedproductFirstImage
                = _cache.GetValueOrDefault<ProductImage>(GetInFirstImagesByIdKey((int)productId));

            if (cachedproductFirstImage is not null)
            {
                cachedImages ??= new();

                cachedImages.Add(cachedproductFirstImage);

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
            _cache.Add(GetInFirstImagesByIdKey(image.Id), image);
        }

        if (cachedImages is not null)
        {
            cachedImages.AddRange(firstImages);

            return cachedImages;
        }

        return firstImages;
    }

    public ProductImage? GetByIdInAllImages(uint id)
    {
        return _cache.GetOrAdd(GetInAllImagesByIdKey((int)id),
            () => _productImageService.GetByIdInAllImages(id));
    }

    public ProductImage? GetFirstImageForProduct(uint productId)
    {
        return _cache.GetOrAdd(GetInFirstImagesByIdKey((int)productId),
            () => _productImageService.GetFirstImageForProduct(productId));
    }

    public OneOf<uint, ValidationResult, UnexpectedFailureResult> InsertInAllImages(ServiceProductImageCreateRequest createRequest,
        IValidator<ServiceProductImageCreateRequest>? validator = null)
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> result = _productImageService.InsertInAllImages(createRequest, validator);

        uint? imageIdFromResult = result.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        if (imageIdFromResult is not null)
        {
            _cache.Evict(GetInAllImagesByIdKey((int)imageIdFromResult));

            if (createRequest.ProductId is null) return result;

            _cache.Evict(GetInAllImagesByProductIdKey(createRequest.ProductId.Value));

            _cache.Evict(CacheKeyUtils.Product.GetByIdKey(createRequest.ProductId.Value));
        }

        return result;
    }

    public OneOf<uint, ValidationResult, UnexpectedFailureResult> InsertInAllImagesAndImageFileNameInfos(ServiceProductImageCreateRequest createRequest,
        uint? displayOrder = null,
        IValidator<ServiceProductImageCreateRequest>? validator = null)
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> result = _productImageService.InsertInAllImagesAndImageFileNameInfos(createRequest, displayOrder, validator);

        uint? imageIdFromResult = result.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        if (imageIdFromResult is not null)
        {
            _cache.Evict(GetInAllImagesByIdKey((int)imageIdFromResult));

            if (createRequest.ProductId is null) return result;

            _cache.Evict(GetInAllImagesByProductIdKey(createRequest.ProductId.Value));

            _cache.Evict(CacheKeyUtils.ProductImageFileNameInfo.GetByProductIdKey(createRequest.ProductId.Value));

            _cache.Evict(CacheKeyUtils.Product.GetByIdKey(createRequest.ProductId.Value));
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
            if (createRequest.ProductId is null) return result;

            _cache.Evict(GetInFirstImagesByIdKey(createRequest.ProductId.Value));

            _cache.Evict(CacheKeyUtils.Product.GetByIdKey(createRequest.ProductId.Value));
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
            ProductImage? updatedOrOldProductImage = GetByIdInAllImages((uint)updateRequest.Id);

            _cache.Evict(GetInAllImagesByIdKey(updateRequest.Id));

            if (updatedOrOldProductImage is null
                || updatedOrOldProductImage.ProductId is null) return result;

            _cache.Evict(GetInAllImagesByProductIdKey(updatedOrOldProductImage.ProductId.Value));

            _cache.Evict(CacheKeyUtils.Product.GetByIdKey(updatedOrOldProductImage.ProductId.Value));
        }

        return result;
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateInFirstImages(ServiceProductFirstImageUpdateRequest updateRequest,
        IValidator<ServiceProductFirstImageUpdateRequest>? validator = null)
    {
        OneOf<Success, ValidationResult, UnexpectedFailureResult> result = _productImageService.UpdateInFirstImages(updateRequest);

        bool successFromResult = result.Match(
            success => true,
            _ => false,
            _ => false);

        if (successFromResult)
        {
            _cache.Evict(GetInFirstImagesByIdKey(updateRequest.ProductId));

            _cache.Evict(CacheKeyUtils.Product.GetByIdKey(updateRequest.ProductId));
        }

        return result;
    }

    public bool DeleteInAllImagesById(uint id)
    {
        ProductImage? productImage = GetByIdInAllImages(id);

        if (productImage is null) return false;

        bool success = _productImageService.DeleteInAllImagesById(id);

        if (success)
        {
            _cache.Evict(GetInAllImagesByIdKey((int)id));

            if (productImage.ProductId is null) return success;

            _cache.Evict(GetInAllImagesByProductIdKey(productImage.ProductId.Value));

            _cache.Evict(CacheKeyUtils.Product.GetByIdKey(productImage.ProductId.Value));
        }

        return success;
    }

    public bool DeleteInAllImagesAndImageFilePathInfosById(uint id)
    {
        ProductImage? productImage = GetByIdInAllImages(id);

        if (productImage is null) return false;

        bool success = _productImageService.DeleteInAllImagesAndImageFilePathInfosById(id);

        if (success)
        {
            _cache.Evict(GetInAllImagesByIdKey((int)id));

            if (productImage.ProductId is null) return success;

            _cache.Evict(GetInAllImagesByProductIdKey(productImage.ProductId.Value));

            _cache.Evict(CacheKeyUtils.ProductImageFileNameInfo.GetByProductIdKey(productImage.ProductId.Value));

            _cache.Evict(CacheKeyUtils.Product.GetByIdKey(productImage.ProductId.Value));
        }

        return success;
    }

    public bool DeleteAllImagesForProduct(uint productId)
    {
        IEnumerable<ProductImage> imagesForProduct = GetAllInProduct(productId);

        if (!imagesForProduct.Any()) return false;

        bool success = _productImageService.DeleteAllImagesForProduct(productId);

        if (success)
        {
            foreach (ProductImage image in imagesForProduct)
            {
                _cache.Evict(GetInAllImagesByIdKey(image.Id));
            }

            _cache.Evict(GetInAllImagesByProductIdKey((int)productId));

            _cache.Evict(CacheKeyUtils.Product.GetByIdKey((int)productId));
        }

        return success;
    }

    public bool DeleteInFirstImagesByProductId(uint productId)
    {
        bool success = _productImageService.DeleteInFirstImagesByProductId(productId);

        if (success)
        {
            _cache.Evict(GetInFirstImagesByIdKey((int)productId));

            _cache.Evict(CacheKeyUtils.Product.GetByIdKey((int)productId));
        }

        return success;
    }
}