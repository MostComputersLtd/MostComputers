using FluentValidation;
using OneOf.Types;
using OneOf;
using FluentValidation.Results;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.Caching.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileNameInfo;
using MOSTComputers.Services.ProductRegister.StaticUtilities;
using static MOSTComputers.Services.ProductRegister.StaticUtilities.CacheKeyUtils.ProductImageFileNameInfo;
using static MOSTComputers.Services.ProductRegister.StaticUtilities.ProductDataCloningUtils;

namespace MOSTComputers.Services.ProductRegister.Services;

internal sealed class CachedProductImageFileNameInfoService : IProductImageFileNameInfoService
{
    public CachedProductImageFileNameInfoService(
        ProductImageFileNameInfoService imageFileNameInfoService,
        ICache<string> cache)
    {
        _imageFileNameInfoService = imageFileNameInfoService;
        _cache = cache;
    }

    private readonly ProductImageFileNameInfoService _imageFileNameInfoService;
    private readonly ICache<string> _cache;

    public IEnumerable<ProductImageFileNameInfo> GetAll()
    {
        IEnumerable<ProductImageFileNameInfo> productImageFileNameInfos = _cache.GetOrAdd(GetAllKey, _imageFileNameInfoService.GetAll);

        return CloneAll(productImageFileNameInfos);
    }

    public IEnumerable<ProductImageFileNameInfo> GetAllInProduct(int productId)
    {
        if (productId <= 0) return Enumerable.Empty<ProductImageFileNameInfo>();

        IEnumerable<ProductImageFileNameInfo> productImageFileNameInfos = _cache.GetOrAdd(GetByProductIdKey(productId),
            () => _imageFileNameInfoService.GetAllInProduct(productId));

        return CloneAll(productImageFileNameInfos);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Insert(ServiceProductImageFileNameInfoCreateRequest createRequest,
        IValidator<ServiceProductImageFileNameInfoCreateRequest>? validator = null)
    {
        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertResult = _imageFileNameInfoService.Insert(createRequest);

        bool successFromResult = insertResult.Match(
            success => true,
            _ => false,
            _ => false);

        if (successFromResult)
        {
            _cache.Evict(GetByProductIdKey(createRequest.ProductId));

            _cache.Evict(GetAllKey);

            _cache.Evict(CacheKeyUtils.ForProduct.GetByIdKey(createRequest.ProductId));
        }

        return insertResult;
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ServiceProductImageFileNameInfoByImageNumberUpdateRequest updateRequest,
        IValidator<ServiceProductImageFileNameInfoByImageNumberUpdateRequest>? validator = null)
    {
        OneOf<Success, ValidationResult, UnexpectedFailureResult> updateResult = _imageFileNameInfoService.Update(updateRequest);

        bool successFromResult = updateResult.Match(
            success => true,
            _ => false,
            _ => false);

        if (successFromResult)
        {
            _cache.Evict(GetByProductIdKey(updateRequest.ProductId));

            _cache.Evict(GetAllKey);

            _cache.Evict(CacheKeyUtils.ForProduct.GetByIdKey(updateRequest.ProductId));
        }

        return updateResult;
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByFileName(
        ServiceProductImageFileNameInfoByFileNameUpdateRequest updateRequest,
        IValidator<ServiceProductImageFileNameInfoByFileNameUpdateRequest>? validator = null)
    {
        OneOf<Success, ValidationResult, UnexpectedFailureResult> updateResult = _imageFileNameInfoService.UpdateByFileName(updateRequest, validator);

        bool successFromResult = updateResult.Match(
            success => true,
            _ => false,
            _ => false);

        if (successFromResult)
        {
            _cache.Evict(GetByProductIdKey(updateRequest.ProductId));

            _cache.Evict(GetAllKey);

            _cache.Evict(CacheKeyUtils.ForProduct.GetByIdKey(updateRequest.ProductId));
        }

        return updateResult;
    }

    public bool DeleteAllForProductId(int productId)
    {
        if (productId <= 0) return false;

        bool success = _imageFileNameInfoService.DeleteAllForProductId(productId);

        if (success)
        {
            _cache.Evict(GetByProductIdKey(productId));

            _cache.Evict(GetAllKey);

            _cache.Evict(CacheKeyUtils.ForProduct.GetByIdKey(productId));
        }

        return success;
    }

    public bool DeleteByProductIdAndDisplayOrder(int productId, int displayOrder)
    {
        if (productId <= 0 || displayOrder <= 0) return false;

        bool success = _imageFileNameInfoService.DeleteByProductIdAndDisplayOrder(productId, displayOrder);

        if (success)
        {
            _cache.Evict(GetByProductIdKey(productId));

            _cache.Evict(GetAllKey);

            _cache.Evict(CacheKeyUtils.ForProduct.GetByIdKey(productId));
        }

        return success;
    }

    public bool DeleteByProductIdAndImageNumber(int productId, int imageNumber)
    {
        if (productId <= 0 || imageNumber <= 0) return false;

        bool success = _imageFileNameInfoService.DeleteByProductIdAndImageNumber(productId, imageNumber);

        if (success)
        {
            _cache.Evict(GetByProductIdKey(productId));

            _cache.Evict(GetAllKey);

            _cache.Evict(CacheKeyUtils.ForProduct.GetByIdKey(productId));
        }

        return success;
    }
}