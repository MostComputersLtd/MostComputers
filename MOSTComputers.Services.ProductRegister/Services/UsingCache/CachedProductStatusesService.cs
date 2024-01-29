using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.ProductStatuses;
using MOSTComputers.Services.Caching.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using OneOf;
using OneOf.Types;
using static MOSTComputers.Services.ProductRegister.StaticUtilities.CacheKeyUtils.ProductStatuses;

namespace MOSTComputers.Services.ProductRegister.Services.UsingCache;

internal sealed class CachedProductStatusesService : IProductStatusesService
{
    public CachedProductStatusesService(
        IProductStatusesService productStatusesService,
        ICache<string> cache)
    {
        _productStatusesService = productStatusesService;
        _cache = cache;
    }

    private readonly IProductStatusesService _productStatusesService;
    private readonly ICache<string> _cache;

    public IEnumerable<ProductStatuses> GetAll()
    {
        return _productStatusesService.GetAll();
    }

    public ProductStatuses? GetByProductId(uint productId)
    {
        return _cache.GetOrAdd(GetByProductIdKey((int)productId),
            () => _productStatusesService.GetByProductId(productId));
    }

    public IEnumerable<ProductStatuses> GetSelectionByProductIds(IEnumerable<uint> productIds)
    {
        List<ProductStatuses>? cachedStatuses = null;

        foreach (uint productId in productIds)
        {
            ProductStatuses? productStatuses = _cache.GetValueOrDefault<ProductStatuses>(GetByProductIdKey((int)productId));

            if (productStatuses is not null)
            {
                cachedStatuses ??= new();

                cachedStatuses.Add(productStatuses);
            }
        }

        if (cachedStatuses is not null)
        {
            productIds = productIds.Except(cachedStatuses.Select(status => (uint)status.ProductId));

            if (!productIds.Any())
            {
                return cachedStatuses;
            }
        }

        IEnumerable<ProductStatuses> statuses = _productStatusesService.GetSelectionByProductIds(productIds);

        if (cachedStatuses is not null)
        {
            return statuses.Concat(cachedStatuses);
        }

        return statuses;
    }

    public OneOf<Success, ValidationResult> InsertIfItDoesntExist(ProductStatusesCreateRequest createRequest)
    {
        OneOf<Success, ValidationResult> insertResult = _productStatusesService.InsertIfItDoesntExist(createRequest);

        bool successFromResult = insertResult.Match(
            success => true,
            _ => false);

        if (successFromResult)
        {
            _cache.Evict(GetByProductIdKey(createRequest.ProductId));
        }

        return insertResult;
    }

    public OneOf<bool, ValidationResult> Update(ProductStatusesUpdateRequest updateRequest)
    {
        OneOf<bool, ValidationResult> updateResult = _productStatusesService.Update(updateRequest);

        bool successFromResult = updateResult.Match(
            success => success,
            _ => false);

        if (successFromResult)
        {
            _cache.Evict(GetByProductIdKey(updateRequest.ProductId));
        }

        return updateResult;
    }
    public bool DeleteByProductId(uint productId)
    {
        bool success = _productStatusesService.DeleteByProductId(productId);

        if (success)
        {
            _cache.Evict(GetByProductIdKey((int)productId));
        }

        return success;
    }
}