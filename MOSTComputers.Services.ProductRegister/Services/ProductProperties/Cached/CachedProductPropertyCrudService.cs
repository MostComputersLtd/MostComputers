using FluentValidation.Results;
using OneOf;
using OneOf.Types;
using System.Collections.Concurrent;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.Caching.Services.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductProperty;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductProperty;

using static MOSTComputers.Services.ProductRegister.Utils.Caching.CacheKeyUtils.ForProductProperty;
using static MOSTComputers.Services.ProductRegister.Utils.Caching.CachingDefaults;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductProperties.Contacts;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.ProductProperties;
using ZiggyCreatures.Caching.Fusion;

namespace MOSTComputers.Services.ProductRegister.Services.ProductProperties.Cached;

internal sealed class CachedProductPropertyCrudService : IProductPropertyCrudService
{
    public CachedProductPropertyCrudService(
        IProductPropertyCrudService productPropertyCrudService,
        IProductRepository productRepository,
        //ICache<string> cache,
        IFusionCache fusionCache)
    {
        _productPropertyCrudService = productPropertyCrudService;
        _productRepository = productRepository;
        //_cache = cache;
        _fusionCache = fusionCache;
    }

    private readonly IProductPropertyCrudService _productPropertyCrudService;
    private readonly IProductRepository _productRepository;
    //private readonly ICache<string> _cache;
    private readonly IFusionCache _fusionCache;

    //private static readonly ConcurrentDictionary<int, CancellationTokenSource> _cacheEntriesByProductCancellationTokens = new();

    //private static CustomCacheEntryOptions GetCacheEntryOptionsForProduct(int productId)
    //{
    //    CancellationTokenSource cts = _cacheEntriesByProductCancellationTokens.GetOrAdd(productId, _ => new CancellationTokenSource());

    //    CustomCacheEntryOptions cacheEntryOptions = new();

    //    cacheEntryOptions.ExpirationTokens.Add(new CancellationChangeToken(cts.Token));

    //    return cacheEntryOptions;
    //}

    //private static void InvalidateAllEntriesForProduct(int productId)
    //{
    //    if (_cacheEntriesByProductCancellationTokens.TryRemove(productId, out CancellationTokenSource? cts))
    //    {
    //        cts.Cancel();
    //        cts.Dispose();
    //    }
    //}

    public async Task<List<ProductProperty>> GetAllAsync()
    {
        return await _productPropertyCrudService.GetAllAsync();
    }

