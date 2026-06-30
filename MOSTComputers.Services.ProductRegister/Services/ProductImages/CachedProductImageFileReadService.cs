using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using ZiggyCreatures.Caching.Fusion;

using static MOSTComputers.Services.ProductRegister.Utils.Caching.CacheKeyUtils.ForProductImageFile;
using static MOSTComputers.Services.ProductRegister.Utils.Caching.CachingDefaults;

namespace MOSTComputers.Services.ProductRegister.Services.ProductImages;

internal sealed class CachedProductImageFileReadService : IProductImageFileReadService
{
    public CachedProductImageFileReadService(
        IProductImageFileReadService productImageFileService,
        IFusionCache fusionCache)
    {
        _productImageFileReadService = productImageFileService;
        _fusionCache = fusionCache;
    }

    private readonly IProductImageFileReadService _productImageFileReadService;
    private readonly IFusionCache _fusionCache;

    public async Task<List<ProductImageFileData>> GetAllAsync()
    {
        List<ProductImageFileData> retrievedProductImageFiles = await _productImageFileReadService.GetAllAsync();

        foreach (ProductImageFileData retrievedProductImageFile in retrievedProductImageFiles)
        {
            await SetByIdAndByImageIdCacheEntriesAsync(retrievedProductImageFile);
        }

        return retrievedProductImageFiles;
    }

    public async Task<List<IGrouping<int, ProductImageFileData>>> GetAllInProductsAsync(IEnumerable<int> productIds)
    {
        List<int> productIdsList = productIds.Distinct().ToList();

        List<int> productIdsListMissingInCache = new();

        List<ProductImageFileData> cachedProductImageFiles = new();

        for (int i = 0; i < productIdsList.Count; i++)
        {
            int productId = productIdsList[i];

            string productImageFilesForProductKey = GetAllByProductIdKey(productId);

            //List<ProductImageFileNameInfo>? cachedProductImageFilesForProduct
            //    = _cache.GetValueOrDefault<List<ProductImageFileNameInfo>>(productImageFilesForProductKey);

            MaybeValue<List<ProductImageFileData>> cachedProductImageFilesForProduct
                = await _fusionCache.TryGetAsync<List<ProductImageFileData>>(productImageFilesForProductKey);

            if (!cachedProductImageFilesForProduct.HasValue)
            {
                productIdsListMissingInCache.Add(productId);

                continue;
            }

            cachedProductImageFiles.AddRange(cachedProductImageFilesForProduct.Value);
        }

        if (productIdsListMissingInCache.Count <= 0)
        {
            return cachedProductImageFiles.GroupBy(property => property.ProductId)
                .ToList();
        }

        List<IGrouping<int, ProductImageFileData>> retrievedProperties = await _productImageFileReadService.GetAllInProductsAsync(productIdsListMissingInCache);

        foreach (IGrouping<int, ProductImageFileData> newProductImageFilesGroup in retrievedProperties)
        {
            string productImageFilesForProductKey = GetAllByProductIdKey(newProductImageFilesGroup.Key);

            //_cache.AddOrUpdate(productImageFilesForProductKey, newProductImageFilesGroup.ToList());

            await _fusionCache.SetAsync(productImageFilesForProductKey, newProductImageFilesGroup.ToList());

            foreach (ProductImageFileData productImageFile in newProductImageFilesGroup)
            {
                await SetByIdAndByImageIdCacheEntriesAsync(productImageFile);
            }

            productIdsListMissingInCache.Remove(newProductImageFilesGroup.Key);
        }

        foreach (int productIdWithoutImageFiles in productIdsListMissingInCache)
        {
            List<ProductImageFileData> productImageFiles = new();

            string productImageFilesForProductKey = GetAllByProductIdKey(productIdWithoutImageFiles);

            //_cache.AddOrUpdate(productImageFilesForProductKey, productImageFiles);

            await _fusionCache.SetAsync(productImageFilesForProductKey, productImageFiles);
        }

        return retrievedProperties
            .SelectMany(x => x)
            .Concat(cachedProductImageFiles)
            .GroupBy(property => property.ProductId)
            .ToList();
    }

