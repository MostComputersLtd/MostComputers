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

namespace MOSTComputers.Services.ProductRegister.Services.UsingCache;

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

    public ProductImageFileNameInfo? GetByProductIdAndImageNumber(int productId, int imageNumber)
    {
        IEnumerable<ProductImageFileNameInfo>? fileNameInfos
            = _cache.GetValueOrDefault<IEnumerable<ProductImageFileNameInfo>>(GetByProductIdKey(productId));

        if (fileNameInfos is not null
            && fileNameInfos.Any())
        {
            foreach (ProductImageFileNameInfo fileNameInfo in fileNameInfos)
            {
                if (fileNameInfo.ImageNumber != imageNumber) continue;

                _cache.AddOrUpdate(GetByProductIdAndImageNumberKey(productId, imageNumber), fileNameInfo);

                return fileNameInfo;
            }
        }

        return _cache.GetOrAdd(GetByProductIdAndImageNumberKey(productId, imageNumber),
            () => _imageFileNameInfoService.GetByProductIdAndImageNumber(productId, imageNumber));
    }

    public ProductImageFileNameInfo? GetByFileName(string fileName)
    {
        ProductImageFileNameInfo? fileNameInfo = _imageFileNameInfoService.GetByFileName(fileName);

        if (fileNameInfo is null) return null;

        _cache.Add(GetByProductIdAndImageNumberKey(fileNameInfo.ProductId, fileNameInfo.ImageNumber), fileNameInfo);

        return fileNameInfo;
    }

    public int? GetHighestImageNumber(int productId)
    {
        IEnumerable<ProductImageFileNameInfo>? fileNameInfos
            = _cache.GetValueOrDefault<IEnumerable<ProductImageFileNameInfo>>(GetByProductIdKey(productId));

        if (fileNameInfos is null)
        {
            return _imageFileNameInfoService.GetHighestImageNumber(productId);
        }

        if (!fileNameInfos.Any()) return null;

        return fileNameInfos.Max(x => x.ImageNumber);
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
            int productId = createRequest.ProductId;

            int? highestImageNumber = GetHighestImageNumber(productId);

            _cache.Evict(GetByProductIdKey(productId));

            _cache.Evict(GetAllKey);

            EvictAllIndividualCachedItems(productId, highestImageNumber);

            _cache.Evict(CacheKeyUtils.ForProduct.GetByIdKey(productId));
        }

        return insertResult;
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByImageNumber(ServiceProductImageFileNameInfoByImageNumberUpdateRequest updateRequest,
        IValidator<ServiceProductImageFileNameInfoByImageNumberUpdateRequest>? validator = null)
    {
        OneOf<Success, ValidationResult, UnexpectedFailureResult> updateResult = _imageFileNameInfoService.UpdateByImageNumber(updateRequest);

        bool successFromResult = updateResult.Match(
            success => true,
            _ => false,
            _ => false);

        if (successFromResult)
        {
            int productId = updateRequest.ProductId;

            int? highestImageNumber = GetHighestImageNumber(productId);

            _cache.Evict(GetByProductIdKey(productId));

            EvictAllIndividualCachedItems(productId, highestImageNumber);

            _cache.Evict(GetAllKey);

            _cache.Evict(CacheKeyUtils.ForProduct.GetByIdKey(productId));
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
            int productId = updateRequest.ProductId;

            int? highestImageNumber = GetHighestImageNumber(productId);

            _cache.Evict(GetByProductIdKey(productId));

            _cache.Evict(GetAllKey);

            EvictAllIndividualCachedItems(productId, highestImageNumber);

            _cache.Evict(CacheKeyUtils.ForProduct.GetByIdKey(productId));
        }

        return updateResult;
    }

    public bool DeleteAllForProductId(int productId)
    {
        if (productId <= 0) return false;

        bool success = _imageFileNameInfoService.DeleteAllForProductId(productId);

        if (success)
        {
            int? highestImageNumber = GetHighestImageNumber(productId);

            _cache.Evict(GetByProductIdKey(productId));

            _cache.Evict(GetAllKey);

            EvictAllIndividualCachedItems(productId, highestImageNumber);

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
            int? highestImageNumber = GetHighestImageNumber(productId);

            _cache.Evict(GetByProductIdKey(productId));

            _cache.Evict(GetAllKey);

            EvictAllIndividualCachedItems(productId, highestImageNumber);

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
            int? highestImageNumber = GetHighestImageNumber(productId);

            _cache.Evict(GetByProductIdKey(productId));

            _cache.Evict(GetAllKey);

            EvictAllIndividualCachedItems(productId, highestImageNumber);

            _cache.Evict(CacheKeyUtils.ForProduct.GetByIdKey(productId));
        }

        return success;
    }

    private void EvictAllIndividualCachedItems(int productId, int? highestImageNumber)
    {
        if (highestImageNumber is not null)
        {
            for (int i = 1; i <= highestImageNumber.Value; i++)
            {
                _cache.Evict(GetByProductIdAndImageNumberKey(productId, i));
            }
        }
    }
}