    public async Task<List<IGrouping<int, ProductProperty>>> GetAllInProductsAsync(IEnumerable<int> productIds)
    {
        List<int> productIdsList = productIds.Distinct().ToList();

        List<ProductProperty> cachedProperties = new();

        for (int i = 0; i < productIdsList.Count; i++)
        {
            int productId = productIdsList[i];

            string propertiesForProductKey = GetAllInProductKey(productId);

            //List<ProductProperty>? cachedPropertiesForProduct = await _cache.GetValueOrDefault<List<ProductProperty>>(propertiesForProductKey);

            MaybeValue<List<ProductProperty>> cachedPropertiesForProduct = await _fusionCache.TryGetAsync<List<ProductProperty>>(propertiesForProductKey);

            //if (cachedPropertiesForProduct is null)
            //{
            //    continue;
            //}

            if (!cachedPropertiesForProduct.HasValue
                || cachedPropertiesForProduct.Value is null)
            {
                continue;
            }

            //cachedProperties.AddRange(cachedPropertiesForProduct);
            cachedProperties.AddRange(cachedPropertiesForProduct.Value);

            productIdsList.RemoveAt(i);

            i--;
        }

        if (productIdsList.Count <= 0)
        {
            return cachedProperties
                .GroupBy(property => property.ProductId)
                .ToList();
        }

        List<int> validProductIds = await _productRepository.GetOnlyExistingIdsAsync(productIdsList);

        List<IGrouping<int, ProductProperty>> retrievedProperties = await _productPropertyCrudService.GetAllInProductsAsync(validProductIds);

        foreach (IGrouping<int, ProductProperty> newPropertyGroup in retrievedProperties)
        {
            string propertiesForProductKey = GetAllInProductKey(newPropertyGroup.Key);

            //CustomCacheEntryOptions cacheEntryOptions = GetCacheEntryOptionsForProduct(productIdWithoutProperties);

            //_cache.AddOrUpdate(propertiesForProductKey, newPropertyGroup.ToList(), cacheEntryOptions);

            await _fusionCache.SetAsync(propertiesForProductKey, newPropertyGroup.ToList(), tags: [GetProductIdTag(newPropertyGroup.Key)]);

            foreach (ProductProperty property in newPropertyGroup)
            {
                if (property.ProductCharacteristicId is null) continue;

                string propertyByProductAndCharacteristicIdKey = GetByProductAndCharacteristicIdKey(property.ProductId, property.ProductCharacteristicId.Value);

                //_cache.AddOrUpdate(propertyByProductAndCharacteristicIdKey, property, cacheEntryOptions);

                List<string> tags = [GetProductIdTag(newPropertyGroup.Key)];

                if (property.ProductCharacteristicId is not null)
                {
                    tags.Add(GetCharacteristicIdTag(property.ProductCharacteristicId.Value));
                }

                await _fusionCache.SetAsync(propertyByProductAndCharacteristicIdKey, property, tags: tags);
            }

            validProductIds.Remove(newPropertyGroup.Key);
        }

        foreach (int productIdWithoutProperties in validProductIds)
        {
            List<ProductProperty> propertiesForProduct = new();

            string propertiesForProductKey = GetAllInProductKey(productIdWithoutProperties);

            //CustomCacheEntryOptions cacheEntryOptions = GetCacheEntryOptionsForProduct(productIdWithoutProperties);

            //_cache.AddOrUpdate(propertiesForProductKey, propertiesForProduct, cacheEntryOptions);

            await _fusionCache.SetAsync(propertiesForProductKey, propertiesForProduct, tags: [GetProductIdTag(productIdWithoutProperties)]);
        }

        return retrievedProperties
            .SelectMany(x => x)
            .Concat(cachedProperties)
            .GroupBy(property => property.ProductId)
            .ToList();
    }

    public async Task<List<ProductPropertiesForProductCountData>> GetCountOfAllInProductsAsync(IEnumerable<int> productIds)
    {
        List<int> productIdsList = productIds.Distinct().ToList();

        List<ProductPropertiesForProductCountData> cachedPropertyCounts = new();

        for (int i = 0; i < productIdsList.Count; i++)
        {
            int productId = productIdsList[i];

            string countOfPropertiesForProductKey = GetCountOfAllInProductKey(productId);

            MaybeValue<int> countOfPropertiesForProduct = await _fusionCache.TryGetAsync<int>(countOfPropertiesForProductKey);

            if (!countOfPropertiesForProduct.HasValue) continue;

            ProductPropertiesForProductCountData productPropertiesCountData = new()
            {
                ProductId = productId,
                PropertyCount = countOfPropertiesForProduct.Value,
            };

            cachedPropertyCounts.Add(productPropertiesCountData);

            productIdsList.RemoveAt(i);

            i--;
        }

        if (productIdsList.Count <= 0)
        {
            return cachedPropertyCounts;
        }

        List<int> validProductIds = await _productRepository.GetOnlyExistingIdsAsync(productIdsList);

        List<ProductPropertiesForProductCountData> retrievedCounts = await _productPropertyCrudService.GetCountOfAllInProductsAsync(validProductIds);

        foreach (ProductPropertiesForProductCountData newCount in retrievedCounts)
        {
            string countOfPropertiesForProductKey = GetCountOfAllInProductKey(newCount.ProductId);

            //CustomCacheEntryOptions cacheEntryOptions = GetCacheEntryOptionsForProduct(newCount.ProductId);

            //_cache.AddOrUpdate(countOfPropertiesForProductKey, newCount, cacheEntryOptions);

            await _fusionCache.SetAsync(countOfPropertiesForProductKey, newCount.PropertyCount, tags: [GetProductIdTag(newCount.ProductId)]);

            validProductIds.Remove(newCount.ProductId);
        }

        foreach (int productIdWithoutCounts in validProductIds)
        {
            string countOfPropertiesForProductKey = GetCountOfAllInProductKey(productIdWithoutCounts);

            //CustomCacheEntryOptions cacheEntryOptions = GetCacheEntryOptionsForProduct(productIdWithoutCounts);

            //_cache.AddOrUpdate(countOfPropertiesForProductKey, 0, cacheEntryOptions);

            await _fusionCache.SetAsync(countOfPropertiesForProductKey, 0, tags: [GetProductIdTag(productIdWithoutCounts)]);
        }

        retrievedCounts.AddRange(cachedPropertyCounts);

        return retrievedCounts;
    }

