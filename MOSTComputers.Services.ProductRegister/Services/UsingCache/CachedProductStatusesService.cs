using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Requests.ProductStatuses;
using MOSTComputers.Services.Caching.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using OneOf;
using OneOf.Types;
using static MOSTComputers.Services.ProductRegister.StaticUtilities.CacheKeyUtils.ProductStatuses;
using static MOSTComputers.Services.ProductRegister.StaticUtilities.ProductDataCloningUtils;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Services.UsingCache;

internal sealed class CachedProductStatusesService : IProductStatusesService
{
    public CachedProductStatusesService(
        ProductStatusesService productStatusesService,
        ICache<string> cache)
    {
        _productStatusesService = productStatusesService;
        _cache = cache;
    }

    private readonly ProductStatusesService _productStatusesService;
    private readonly ICache<string> _cache;

    public IEnumerable<ProductStatuses> GetAll()
    {
        return _productStatusesService.GetAll();
    }

    public ProductStatuses? GetByProductId(int productId)
    {
        if (productId <= 0) return null;

        ProductStatuses? productStatuses = _cache.GetOrAdd(GetByProductIdKey(productId),
            () => _productStatusesService.GetByProductId(productId));

        if (productStatuses is null) return null;

        return Clone(productStatuses);
    }

    public IEnumerable<ProductStatuses> GetSelectionByProductIds(IEnumerable<int> productIds)
    {
        productIds = RemoveValuesSmallerThanOne(productIds);

        List<ProductStatuses>? cachedStatuses = null;

        foreach (int productId in productIds)
        {
            ProductStatuses? productStatuses = _cache.GetValueOrDefault<ProductStatuses>(GetByProductIdKey(productId));

            if (productStatuses is not null)
            {
                cachedStatuses ??= new();

                cachedStatuses.Add(productStatuses);
            }
        }

        if (cachedStatuses is not null)
        {
            productIds = productIds.Except(cachedStatuses.Select(status => status.ProductId));

            if (!productIds.Any())
            {
                return CloneAll(cachedStatuses);
            }
        }

        IEnumerable<ProductStatuses> statuses = _productStatusesService.GetSelectionByProductIds(productIds);

        foreach (ProductStatuses status in statuses)
        {
            _cache.Add(GetByProductIdKey(status.ProductId), status);
        }

        if (cachedStatuses is not null)
        {
            List<ProductStatuses> productStatusesClone = CloneAll(statuses);

            return productStatusesClone.Concat(cachedStatuses);
        }

        return CloneAll(statuses);
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
    public bool DeleteByProductId(int productId)
    {
        if (productId <= 0) return false;

        bool success = _productStatusesService.DeleteByProductId(productId);

        if (success)
        {
            _cache.Evict(GetByProductIdKey(productId));
        }

        return success;
    }
}