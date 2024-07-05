using FluentValidation;
using OneOf.Types;
using OneOf;
using FluentValidation.Results;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Models.Product.Models.Requests.ProductImageFileNameInfo;
using MOSTComputers.Services.Caching.Services.Contracts;
using static MOSTComputers.Services.ProductRegister.StaticUtilities.CacheKeyUtils.ProductImageFileNameInfo;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileNameInfo;
using MOSTComputers.Services.ProductRegister.StaticUtilities;

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
        return _cache.GetOrAdd(GetAllKey, _imageFileNameInfoService.GetAll);
    }

    public IEnumerable<ProductImageFileNameInfo> GetAllInProduct(uint productId)
    {
        return _cache.GetOrAdd(GetByProductIdKey((int)productId),
            () => _imageFileNameInfoService.GetAllInProduct(productId));
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

    public bool DeleteAllForProductId(uint productId)
    {
        bool success = _imageFileNameInfoService.DeleteAllForProductId(productId);

        if (success)
        {
            _cache.Evict(GetByProductIdKey((int)productId));

            _cache.Evict(GetAllKey);

            _cache.Evict(CacheKeyUtils.ForProduct.GetByIdKey((int)productId));
        }

        return success;
    }

    public bool DeleteByProductIdAndDisplayOrder(uint productId, int displayOrder)
    {
        bool success = _imageFileNameInfoService.DeleteByProductIdAndDisplayOrder(productId, displayOrder);

        if (success)
        {
            _cache.Evict(GetByProductIdKey((int)productId));

            _cache.Evict(GetAllKey);

            _cache.Evict(CacheKeyUtils.ForProduct.GetByIdKey((int)productId));
        }

        return success;
    }
}