    public async Task<List<ProductProperty>> GetAllInProductAsync(int productId)
    {
        string cacheKey = GetAllInProductKey(productId);

        //List<ProductProperty>? cachedProperties = _cache.GetValueOrDefault<List<ProductProperty>>(cacheKey);

        //if (cachedProperties is not null) return cachedProperties.ToList();

        //List<ProductProperty> retrievedProperties = await _productPropertyCrudService.GetAllInProductAsync(productId);

        //CustomCacheEntryOptions cacheEntryOptions = GetCacheEntryOptionsForProduct(productId);

        //_cache.AddOrUpdate(cacheKey, retrievedProperties, cacheEntryOptions);

        List<ProductProperty> retrievedProperties = await _fusionCache.GetOrSetAsync(cacheKey,
            async (cancellationToken) =>
            {
                List<ProductProperty> data = await _productPropertyCrudService.GetAllInProductAsync(productId);

                foreach (ProductProperty property in data)
                {
                    if (property.ProductCharacteristicId is null) continue;

                    string propertyByProductAndCharacteristicIdKey = GetByProductAndCharacteristicIdKey(productId, property.ProductCharacteristicId.Value);

                    List<string> tags = [GetProductIdTag(productId)];

                    if (property.ProductCharacteristicId is not null)
                    {
                        tags.Add(GetCharacteristicIdTag(property.ProductCharacteristicId.Value));
                    }

                    await _fusionCache.SetAsync(propertyByProductAndCharacteristicIdKey, property, tags: tags, token: cancellationToken);
                }

                return data;
            },
            tags: [GetProductIdTag(productId)]);

        return retrievedProperties.ToList();
    }

    public async Task<int> GetCountOfAllInProductAsync(int productId)
    {
        string cacheKey = GetCountOfAllInProductKey(productId);

        //CustomCacheEntryOptions cacheEntryOptions = GetCacheEntryOptionsForProduct(productId);

        //return await _cache.GetOrAdd(cacheKey, () => _productPropertyCrudService.GetCountOfAllInProductAsync(productId), cacheEntryOptions)!;

        return await _fusionCache.GetOrSetAsync(cacheKey,
            (_) => _productPropertyCrudService.GetCountOfAllInProductAsync(productId),
            tags: [GetProductIdTag(productId)]);
    }

