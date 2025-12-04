using FluentValidation.Results;
using OneOf;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductWorkStatuses;
using MOSTComputers.Services.ProductRegister.Services.ProductStatus.Contracts;
using MOSTComputers.Services.Caching.Models;
using ZiggyCreatures.Caching.Fusion;

using static MOSTComputers.Services.ProductRegister.Utils.Caching.CachingDefaults;
using static MOSTComputers.Services.ProductRegister.Utils.Caching.CacheKeyUtils.ForProductWorkStatuses;

namespace MOSTComputers.Services.ProductRegister.Services.ProductStatus.Cached;
internal sealed class CachedProductWorkStatusesService : IProductWorkStatusesService
{
    public CachedProductWorkStatusesService(
        IProductWorkStatusesService productWorkStatusesService,
        //IProductRepository productRepository,
        //ICache<string> cache,
        IFusionCache fusionCache)
    {
        _productWorkStatusesService = productWorkStatusesService;
        //_productRepository = productRepository;
        //_cache = cache;
        _fusionCache = fusionCache;
        //_emptyValuesCacheEntryOptions = new() { AbsoluteExpirationRelativeToNow = EmptyValuesCacheAbsoluteExpiration };
    }

    private readonly IProductWorkStatusesService _productWorkStatusesService;
    //private readonly IProductRepository _productRepository;
    //private readonly ICache<string> _cache;
    private readonly IFusionCache _fusionCache;

    //private readonly CustomCacheEntryOptions _emptyValuesCacheEntryOptions;

    private static string[] GetTagsForItem(ProductWorkStatuses productWorkStatuses)
    {
        return GetTagsForItem(productWorkStatuses.Id, productWorkStatuses.ProductId);
    }

    private static string[] GetTagsForItem(int id, int productId)
    {
        string idTag = GetIdTag(id);
        string productIdTag = GetProductIdTag(productId);

        return [idTag, productIdTag];
    }

    public async Task<List<ProductWorkStatuses>> GetAllAsync()
    {
        List<ProductWorkStatuses> allProductWorkStatuses = await _productWorkStatusesService.GetAllAsync();

        foreach (ProductWorkStatuses productWorkStatus in allProductWorkStatuses)
        {
            string getByIdKey = GetByIdKey(productWorkStatus.Id);
            string getByProductIdKey = GetByProductIdKey(productWorkStatus.ProductId);

            //CacheOption<ProductWorkStatuses> cacheOption = CacheOption<ProductWorkStatuses>.FromValue(productWorkStatus);

            //_cache.AddOrUpdate(getByIdKey, cacheOption);
            //_cache.AddOrUpdate(getByProductIdKey, cacheOption);

            await _fusionCache.SetAsync(getByIdKey, productWorkStatus, tags: GetTagsForItem(productWorkStatus));
            await _fusionCache.SetAsync(getByProductIdKey, productWorkStatus, tags: GetTagsForItem(productWorkStatus));
        }

        return allProductWorkStatuses;
    }

    public async Task<List<ProductWorkStatuses>> GetAllForProductsAsync(IEnumerable<int> productIds)
    {
        List<int> productIdsList = productIds.Distinct().ToList();

        List<ProductWorkStatuses> cachedProductWorkStatuses = new();

        for (int i = 0; i < productIdsList.Count; i++)
        {
            int productId = productIdsList[i];

            string getByProductIdKey = GetByProductIdKey(productId);

            //CacheOption<ProductWorkStatuses>? cachedProductWorkStatus
            //    = _cache.GetValueOrDefault<CacheOption<ProductWorkStatuses>>(getByProductIdKey);

            MaybeValue<ProductWorkStatuses?> cachedProductWorkStatus
                = await _fusionCache.TryGetAsync<ProductWorkStatuses?>(getByProductIdKey);

            if (!cachedProductWorkStatus.HasValue) continue;

            if (cachedProductWorkStatus.Value is not null)
            {
                cachedProductWorkStatuses.Add(cachedProductWorkStatus.Value);
            }

            productIdsList.RemoveAt(i);

            i--;
        }

        if (productIdsList.Count <= 0) return cachedProductWorkStatuses;

        List<ProductWorkStatuses> retrivedProductWorkStatuses = await _productWorkStatusesService.GetAllForProductsAsync(productIdsList);

        foreach (ProductWorkStatuses productWorkStatus in retrivedProductWorkStatuses)
        {
            string getByIdKey = GetByIdKey(productWorkStatus.Id);
            string getByProductIdKey = GetByProductIdKey(productWorkStatus.ProductId);

            //CacheOption<ProductWorkStatuses> cacheOption = CacheOption<ProductWorkStatuses>.FromValue(productWorkStatus);

            //_cache.AddOrUpdate(getByIdKey, cacheOption);
            //_cache.AddOrUpdate(getByProductIdKey, cacheOption);

            await _fusionCache.SetAsync(getByIdKey, productWorkStatus, tags: GetTagsForItem(productWorkStatus));
            await _fusionCache.SetAsync(getByProductIdKey, productWorkStatus, tags: GetTagsForItem(productWorkStatus));

            productIdsList.Remove(productWorkStatus.ProductId);
        }

        foreach (int productIdWithoutStatus in productIdsList)
        {
            string getByProductIdKey = GetByProductIdKey(productIdWithoutStatus);

            //_cache.AddOrUpdate(getByProductIdKey, CacheOption<ProductWorkStatuses>.Empty(), _emptyValuesCacheEntryOptions);

            await _fusionCache.SetAsync<ProductWorkStatuses?>(getByProductIdKey, null, options => options.SetDuration(EmptyValuesCacheAbsoluteExpiration));
        }

        retrivedProductWorkStatuses.AddRange(cachedProductWorkStatuses);

        return retrivedProductWorkStatuses;
    }

