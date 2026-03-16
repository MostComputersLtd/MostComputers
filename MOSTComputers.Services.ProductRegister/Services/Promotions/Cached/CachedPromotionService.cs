using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Promotions;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Contracts;
using ZiggyCreatures.Caching.Fusion;
using static MOSTComputers.Services.ProductRegister.Utils.Caching.CacheKeyUtils.ForPromotion;
using static MOSTComputers.Services.ProductRegister.Utils.SearchByIdsUtils;

namespace MOSTComputers.Services.ProductRegister.Services.Promotions.Cached;

public sealed class CachedPromotionService : IPromotionService
{
    public CachedPromotionService(
        IPromotionService promotionService,
        IFusionCache fusionCache)
    {
        _promotionService = promotionService;
        _fusionCache = fusionCache;
    }

    private static readonly TimeSpan _defaultItemAbsoluteExpiration = TimeSpan.FromMinutes(5);

    private static readonly TimeSpan _emptyItemAbsoluteExpiration = TimeSpan.FromMinutes(2);

    private readonly IPromotionService _promotionService;
    private readonly IFusionCache _fusionCache;

    public async Task<List<Promotion>> GetAllAsync()
    {
        return await _fusionCache.GetOrSetAsync(GetAllKey,
            async (cancellationToken) => await _promotionService.GetAllAsync(),
            _defaultItemAbsoluteExpiration);
    }

    public async Task<List<Promotion>> GetAllActiveAsync()
    {
        return await _fusionCache.GetOrSetAsync(GetAllActiveKey,
            async (cancellationToken) => await _promotionService.GetAllActiveAsync(),
            _defaultItemAbsoluteExpiration);
    }

    public async Task<List<IGrouping<int?, Promotion>>> GetAllActiveForSelectionOfProductsAsync(IEnumerable<int> productIds)
    {
        productIds = RemoveValuesSmallerThanOne(productIds.Distinct());

        Dictionary<int, List<Promotion>> cachedPromotions = new();
        List<int> missingProductIds = new();

        foreach (int productId in productIds)
        {
            MaybeValue<List<Promotion>?> cached = await _fusionCache.TryGetAsync<List<Promotion>?>(GetAllActiveForProductKey(productId));

            if (cached.HasValue)
            {
                cachedPromotions.Add(productId, cached.Value ?? new());
            }
            else
            {
                missingProductIds.Add(productId);
            }
        }

        if (missingProductIds.Count == 0)
        {
            return cachedPromotions
                .SelectMany(x => x.Value)
                .GroupBy(x => x.ProductId)
                .ToList();
        }

        List<IGrouping<int?, Promotion>> retrievedPromotions
            = await _promotionService.GetAllActiveForSelectionOfProductsAsync(missingProductIds);

        foreach (IGrouping<int?, Promotion> retrievedProductPromotions in retrievedPromotions)
        {
            if (retrievedProductPromotions.Key is null) continue;

            int productId = retrievedProductPromotions.Key.Value;

            await _fusionCache.SetAsync(GetAllActiveForProductKey(productId), retrievedProductPromotions.ToList());

            missingProductIds.Remove(productId);
        }

        foreach (int productIdWithoutPromotions in missingProductIds)
        {
            await _fusionCache.SetAsync<List<Promotion>?>(
                GetAllActiveForProductKey(productIdWithoutPromotions),
                null,
                options => options.SetDuration(_emptyItemAbsoluteExpiration));
        }

        return retrievedPromotions
            .SelectMany(x => x)
            .Concat(cachedPromotions.SelectMany(x => x.Value))
            .GroupBy(promotion => promotion.ProductId)
            .ToList();
    }

    public async Task<List<Promotion>> GetAllForProductAsync(int productId)
    {
        List<Promotion>? cachedPromotions = await _fusionCache.GetOrSetAsync<List<Promotion>?>(GetAllForProductKey(productId),
            async (entry, cancellationToken) =>
            {
                List<Promotion> promotionsForProduct = await _promotionService.GetAllForProductAsync(productId);

                if (promotionsForProduct.Count == 0)
                {
                    entry.Options.SetDuration(_emptyItemAbsoluteExpiration);
                }
                else
                {
                    entry.Options.SetDuration(_defaultItemAbsoluteExpiration);
                }

                return promotionsForProduct;
            });

        return cachedPromotions ?? new();
    }

    public async Task<List<IGrouping<int?, Promotion>>> GetAllForSelectionOfProductsAsync(IEnumerable<int> productIds)
    {
        productIds = RemoveValuesSmallerThanOne(productIds.Distinct());

        Dictionary<int, List<Promotion>> cachedPromotions = new();
        List<int> missingProductIds = new();

        foreach (int productId in productIds)
        {
            MaybeValue<List<Promotion>?> cached = await _fusionCache.TryGetAsync<List<Promotion>?>(GetAllForProductKey(productId));

            if (cached.HasValue)
            {
                cachedPromotions.Add(productId, cached.Value ?? new());
            }
            else
            {
                missingProductIds.Add(productId);
            }
        }

        if (missingProductIds.Count == 0)
        {
            return cachedPromotions
                .SelectMany(x => x.Value)
                .GroupBy(x => x.ProductId)
                .ToList();
        }

        List<IGrouping<int?, Promotion>> retrievedPromotions
            = await _promotionService.GetAllForSelectionOfProductsAsync(missingProductIds);

        foreach (IGrouping<int?, Promotion> retrievedProductPromotions in retrievedPromotions)
        {
            if (retrievedProductPromotions.Key is null) continue;

            int productId = retrievedProductPromotions.Key.Value;

            await _fusionCache.SetAsync(GetAllForProductKey(productId), retrievedProductPromotions.ToList());

            missingProductIds.Remove(productId);
        }

        foreach (int productIdWithoutPromotions in missingProductIds)
        {
            await _fusionCache.SetAsync<List<Promotion>?>(
                GetAllForProductKey(productIdWithoutPromotions),
                null,
                options => options.SetDuration(_emptyItemAbsoluteExpiration));
        }

        return retrievedPromotions
            .SelectMany(x => x)
            .Concat(cachedPromotions.SelectMany(x => x.Value))
            .GroupBy(promotion => promotion.ProductId)
            .ToList();
    }

    public async Task<List<Promotion>> GetAllActiveForProductAsync(int productId)
    {
        return await _fusionCache.GetOrSetAsync(GetAllActiveForProductKey(productId),
         async (cancellationToken) => await _promotionService.GetAllActiveForProductAsync(productId),
         _defaultItemAbsoluteExpiration);
    }
}