    public async Task<ProductProperty?> GetByProductAndCharacteristicIdAsync(int productId, int characteristicId)
    {
        string cacheKey = GetByProductAndCharacteristicIdKey(productId, characteristicId);

        //ProductProperty? cachedProperty = _cache.GetValueOrDefault<ProductProperty>(cacheKey);

        //if (cachedProperty is not null) return cachedProperty;

        //string allInProductCacheKey = GetAllInProductKey(productId);

        //List<ProductProperty>? allPropertiesForProductCached = _cache.GetValueOrDefault<List<ProductProperty>>(allInProductCacheKey);

        //ProductProperty? cachedPropertyFromAllProducts = allPropertiesForProductCached?
        //    .FirstOrDefault(cachedProperty => cachedProperty.ProductCharacteristicId == characteristicId);

        //CustomCacheEntryOptions cacheEntryOptions = GetCacheEntryOptionsForProduct(productId);

        //if (cachedPropertyFromAllProducts is not null)
        //{
            //_cache.AddOrUpdate(cacheKey, cachedPropertyFromAllProducts, cacheEntryOptions);

        //    return cachedPropertyFromAllProducts;
        //}

        //return await _cache.GetOrAddAsync(cacheKey, () => _productPropertyCrudService.GetByProductAndCharacteristicIdAsync(productId, characteristicId), cacheEntryOptions);

        return await _fusionCache.GetOrSetAsync<ProductProperty?>(cacheKey,
            async (entry, cancellationToken) =>
            {
                string allInProductCacheKey = GetAllInProductKey(productId);

                MaybeValue<List<ProductProperty>> allPropertiesForProductCached
                    = await _fusionCache.TryGetAsync<List<ProductProperty>>(allInProductCacheKey, token: cancellationToken);

                if (allPropertiesForProductCached.HasValue)
                {
                    ProductProperty? cachedPropertyFromAllProducts = allPropertiesForProductCached.Value
                        .FirstOrDefault(cachedProperty => cachedProperty.ProductCharacteristicId == characteristicId);

                    return cachedPropertyFromAllProducts;
                }

                ProductProperty? data = await _productPropertyCrudService.GetByProductAndCharacteristicIdAsync(productId, characteristicId);

                if (data is null)
                {
                    entry.Options.SetDuration(EmptyValuesCacheAbsoluteExpiration);
                }

                return data;
            },
            tags: [GetProductIdTag(productId), GetCharacteristicIdTag(characteristicId)]);
    }

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> InsertAsync(ServiceProductPropertyByCharacteristicIdCreateRequest createRequest)
    {
        OneOf<Success, ValidationResult, UnexpectedFailureResult> result = await _productPropertyCrudService.InsertAsync(createRequest);

        if (result.IsT0)
        {
            //_cache.Evict(GetAllInProductKey(createRequest.ProductId));

            //_cache.Evict(GetCountOfAllInProductKey(createRequest.ProductId));

            //_cache.Evict(GetByProductAndCharacteristicIdKey(createRequest.ProductId, createRequest.ProductCharacteristicId));

            await _fusionCache.RemoveAsync(GetAllInProductKey(createRequest.ProductId));

            await _fusionCache.RemoveAsync(GetCountOfAllInProductKey(createRequest.ProductId));

            await _fusionCache.RemoveAsync(GetByProductAndCharacteristicIdKey(createRequest.ProductId, createRequest.ProductCharacteristicId));
        }

        return result;
    }

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpdateAsync(ProductPropertyUpdateRequest updateRequest)
    {
        OneOf<Success, ValidationResult, UnexpectedFailureResult> result = await _productPropertyCrudService.UpdateAsync(updateRequest);

        if (result.IsT0)
        {
            //_cache.Evict(GetAllInProductKey(updateRequest.ProductId));

            //_cache.Evict(GetCountOfAllInProductKey(updateRequest.ProductId));

            //_cache.Evict(GetByProductAndCharacteristicIdKey(updateRequest.ProductId, updateRequest.ProductCharacteristicId));

            await _fusionCache.RemoveAsync(GetAllInProductKey(updateRequest.ProductId));

            await _fusionCache.RemoveAsync(GetCountOfAllInProductKey(updateRequest.ProductId));

            await _fusionCache.RemoveAsync(GetByProductAndCharacteristicIdKey(updateRequest.ProductId, updateRequest.ProductCharacteristicId));
        }

        return result;
    }

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertAsync(ProductPropertyUpdateRequest updateRequest)
    {
        OneOf<Success, ValidationResult, UnexpectedFailureResult> result = await _productPropertyCrudService.UpsertAsync(updateRequest);

        if (result.IsT0)
        {
            //_cache.Evict(GetAllInProductKey(updateRequest.ProductId));

            //_cache.Evict(GetCountOfAllInProductKey(updateRequest.ProductId));

            //_cache.Evict(GetByProductAndCharacteristicIdKey(updateRequest.ProductId, updateRequest.ProductCharacteristicId));

            await _fusionCache.RemoveAsync(GetAllInProductKey(updateRequest.ProductId));

            await _fusionCache.RemoveAsync(GetCountOfAllInProductKey(updateRequest.ProductId));

            await _fusionCache.RemoveAsync(GetByProductAndCharacteristicIdKey(updateRequest.ProductId, updateRequest.ProductCharacteristicId));
        }

        return result;
    }

