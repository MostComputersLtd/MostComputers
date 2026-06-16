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

    public async Task<List<int>> GetAllProductIdsBoundToPromotionAsync(int promotionId)
    {
        if (promotionId <= 0) return new();

        return await _fusionCache.GetOrSetAsync(GetAllForGroupPromotionKey(promotionId),
            async (cancellationToken) => await _groupPromotionContentsToProductsService.GetAllProductIdsBoundToPromotionAsync(promotionId));
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