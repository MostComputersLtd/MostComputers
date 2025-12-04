using FluentValidation.Results;
using OneOf;
using OneOf.Types;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.Caching.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionProductFileInfo.Internal;

using static MOSTComputers.Services.ProductRegister.Utils.Caching.CachingDefaults;
using static MOSTComputers.Services.ProductRegister.Utils.Caching.CacheKeyUtils.ForPromotionProductFile;
using MOSTComputers.Models.Product.Models.Promotions.Files;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.Promotions.PromotionProductFileInfos;
using MOSTComputers.Services.ProductRegister.Services.Promotions.PromotionFiles.Contracts;
using ZiggyCreatures.Caching.Fusion;

namespace MOSTComputers.Services.ProductRegister.Services.Promotions.PromotionFiles.Cached;
internal sealed class CachedPromotionProductFileInfoService : IPromotionProductFileInfoService
{
    public CachedPromotionProductFileInfoService(
        IPromotionProductFileInfoService promotionProductFileInfoService,
        //ICache<string> cache,
        IFusionCache fusionCache)
    {
        _promotionProductFileInfoService = promotionProductFileInfoService;
        //_cache = cache;
        _fusionCache = fusionCache;
    }
    
    private readonly IPromotionProductFileInfoService _promotionProductFileInfoService;
    //private readonly ICache<string> _cache;
    private readonly IFusionCache _fusionCache;

    public async Task<List<IGrouping<int, PromotionProductFileInfo>>> GetAllForProductsAsync(IEnumerable<int> productIds)
    {
        List<int> productIdsList = productIds.Distinct().ToList();

        List<PromotionProductFileInfo> cachedPromotionProductFiles = new();

        for (int i = 0; i < productIdsList.Count; i++)
        {
            int productId = productIdsList[i];

            string promotionProductFilesForProductKey = GetAllByProductIdKey(productId);

            //List<PromotionProductFileInfo>? cachedPromotionProductFilesForProduct
            //    = _cache.GetValueOrDefault<List<PromotionProductFileInfo>>(promotionProductFilesForProductKey);

            MaybeValue<List<PromotionProductFileInfo>> cachedPromotionProductFilesForProduct
                = await _fusionCache.TryGetAsync<List<PromotionProductFileInfo>>(promotionProductFilesForProductKey);

            if (!cachedPromotionProductFilesForProduct.HasValue) continue;

            cachedPromotionProductFiles.AddRange(cachedPromotionProductFilesForProduct.Value);

            productIdsList.RemoveAt(i);

            i--;
        }

        if (productIdsList.Count <= 0)
        {
            return cachedPromotionProductFiles
                .GroupBy(promotionProductFile => promotionProductFile.ProductId)
                .ToList();
        }

        List<IGrouping<int, PromotionProductFileInfo>> retrievedPromotionProductFiles
            = await _promotionProductFileInfoService.GetAllForProductsAsync(productIds);

        foreach (IGrouping<int, PromotionProductFileInfo> promotionProductFilesForProduct in retrievedPromotionProductFiles)
        {
            string getAllForProductKey = GetAllByProductIdKey(promotionProductFilesForProduct.Key);

            //_cache.AddOrUpdate(getAllForProductKey, promotionProductFilesForProduct.ToList());

            await _fusionCache.SetAsync(getAllForProductKey, promotionProductFilesForProduct.ToList());

            foreach (PromotionProductFileInfo promotionProductFile in promotionProductFilesForProduct)
            {
                string getByIdKey = GetByIdKey(promotionProductFile.Id);

                //_cache.AddOrUpdate(getByIdKey, promotionProductFile);

                await _fusionCache.SetAsync(getByIdKey, promotionProductFile);
            }

            productIdsList.Remove(promotionProductFilesForProduct.Key);
        }

        foreach (int productIdWithoutPromotionProductFiles in productIdsList)
        {
            List<PromotionProductFileInfo> promotionProductFileInfos = new();

            string getAllForProductKey = GetAllByProductIdKey(productIdWithoutPromotionProductFiles);

            //_cache.AddOrUpdate(getAllForProductKey, promotionProductFileInfos);

            await _fusionCache.SetAsync(getAllForProductKey, promotionProductFileInfos);
        }

        return retrievedPromotionProductFiles
            .SelectMany(x => x)
            .Concat(cachedPromotionProductFiles)
            .GroupBy(property => property.ProductId)
            .ToList();
    }