    public async Task<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>> ChangePropertyCharacteristicIdAsync(
        int productId,
        int oldCharacteristicId,
        int newCharacteristicId)
    {
        OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult> result
            = await _productPropertyCrudService.ChangePropertyCharacteristicIdAsync(productId, oldCharacteristicId, newCharacteristicId);

        if (result.IsT0)
        {
            //_cache.Evict(GetAllInProductKey(productId));

            //_cache.Evict(GetCountOfAllInProductKey(productId));

            //_cache.Evict(GetByProductAndCharacteristicIdKey(productId, oldCharacteristicId));

            //_cache.Evict(GetByProductAndCharacteristicIdKey(productId, newCharacteristicId));

            await _fusionCache.RemoveAsync(GetAllInProductKey(productId));

            await _fusionCache.RemoveAsync(GetCountOfAllInProductKey(productId));

            await _fusionCache.RemoveAsync(GetByProductAndCharacteristicIdKey(productId, oldCharacteristicId));

            await _fusionCache.RemoveAsync(GetByProductAndCharacteristicIdKey(productId, newCharacteristicId));
        }

        return result;
    }

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertAllProductPropertiesAsync(
        ProductPropertyUpsertAllForProductRequest productPropertyChangeAllForProductRequest)
    {
        OneOf<Success, ValidationResult, UnexpectedFailureResult> result
            = await _productPropertyCrudService.UpsertAllProductPropertiesAsync(productPropertyChangeAllForProductRequest);

        if (result.IsT0)
        {
            //InvalidateAllEntriesForProduct(productPropertyChangeAllForProductRequest.ProductId);

            await _fusionCache.RemoveByTagAsync(GetProductIdTag(productPropertyChangeAllForProductRequest.ProductId));
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int productId, int characteristicId)
    {
        bool result = await _productPropertyCrudService.DeleteAsync(productId, characteristicId);

        if (result)
        {
            //_cache.Evict(GetAllInProductKey(productId));

            //_cache.Evict(GetCountOfAllInProductKey(productId));

            //_cache.Evict(GetByProductAndCharacteristicIdKey(productId, characteristicId));

            await _fusionCache.RemoveAsync(GetAllInProductKey(productId));

            await _fusionCache.RemoveAsync(GetCountOfAllInProductKey(productId));

            await _fusionCache.RemoveAsync(GetByProductAndCharacteristicIdKey(productId, characteristicId));
        }

        return result;
    }

    public async Task<bool> DeleteAllForProductAsync(int productId)
    {
        bool result = await _productPropertyCrudService.DeleteAllForProductAsync(productId);

        if (result)
        {
            //InvalidateAllEntriesForProduct(productId);

            await _fusionCache.RemoveByTagAsync(GetProductIdTag(productId));
        }

        return result;
    }

    public async Task<bool> DeleteAllForCharacteristicAsync(int characteristicId)
    {
        bool result = await _productPropertyCrudService.DeleteAllForCharacteristicAsync(characteristicId);

        if (!result) return result;

        //List<int> propertyCachedProductIdTokens = _cacheEntriesByProductCancellationTokens.Keys.ToList();

        //foreach (int key in propertyCachedProductIdTokens)
        //{
        //    if (_cacheEntriesByProductCancellationTokens.TryRemove(key, out CancellationTokenSource? cts))
        //    {
        //        cts.Cancel();
        //        cts.Dispose();
        //    }
        //}

        await _fusionCache.RemoveByTagAsync(GetCharacteristicIdTag(characteristicId));

        return result;
    }
}