    public async Task<ProductWorkStatuses?> GetByIdAsync(int productWorkStatusesId)
    {
        string getByIdKey = GetByIdKey(productWorkStatusesId);

        //CacheOption<ProductWorkStatuses> retrievedProductWorkStatuses = await _cache.GetOrAddAsync<CacheOption<ProductWorkStatuses>>(
        //    getByIdKey,
        //    async (entry) =>
        //    {
        //        ProductWorkStatuses? underlyingData = await _productWorkStatusesService.GetByIdAsync(productWorkStatusesId);

        //        if (underlyingData is not null)
        //        {
        //            return CacheOption<ProductWorkStatuses>.FromValue(underlyingData);
        //        }

        //        entry.AbsoluteExpirationRelativeToNow = _emptyValuesCacheAbsoluteExpiration;

        //        return CacheOption<ProductWorkStatuses>.Empty();
        //    });

        ProductWorkStatuses? retrievedProductWorkStatuses = await _fusionCache.GetOrSetAsync<ProductWorkStatuses?>(
           getByIdKey,
           async (entry, cancellationToken) =>
           {
               ProductWorkStatuses? underlyingData = await _productWorkStatusesService.GetByIdAsync(productWorkStatusesId);

               if (underlyingData is not null)
               {
                   string[] tags = GetTagsForItem(underlyingData);

                   entry.Tags = tags;

                    string getByProductIdKey = GetByProductIdKey(underlyingData.ProductId);

                    await _fusionCache.SetAsync(getByProductIdKey, underlyingData, tags: tags, token: cancellationToken);

                    return underlyingData;
               }

               entry.Options.SetDuration(EmptyValuesCacheAbsoluteExpiration);

               return null;
           });

        return retrievedProductWorkStatuses;
    }

    public async Task<ProductWorkStatuses?> GetByProductIdAsync(int productId)
    {
        string getByProductIdKey = GetByProductIdKey(productId);

        //CacheOption<ProductWorkStatuses> retrievedProductWorkStatuses = await _cache.GetOrAddAsync(getByProductIdKey,
        //    async (entry) =>
        //    {
        //        ProductWorkStatuses? underlyingData = await _productWorkStatusesService.GetByProductIdAsync(productId);

        //        if (underlyingData is not null)
        //        {
        //            return CacheOption<ProductWorkStatuses>.FromValue(underlyingData);
        //        }

        //        entry.AbsoluteExpirationRelativeToNow = _emptyValuesCacheAbsoluteExpiration;

        //        return CacheOption<ProductWorkStatuses>.Empty();
        //    });

        ProductWorkStatuses? retrievedProductWorkStatuses = await _fusionCache.GetOrSetAsync<ProductWorkStatuses?>(getByProductIdKey,
            async (entry, cancellationToken) =>
            {
                ProductWorkStatuses? underlyingData = await _productWorkStatusesService.GetByProductIdAsync(productId);

                if (underlyingData is not null)
                {
                    string[] tags = GetTagsForItem(underlyingData);

                    entry.Tags = tags;

                    string getByIdKey = GetByIdKey(underlyingData.Id);

                    await _fusionCache.SetAsync(getByIdKey, underlyingData, tags: tags, token: cancellationToken);

                    return underlyingData;
                }

                entry.Options.SetDuration(EmptyValuesCacheAbsoluteExpiration);

                return null;
            });

        return retrievedProductWorkStatuses;
    }

    public async Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> InsertIfItDoesntExistAsync(ServiceProductWorkStatusesCreateRequest createRequest)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> createResult = await _productWorkStatusesService.InsertIfItDoesntExistAsync(createRequest);