    public async Task<List<PromotionProductFileInfoForProductCountData>> GetCountOfAllForProductsAsync(IEnumerable<int> productIds)
    {
        List<int> productIdsList = productIds.Distinct().ToList();

        List<PromotionProductFileInfoForProductCountData> cachedPromotionProductFileCounts = new();

        for (int i = 0; i < productIdsList.Count; i++)
        {
            int productId = productIdsList[i];

            string countOfPromotionProductFilesForProductKey = GetCountOfAllByProductIdKey(productId);

            //int? countOfPromotionProductFilesForProduct = _cache.GetValueOrDefault<int?>(countOfPromotionProductFilesForProductKey);

            MaybeValue<int> countOfPromotionProductFilesForProduct = await _fusionCache.TryGetAsync<int>(countOfPromotionProductFilesForProductKey);

            if (!countOfPromotionProductFilesForProduct.HasValue) continue;

            PromotionProductFileInfoForProductCountData productPromotionProductFilesCountData = new()
            {
                ProductId = productId,
                Count = countOfPromotionProductFilesForProduct.Value,
            };

            cachedPromotionProductFileCounts.Add(productPromotionProductFilesCountData);

            productIdsList.RemoveAt(i);

            i--;
        }

        if (productIdsList.Count <= 0)
        {
            return cachedPromotionProductFileCounts;
        }

        List<PromotionProductFileInfoForProductCountData> retrievedCounts = await _promotionProductFileInfoService.GetCountOfAllForProductsAsync(productIdsList);

        foreach (PromotionProductFileInfoForProductCountData promotionProductFileCountData in retrievedCounts)
        {
            string getCountForProductKey = GetCountOfAllByProductIdKey(promotionProductFileCountData.ProductId);

            //_cache.AddOrUpdate(getCountForProductKey, promotionProductFileCountData.Count);

            await _fusionCache.SetAsync(getCountForProductKey, promotionProductFileCountData.Count);

            productIdsList.Remove(promotionProductFileCountData.ProductId);
        }

        foreach (int productIdWithoutPromotionProductFiles in productIdsList)
        {
            string getCountForProductKey = GetCountOfAllByProductIdKey(productIdWithoutPromotionProductFiles);

            //_cache.AddOrUpdate(getCountForProductKey, 0);

            await _fusionCache.SetAsync(getCountForProductKey, 0);
        }

        retrievedCounts.AddRange(cachedPromotionProductFileCounts);

        return retrievedCounts;
    }

    public async Task<List<PromotionProductFileInfo>> GetAllForProductAsync(int productId)
    {
        string cacheKey = GetAllByProductIdKey(productId);

        //List<PromotionProductFileInfo> promotionProductFiles = await _cache.GetOrAddAsync(cacheKey, async () =>
        //{
        //    List<PromotionProductFileInfo> data = await _promotionProductFileInfoService.GetAllForProductAsync(productId);

        //    foreach (PromotionProductFileInfo promotionProductFile in data)
        //    {
        //        string getByIdKey = GetByIdKey(promotionProductFile.Id);

        //        _cache.AddOrUpdate(getByIdKey, promotionProductFile);
        //    }

        //    return data;
        //});

        List<PromotionProductFileInfo> promotionProductFiles = await _fusionCache.GetOrSetAsync<List<PromotionProductFileInfo>>(
            cacheKey, async (entry, cancellationToken) =>
            {
                List<PromotionProductFileInfo> data = await _promotionProductFileInfoService.GetAllForProductAsync(productId);

                foreach (PromotionProductFileInfo promotionProductFile in data)
                {
                    string getByIdKey = GetByIdKey(promotionProductFile.Id);

                    await _fusionCache.SetAsync(getByIdKey, promotionProductFile, token: cancellationToken);
                }

                return data;
            });

        return promotionProductFiles.ToList();
    }

