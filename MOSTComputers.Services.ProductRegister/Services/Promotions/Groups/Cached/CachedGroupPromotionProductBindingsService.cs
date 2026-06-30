using MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;
using OneOf;
using OneOf.Types;
using ZiggyCreatures.Caching.Fusion;

using static MOSTComputers.Services.ProductRegister.Utils.Caching.CacheKeyUtils.ForGroupPromotionProductBinding;

namespace MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Cached;

internal sealed class CachedGroupPromotionProductBindingsService : IGroupPromotionProductBindingsService
{
    private readonly IGroupPromotionProductBindingsService _groupPromotionContentsToProductsService;
    private readonly IFusionCache _fusionCache;

    public CachedGroupPromotionProductBindingsService(
        IGroupPromotionProductBindingsService groupPromotionContentsToProductsService,
        IFusionCache fusionCache)
    {
        _groupPromotionContentsToProductsService = groupPromotionContentsToProductsService;
        _fusionCache = fusionCache;
    }

    public async Task<Dictionary<int, List<int>>> GetAllAsync()
    {
        return await _fusionCache.GetOrSetAsync(GetAllKey,
            async (cancellationToken) => await _groupPromotionContentsToProductsService.GetAllAsync());
    }

    public async Task<List<int>> GetAllProductIdsBoundToPromotionAsync(int promotionId)
    {
        if (promotionId <= 0) return new();

        return await _fusionCache.GetOrSetAsync(GetAllForGroupPromotionKey(promotionId),
            async (cancellationToken) => await _groupPromotionContentsToProductsService.GetAllProductIdsBoundToPromotionAsync(promotionId));
    }

    public async Task<Dictionary<int, List<int>>> GetAllPromotionIdsBoundToProductsAsync(IEnumerable<int> productIds)
    {
        if (!productIds.Any()) return new Dictionary<int, List<int>>();

        IEnumerable<int> distinctProductIds = productIds.Distinct();

        Dictionary<int, List<int>> cachedPromotionIds = new();

        List<int> missingProductIds = new();

        foreach (int productId in distinctProductIds)
        {
            MaybeValue<List<int>> cached = await _fusionCache.TryGetAsync<List<int>>(GetAllForProductKey(productId));

            if (cached.HasValue && cached.Value.Count > 0)
            {
                cachedPromotionIds.Add(productId, cached.Value);
            }
            else
            {
                missingProductIds.Add(productId);
            }
        }

        if (missingProductIds.Count == 0) return cachedPromotionIds;

        Dictionary<int, List<int>> results
            = await _groupPromotionContentsToProductsService.GetAllPromotionIdsBoundToProductsAsync(missingProductIds);

        foreach (KeyValuePair<int, List<int>> kvp in results)
        {
            await _fusionCache.SetAsync(GetAllForProductKey(kvp.Key), kvp.Value);

            cachedPromotionIds.Add(kvp.Key, kvp.Value);
        }

        return cachedPromotionIds;
    }

    public async Task<List<int>> GetAllPromotionIdsBoundToProductAsync(int productId)
    {
        if (productId <= 0) return new();

        return await _fusionCache.GetOrSetAsync(GetAllForProductKey(productId),
            async (cancellationToken) => await _groupPromotionContentsToProductsService.GetAllPromotionIdsBoundToProductAsync(productId));
    }

    public async Task<OneOf<Success, NotFound>> UpsertAllAsync(int promotionId, List<int>? relatedProductIds)
    {
        List<int> existingBoundProductIds = await GetAllProductIdsBoundToPromotionAsync(promotionId);

        OneOf<Success, NotFound> result = await _groupPromotionContentsToProductsService.UpsertAllAsync(promotionId, relatedProductIds);

        if (result.IsT0)
        {
            await _fusionCache.RemoveAsync(GetAllForGroupPromotionKey(promotionId));

            foreach (int oldProductId in existingBoundProductIds)
            {
                await _fusionCache.RemoveAsync(GetAllForProductKey(oldProductId));
            }

            if (relatedProductIds?.Count > 0)
            {
                foreach (int productId in relatedProductIds)
                {
                    if (existingBoundProductIds.Contains(productId)) continue;

                    await _fusionCache.RemoveAsync(GetAllForProductKey(productId));
                }
            }
        }

        return result;
    }
}