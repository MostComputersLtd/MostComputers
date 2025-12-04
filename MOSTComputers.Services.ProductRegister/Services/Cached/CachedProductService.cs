using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.Caching.Models;
using MOSTComputers.Services.Caching.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using System.Collections.Generic;
using ZiggyCreatures.Caching.Fusion;
using static MOSTComputers.Services.ProductRegister.Configuration.ConfigureServices;
using static MOSTComputers.Services.ProductRegister.Utils.Caching.CachingDefaults;
using static MOSTComputers.Services.ProductRegister.Utils.Caching.CacheKeyUtils.ForProduct;
using static MOSTComputers.Services.ProductRegister.Utils.SearchByIdsUtils;

namespace MOSTComputers.Services.ProductRegister.Services.Cached;
internal sealed class CachedProductService : IProductService
{
    public CachedProductService(
        [FromKeyedServices(ProductServiceKey)] IProductService productService,
        //ICache<string> cache,
        IFusionCache fusionCache)
    {
        _productService = productService;
        //_cache = cache;
        _fusionCache = fusionCache;
    }

    private readonly IProductService _productService;
    //private readonly ICache<string> _cache;
    private readonly IFusionCache _fusionCache;

    private static readonly TimeSpan _defaultItemAbsoluteExpiration = TimeSpan.FromMinutes(5);

    //private readonly CustomCacheEntryOptions? _memoryCacheEntryOptions = new()
    //{
    //    AbsoluteExpirationRelativeToNow = _defaultItemAbsoluteExpiration,
    //};

    public async Task<List<Product>> GetAllAsync()
    {
        //List<Product> products = await _cache.GetOrAddAsync(GetAllKey, async () =>
        List<Product> products = await _fusionCache.GetOrSetAsync(GetAllKey, async (cancellationToken) =>
        {
            List<Product> data = await _productService.GetAllAsync();

            IEnumerable<IGrouping<int?, Product>> groupedProductsByCategory = data.GroupBy(x => x.CategoryId);

            foreach (IGrouping<int?, Product> productsInCategoryGroup in groupedProductsByCategory)
            {
                string productsInCategoryKey = GetAllInCategoryKey(productsInCategoryGroup.Key);

                //AddOrUpdateCache(productsInCategoryKey, productsInCategoryGroup.ToList());

                List<Product> productsInCategory = productsInCategoryGroup.ToList();

                await _fusionCache.SetAsync(productsInCategoryKey, productsInCategory, _defaultItemAbsoluteExpiration, token: cancellationToken);

                foreach (Product product in productsInCategory)
                {
                    string productKey = GetByIdKey(product.Id);

                    //AddOrUpdateCache(productKey, product);

                    await _fusionCache.SetAsync(productKey, product, _defaultItemAbsoluteExpiration, token: cancellationToken);
                }
            }

            IEnumerable<IGrouping<MOSTComputers.Models.Product.Models.ProductStatus?, Product>> groupedProductsByStatus = data.GroupBy(x => x.Status);

            foreach (IGrouping<MOSTComputers.Models.Product.Models.ProductStatus?, Product> productsWithStatusesGroup in groupedProductsByStatus)
            {
                if (productsWithStatusesGroup.Key is null) continue;

                string productsWithStatusKey = GetAllWithStatusKey(productsWithStatusesGroup.Key.Value);

                //AddOrUpdateCache(productsWithStatusKey, productsWithStatusesGroup.ToList());

                await _fusionCache.SetAsync(productsWithStatusKey, productsWithStatusesGroup.ToList(), _defaultItemAbsoluteExpiration, token: cancellationToken);
            }

            return data;
        },
        options => options.SetDuration(_defaultItemAbsoluteExpiration));

        return products;
    }