    public async Task<List<ProductImageFileData>> GetAllInProductAsync(int productId)
    {
        string cacheKey = GetAllByProductIdKey(productId);

        //List<ProductImageFileNameInfo> retrievedProductImageFiles = await _cache.GetOrAddAsync(cacheKey, async () =>
        //{
        //    List<ProductImageFileNameInfo> data = await _productImageFileService.GetAllInProductAsync(productId);

        //    foreach (ProductImageFileNameInfo retrievedProductImageFile in data)
        //    {
        //        await AddOrUpdateByIdAndByImageIdCacheEntriesAsync(retrievedProductImageFile);
        //    }

        //    return data;
        //});

        List<ProductImageFileData> retrievedProductImageFiles = await _fusionCache.GetOrSetAsync(cacheKey,
            async (_) =>
            {
                List<ProductImageFileData> data = await _productImageFileReadService.GetAllInProductAsync(productId);

                foreach (ProductImageFileData productImageFile in data)
                {
                    await SetByIdAndByImageIdCacheEntriesAsync(productImageFile);
                }

                return data;
            });

        return retrievedProductImageFiles.ToList();
    }

    public async Task<ProductImageFileData?> GetByIdAsync(int id)
    {
        string cacheKey = GetByIdKey(id);

        //return await _cache.GetOrAddAsync(cacheKey, async () =>
        //{
        //    ProductImageFileNameInfo? data = await _productImageFileService.GetByIdAsync(id);

        //    if (data is null)
        //    {
        //        entry.AbsoluteExpirationRelativeToNow = EmptyValuesCacheAbsoluteExpiration\
        
        //        return null;
        //    }

        //    if (data.ImageId is not null)
        //    {
        //        string getByProductAndImageIdKey = GetByProductIdAndImageIdKey(data.ProductId, data.ImageId.Value);

        //        _cache.AddOrUpdate(getByProductAndImageIdKey, data);
        //    }

        //    return data;
        //});

        return await _fusionCache.GetOrSetAsync<ProductImageFileData?>(cacheKey, async (entry, cancellationToken) =>
        {
            ProductImageFileData? data = await _productImageFileReadService.GetByIdAsync(id);

            if (data is null)
            {
                entry.Options.SetDuration(EmptyValuesCacheAbsoluteExpiration);

                return data;
            }

            if (data.ImageId is not null)
            {
                string getByProductAndImageIdKey = GetByProductIdAndImageIdKey(data.ProductId, data.ImageId.Value);

                await _fusionCache.SetAsync(getByProductAndImageIdKey, data, token: cancellationToken);
            }

            return data;
        });
    }

    public async Task<ProductImageFileData?> GetByProductIdAndImageIdAsync(int productId, int imageId)
    {
        string cacheKey = GetByProductIdAndImageIdKey(productId, imageId);

        //ProductImageFileNameInfo? retrievedProductImageFile = await _cache.GetOrAddAsync(cacheKey, async () =>
        //{
        //    ProductImageFileNameInfo? data = await _productImageFileService.GetByProductIdAndImageIdAsync(productId, imageId);

        //    if (data is null) return null;

        //    string getByIdKey = GetByIdKey(data.Id);

        //    _cache.AddOrUpdate(getByIdKey, data);

        //    return data;
        //});

        ProductImageFileData? retrievedProductImageFile = await _fusionCache.GetOrSetAsync(cacheKey, async (cancellationToken) =>
        {
            ProductImageFileData? data = await _productImageFileReadService.GetByProductIdAndImageIdAsync(productId, imageId);

            if (data is null) return null;

            string getByIdKey = GetByIdKey(data.Id);

            await _fusionCache.SetAsync(getByIdKey, data, token: cancellationToken);

            return data;
        });

        return retrievedProductImageFile;
    }

    private async Task SetByIdAndByImageIdCacheEntriesAsync(ProductImageFileData productImageFile)
    {
        string getByIdKey = GetByIdKey(productImageFile.Id);

        //_cache.AddOrUpdate(getByIdKey, productImageFile);

        await _fusionCache.SetAsync(getByIdKey, productImageFile);

        if (productImageFile.ImageId is not null)
        {
            string getByProductAndImageIdKey = GetByProductIdAndImageIdKey(productImageFile.ProductId, productImageFile.ImageId.Value);

            //_cache.AddOrUpdate(getByProductAndImageIdKey, productImageFile);

            await _fusionCache.SetAsync(getByProductAndImageIdKey, productImageFile);
        }
    }
}