    public async Task<int> GetCountOfAllForProductAsync(int productId)
    {
        string cacheKey = GetCountOfAllByProductIdKey(productId);

        //return await _cache.GetOrAddAsync(cacheKey, () => _promotionProductFileInfoService.GetCountOfAllForProductAsync(productId));

        return await _fusionCache.GetOrSetAsync(cacheKey, (_) => _promotionProductFileInfoService.GetCountOfAllForProductAsync(productId));
    }

    public async Task<bool> DoesExistForPromotionFileAsync(int promotionFileId)
    {
        return await _promotionProductFileInfoService.DoesExistForPromotionFileAsync(promotionFileId);
    }

    public async Task<PromotionProductFileInfo?> GetByIdAsync(int id)
    {
        string cacheKey = GetByIdKey(id);

        //return await _cache.GetOrAddAsync(cacheKey, async (entry) =>
        //{
        //    PromotionProductFileInfo? data = await _promotionProductFileInfoService.GetByIdAsync(id);

        //    if (data is null)
        //    {
        //        entry.AbsoluteExpirationRelativeToNow = EmptyValuesCacheAbsoluteExpiration;

        //        return null;
        //    }

        //    return data;
        //});

        return await _fusionCache.GetOrSetAsync<PromotionProductFileInfo?>(cacheKey, async (entry, _) =>
        {
            PromotionProductFileInfo? data = await _promotionProductFileInfoService.GetByIdAsync(id);

            if (data is null)
            {
                entry.Options.SetDuration(EmptyValuesCacheAbsoluteExpiration);

                return null;
            }

            return data;
        });
    }

    public async Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> InsertAsync(ServicePromotionProductFileInfoCreateRequest createRequest)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult = await _promotionProductFileInfoService.InsertAsync(createRequest);

        if (insertResult.IsT0)
        {
            //_cache.Evict(GetAllByProductIdKey(createRequest.ProductId));

            //_cache.Evict(GetCountOfAllByProductIdKey(createRequest.ProductId));

            //_cache.Evict(GetByIdKey(insertResult.AsT0));

            await _fusionCache.RemoveAsync(GetAllByProductIdKey(createRequest.ProductId));

            await _fusionCache.RemoveAsync(GetCountOfAllByProductIdKey(createRequest.ProductId));

            await _fusionCache.RemoveAsync(GetByIdKey(insertResult.AsT0));
        }

        return insertResult;
    }

    public async Task<OneOf<Success, NotFound, ValidationResult>> UpdateAsync(ServicePromotionProductFileInfoUpdateRequest updateRequest)
    {
        PromotionProductFileInfo? oldPromotionProductFile = await GetByIdAsync(updateRequest.Id);

        if (oldPromotionProductFile is null) return new NotFound();

        OneOf<Success, NotFound, ValidationResult> updateResult = await _promotionProductFileInfoService.UpdateAsync(updateRequest);

        if (updateResult.IsT0)
        {
            //_cache.Evict(GetByIdKey(updateRequest.Id));

            //_cache.Evict(GetAllByProductIdKey(oldPromotionProductFile!.ProductId));

            //_cache.Evict(GetCountOfAllByProductIdKey(oldPromotionProductFile!.ProductId));

            await _fusionCache.RemoveAsync(GetByIdKey(updateRequest.Id));

            await _fusionCache.RemoveAsync(GetAllByProductIdKey(oldPromotionProductFile!.ProductId));

            await _fusionCache.RemoveAsync(GetCountOfAllByProductIdKey(oldPromotionProductFile!.ProductId));
        }

        return updateResult;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        PromotionProductFileInfo? promotionProductFile = await GetByIdAsync(id);

        if (promotionProductFile is null) return false;

        bool isDeleted = await _promotionProductFileInfoService.DeleteAsync(id);

        if (!isDeleted) return false;

        //_cache.Evict(GetAllByProductIdKey(promotionProductFile.ProductId));

        //_cache.Evict(GetCountOfAllByProductIdKey(promotionProductFile.ProductId));

        //_cache.Evict(GetByIdKey(id));

        await _fusionCache.RemoveAsync(GetAllByProductIdKey(promotionProductFile.ProductId));

        await _fusionCache.RemoveAsync(GetCountOfAllByProductIdKey(promotionProductFile.ProductId));

        await _fusionCache.RemoveAsync(GetByIdKey(id));

        return isDeleted;
    }
}