    public async Task<List<Product>> GetAllWithStatusesAsync(List<MOSTComputers.Models.Product.Models.ProductStatus> productStatuses)
    {
        if (productStatuses == null || productStatuses.Count == 0) return new();

        List<MOSTComputers.Models.Product.Models.ProductStatus> distinctProductStatuses = productStatuses.Distinct().ToList();
        List<Product> cachedProducts = new();
        List<MOSTComputers.Models.Product.Models.ProductStatus> missingProductStatuses = new();

        foreach (MOSTComputers.Models.Product.Models.ProductStatus productStatus in distinctProductStatuses)
        {
            //List<Product>? cached = _cache.GetValueOrDefault<List<Product>>(GetAllWithStatusKey(productStatus));

            MaybeValue<List<Product>> cached = await _fusionCache.TryGetAsync<List<Product>>(GetAllWithStatusKey(productStatus));

            if (cached.HasValue && cached.Value.Count > 0)
            {
                cachedProducts.AddRange(cached.Value);
            }
            else
            {
                missingProductStatuses.Add(productStatus);
            }
        }

        if (missingProductStatuses.Count == 0) return Order(cachedProducts);

        List<MOSTComputers.Models.Product.Models.ProductStatus> stillMissingProductStatuses = new();

        foreach (MOSTComputers.Models.Product.Models.ProductStatus productStatus in missingProductStatuses)
        {
            //List<Product>? cached = _cache.GetValueOrDefault<List<Product>>(GetAllWithStatusKey(productStatus));

            MaybeValue<List<Product>> cached = await _fusionCache.TryGetAsync<List<Product>>(GetAllWithStatusKey(productStatus));

            if (cached.HasValue && cached.Value.Count > 0)
            {
                cachedProducts.AddRange(cached.Value);
            }
            else
            {
                stillMissingProductStatuses.Add(productStatus);
            }
        }

        if (stillMissingProductStatuses.Count == 0) return Order(cachedProducts);

        List<Product> retrievedProducts = await _productService.GetAllWithStatusesAsync(stillMissingProductStatuses);

        IEnumerable<IGrouping<MOSTComputers.Models.Product.Models.ProductStatus?, Product>> productsWithStatusGroups = retrievedProducts.GroupBy(product => product.Status);

        foreach (IGrouping<MOSTComputers.Models.Product.Models.ProductStatus?, Product> productsWithStatusGroup in productsWithStatusGroups)
        {
            if (productsWithStatusGroup.Key is null) continue;

            List<Product> productsWithStatus = productsWithStatusGroup.ToList();

            string productsWithStatusKey = GetAllWithStatusKey(productsWithStatusGroup.Key.Value);

            //_cache.Add(productsWithStatusKey, productsWithStatus);

            await _fusionCache.SetAsync(productsWithStatusKey, productsWithStatus, _defaultItemAbsoluteExpiration);

            cachedProducts.AddRange(productsWithStatus);

            foreach (Product product in productsWithStatus)
            {
                //_cache.Add(GetByIdKey(product.Id), product);

                await _fusionCache.SetAsync(GetByIdKey(product.Id), product, _defaultItemAbsoluteExpiration);
            }
        }

        return Order(cachedProducts);
    }

    public async Task<List<Product>> GetAllInCategoriesAsync(List<int> categoryIds)
    {
        if (categoryIds == null || categoryIds.Count == 0) return new();

        List<int> distinctCategoryIds = categoryIds.Distinct().ToList();
        List<Product> cachedProducts = new();
        List<int> missingCategoryIds = new();

        foreach (int categoryId in distinctCategoryIds)
        {
            //List<Product>? cached = _cache.GetValueOrDefault<List<Product>>(GetAllInCategoryKey(categoryId));

            MaybeValue<List<Product>> cached = await _fusionCache.TryGetAsync<List<Product>>(GetAllInCategoryKey(categoryId));

            if (cached.HasValue && cached.Value.Count > 0)
            {
                cachedProducts.AddRange(cached.Value);
            }
            else
            {
                missingCategoryIds.Add(categoryId);
            }
        }

        if (missingCategoryIds.Count == 0) return Order(cachedProducts);

        List<int> stillMissingCategoryIds = new();

        foreach (int categoryId in missingCategoryIds)
        {
            //List<Product>? cached = _cache.GetValueOrDefault<List<Product>>(GetAllInCategoryKey(categoryId));

            MaybeValue<List<Product>> cached = await _fusionCache.TryGetAsync<List<Product>>(GetAllInCategoryKey(categoryId));

            if (cached.HasValue && cached.Value.Count > 0)
            {
                cachedProducts.AddRange(cached.Value);
            }
            else
            {
                stillMissingCategoryIds.Add(categoryId);
            }
        }

        if (stillMissingCategoryIds.Count == 0) return Order(cachedProducts);

        List<Product> retrievedProducts = await _productService.GetAllInCategoriesAsync(stillMissingCategoryIds);

        IEnumerable<IGrouping<int?, Product>> productsInCategoryGroups = retrievedProducts.GroupBy(product => product.CategoryId);

        foreach (IGrouping<int?, Product> productsInCategoryGroup in productsInCategoryGroups)
        {
            List<Product> productsInCategory = productsInCategoryGroup.ToList();

            string productsInCategoryKey = GetAllInCategoryKey(productsInCategoryGroup.Key);

            //_cache.Add(productsInCategoryKey, productsInCategory);

            await _fusionCache.SetAsync(productsInCategoryKey, productsInCategory, _defaultItemAbsoluteExpiration);

            cachedProducts.AddRange(productsInCategory);

            foreach (Product product in productsInCategory)
            {
                //_cache.Add(GetByIdKey(product.Id), product);

                await _fusionCache.SetAsync(GetByIdKey(product.Id), product, _defaultItemAbsoluteExpiration);
            }
        }

        return Order(cachedProducts);
    }