        if (createResult.IsT0)
        {
            //_cache.Evict(GetByIdKey(createResult.AsT0));
            //_cache.Evict(GetByProductIdKey(createRequest.ProductId));

            await _fusionCache.RemoveAsync(GetByIdKey(createResult.AsT0));
            await _fusionCache.RemoveAsync(GetByProductIdKey(createRequest.ProductId));
        }

        return createResult;
    }

    public async Task<OneOf<bool, ValidationResult>> UpdateByIdAsync(ServiceProductWorkStatusesUpdateByIdRequest updateRequest)
    {
        OneOf<bool, ValidationResult> updateResult = await _productWorkStatusesService.UpdateByIdAsync(updateRequest);

        if (updateResult.IsT0 && updateResult.AsT0)
        {
            //_cache.Evict(GetByIdKey(updateRequest.Id));

            //ProductWorkStatuses? updatedProductWorkStatus = await GetByIdAsync(updateRequest.Id);

            //if (updatedProductWorkStatus is not null)
            //{
            //    _cache.Evict(GetByProductIdKey(updatedProductWorkStatus.ProductId));
            //}

            await _fusionCache.RemoveByTagAsync(GetIdTag(updateRequest.Id));
        }

        return updateResult;
    }

    public async Task<OneOf<bool, ValidationResult>> UpdateByProductIdAsync(ServiceProductWorkStatusesUpdateByProductIdRequest updateRequest)
    {
        OneOf<bool, ValidationResult> updateResult = await _productWorkStatusesService.UpdateByProductIdAsync(updateRequest);

        if (updateResult.IsT0 && updateResult.AsT0)
        {
            //_cache.Evict(GetByProductIdKey(updateRequest.ProductId));

            //ProductWorkStatuses? updatedProductWorkStatus = await GetByProductIdAsync(updateRequest.ProductId);

            //if (updatedProductWorkStatus is not null)
            //{
            //    _cache.Evict(GetByIdKey(updatedProductWorkStatus.Id));
            //}

            await _fusionCache.RemoveByTagAsync(GetProductIdTag(updateRequest.ProductId));
        }

        return updateResult;
    }

    public async Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> UpsertByProductIdAsync(ServiceProductWorkStatusesUpsertRequest upsertRequest)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> upsertResult = await _productWorkStatusesService.UpsertByProductIdAsync(upsertRequest);

        if (upsertResult.IsT0)
        {
            //_cache.Evict(GetByIdKey(upsertResult.AsT0));
            //_cache.Evict(GetByProductIdKey(upsertRequest.ProductId));

            await _fusionCache.RemoveAsync(GetByIdKey(upsertResult.AsT0));
            await _fusionCache.RemoveAsync(GetByProductIdKey(upsertRequest.ProductId));
        }

        return upsertResult;
    }

    public async Task<bool> DeleteAllAsync()
    {
        List<ProductWorkStatuses> productWorkStatuses = await GetAllAsync();

        bool areAllDeleted = await _productWorkStatusesService.DeleteAllAsync();

        if (!areAllDeleted) return areAllDeleted;

        foreach (ProductWorkStatuses productWorkStatus in productWorkStatuses)
        {
            //_cache.Evict(GetByIdKey(productWorkStatus.Id));
            //_cache.Evict(GetByProductIdKey(productWorkStatus.ProductId));

            await _fusionCache.RemoveAsync(GetByIdKey(productWorkStatus.Id));
            await _fusionCache.RemoveAsync(GetByProductIdKey(productWorkStatus.ProductId));
        }

        return true;
    }

    public async Task<bool> DeleteByIdAsync(int productWorkStatusesId)
    {
        //ProductWorkStatuses? productWorkStatus = await GetByIdAsync(productWorkStatusesId);

        //if (productWorkStatus is null) return false;

        bool isDeleted = await _productWorkStatusesService.DeleteByIdAsync(productWorkStatusesId);

        if (!isDeleted) return false;

        //_cache.Evict(GetByIdKey(productWorkStatusesId));
        //_cache.Evict(GetByProductIdKey(productWorkStatus.ProductId));

        await _fusionCache.RemoveByTagAsync(GetIdTag(productWorkStatusesId));

        return true;
    }

    public async Task<bool> DeleteByProductIdAsync(int productId)
    {
        //ProductWorkStatuses? productWorkStatus = await GetByProductIdAsync(productId);

        //if (productWorkStatus is null) return false;

        bool isDeleted = await _productWorkStatusesService.DeleteByProductIdAsync(productId);

        if (!isDeleted) return false;

        //_cache.Evict(GetByIdKey(productWorkStatus.Id));
        //_cache.Evict(GetByProductIdKey(productId));

        await _fusionCache.RemoveByTagAsync(GetProductIdTag(productId));

        return true;
    }
}