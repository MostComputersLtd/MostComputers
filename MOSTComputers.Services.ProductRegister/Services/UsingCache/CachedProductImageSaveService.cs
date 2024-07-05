using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.Caching.Models;
using MOSTComputers.Services.Caching.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using static MOSTComputers.Services.ProductRegister.StaticUtilities.CacheKeyUtils.ProductImageSaveKeys;

namespace MOSTComputers.Services.ProductRegister.Services.UsingCache;

internal class CachedProductImageSaveService : IProductImageSaveService
{
    public CachedProductImageSaveService(ICache<string> cache)
    {
        _cache = cache;
    }

    private readonly ICache<string> _cache;

    public bool AddImageToProduct(int productId, ProductImage image, ProductImageFileNameInfo productImageFileNameInfo)
    {
        string productImagesCacheKey = GetProductImagesByProductIdKey(productId);

        Dictionary<ProductImage, ProductImageFileNameInfo>? productImageDictionary
            = _cache.GetValueOrDefault<Dictionary<ProductImage, ProductImageFileNameInfo>>(productImagesCacheKey);

        if (productImageDictionary is null)
        {
            _cache.Add(productImagesCacheKey, new Dictionary<ProductImage, ProductImageFileNameInfo>()
            {
                { image, productImageFileNameInfo }
            },
            new CustomMemoryCacheEntryOptions()
            {
                Priority = CustomCacheItemPriorityEnum.NeverRemove
            });

            return true;
        }

        productImageDictionary.Add(image, productImageFileNameInfo);

        return true;
    }

    public Dictionary<ProductImage, ProductImageFileNameInfo>? GetImagesForProduct(int productId)
    {
        string productImagesCacheKey = GetProductImagesByProductIdKey(productId);

        return _cache.GetValueOrDefault<Dictionary<ProductImage, ProductImageFileNameInfo>>(productImagesCacheKey);
    }

    public bool AddOrUpdateImagesOfProduct(int productId, Dictionary<ProductImage, ProductImageFileNameInfo> productUpdatedImages)
    {
        string productImagesCacheKey = GetProductImagesByProductIdKey(productId);

        _cache.AddOrUpdate(productImagesCacheKey, productUpdatedImages, new CustomMemoryCacheEntryOptions()
        {
            Priority = CustomCacheItemPriorityEnum.NeverRemove
        });

        return true;
    }
}