    public async Task<List<Product>> GetAllInCategoryAsync(int categoryId)
    {
        string key = GetAllInCategoryKey(categoryId);

        //List<Product> products = await _cache.GetOrAddAsync(key, async () =>
        //{
        //    List<Product> data = await _productService.GetAllInCategoryAsync(categoryId);

        //    foreach (Product product in data)
        //    {
        //        string productKey = GetByIdKey(product.Id);

        //        AddOrUpdateCache(productKey, product);
        //    }

        //    return data;
        //});

        List<Product> products = await _fusionCache.GetOrSetAsync(key, async (cancellationToken) =>
        {
            List<Product> data = await _productService.GetAllInCategoryAsync(categoryId);

            foreach (Product product in data)
            {
                string productKey = GetByIdKey(product.Id);

                await _fusionCache.SetAsync(productKey, product, _defaultItemAbsoluteExpiration, cancellationToken);
            }

            return data;
        },
        _defaultItemAbsoluteExpiration);

        return products.ToList();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        string productKey = GetByIdKey(id);

        //Product? product = await _cache.GetOrAddAsync(productKey, async (entry) =>
        //{
        //    Product? data = await _productService.GetByIdAsync(id);

        //    if (data is null)
        //    {
        //        entry.AbsoluteExpirationRelativeToNow = EmptyValuesCacheAbsoluteExpiration;

        //        return data;
        //    }

        //    return data;
        //});


        return await _fusionCache.GetOrSetAsync<Product?>(productKey, async (entry, _) =>
        {
            Product? data = await _productService.GetByIdAsync(id);

            if (data is null)
            {
                entry.Options.SetDuration(EmptyValuesCacheAbsoluteExpiration);

                return data;
            }

            entry.Options.SetDuration(_defaultItemAbsoluteExpiration);

            return data;
        });
    }

    public async Task<Product?> GetProductWithHighestIdAsync()
    {
        return await _productService.GetProductWithHighestIdAsync();
    }

    public async Task<List<Product>> GetByIdsAsync(List<int> ids)
    {
        if (ids.Count <= 0) return new();

        ids = RemoveValuesSmallerThanOne(ids);

        List<Product> cachedProducts = new();

        for (int i = 0; i < ids.Count; i++)
        {
            int id = ids[i];

            string productKey = GetByIdKey(id);

            //Product? cachedProduct = _cache.GetValueOrDefault<Product>(productKey);

            MaybeValue<Product?> cachedProduct = await _fusionCache.TryGetAsync<Product?>(productKey);

            if (!cachedProduct.HasValue || cachedProduct.Value is null) continue;

            cachedProducts.Add(cachedProduct.Value);

            ids.RemoveAt(i);

            i--;
        }

        if (ids.Count <= 0) return Order(cachedProducts);

        List<Product> products = await _productService.GetByIdsAsync(ids);

        foreach (Product product in products)
        {
            string productKey = GetByIdKey(product.Id);

            //AddOrUpdateCache(productKey, product);

            await _fusionCache.SetAsync(productKey, product);
        }

        cachedProducts.AddRange(products);

        return Order(cachedProducts);
    }

    private static List<Product> Order(List<Product> products)
    {
        return products
            .OrderBy(product => product.DisplayOrder ?? int.MaxValue)
            .ToList();
    }

    //private bool AddOrUpdateCache<TResult>(string key, TResult result)
    //{
    //    if (_memoryCacheEntryOptions is null)
    //    {
    //        return _cache.AddOrUpdate(key, result);
    //    }

    //    return _cache.AddOrUpdate(key, result, _memoryCacheEntryOptions);
    //}
}