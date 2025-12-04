using FluentValidation.Results;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models.Promotions.Files;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.Caching.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionFile;
using MOSTComputers.Services.ProductRegister.Models.Responses;
using MOSTComputers.Services.ProductRegister.Services.Promotions.PromotionFiles.Contracts;
using OneOf;
using OneOf.Types;
using ZiggyCreatures.Caching.Fusion;

using static MOSTComputers.Services.ProductRegister.Utils.Caching.CachingDefaults;
using static MOSTComputers.Services.ProductRegister.Utils.Caching.CacheKeyUtils.ForPromotionFile;

namespace MOSTComputers.Services.ProductRegister.Services.Promotions.PromotionFiles.Cached;
internal sealed class CachedPromotionFileService : IPromotionFileService
{
    public CachedPromotionFileService(
        IPromotionFileService promotionFileService,
        //ICache<string> cache,
        IFusionCache fusionCache)
    {
        _promotionFileService = promotionFileService;
        //_cache = cache;
        _fusionCache = fusionCache;
    }

    private readonly IPromotionFileService _promotionFileService;
    //private readonly ICache<string> _cache;
    private readonly IFusionCache _fusionCache;

    public async Task<List<PromotionFileInfo>> GetAllAsync()
    {
        string cacheKey = GetAllKey();

        //List<PromotionFileInfo> promotionFileInfos = await _cache.GetOrAddAsync(cacheKey, async () =>
        //{
        //    List<PromotionFileInfo> data = await _promotionFileService.GetAllAsync();

        //    IEnumerable<IGrouping<bool, PromotionFileInfo>> dataGroupedByActivity = data.GroupBy(x => x.Active);

        //    foreach (IGrouping<bool, PromotionFileInfo> group in dataGroupedByActivity)
        //    {
        //        _cache.AddOrUpdate(GetAllByActivityKey(group.Key), group.ToList());

        //        foreach (PromotionFileInfo item in group)
        //        {
        //            _cache.AddOrUpdate(GetByIdKey(item.Id), item);
        //        }
        //    }

        //    return data;
        //});

        List<PromotionFileInfo> promotionFileInfos = await _fusionCache.GetOrSetAsync(cacheKey, async (cancellationToken) =>
        {
            List<PromotionFileInfo> data = await _promotionFileService.GetAllAsync();

            IEnumerable<IGrouping<bool, PromotionFileInfo>> dataGroupedByActivity = data.GroupBy(x => x.Active);

            foreach (IGrouping<bool, PromotionFileInfo> group in dataGroupedByActivity)
            {
                await _fusionCache.SetAsync(GetAllByActivityKey(group.Key), group.ToList(), token: cancellationToken);

                foreach (PromotionFileInfo item in group)
                {
                    await _fusionCache.SetAsync(GetByIdKey(item.Id), item, token: cancellationToken);
                }
            }

            return data;
        });

        return promotionFileInfos.ToList();
    }

    public async Task<List<PromotionFileInfo>> GetAllByActivityAsync(bool active = true)
    {
        string cacheKey = GetAllByActivityKey(active);

        //List<PromotionFileInfo> data = await _cache.GetOrAddAsync(cacheKey, () => _promotionFileService.GetAllByActivityAsync(active));

        List<PromotionFileInfo> data = await _fusionCache.GetOrSetAsync(cacheKey, async (cancellationToken) =>
        {
            List<PromotionFileInfo> data = await _promotionFileService.GetAllByActivityAsync(active);

            foreach (PromotionFileInfo item in data)
            {
                await _fusionCache.SetAsync(GetByIdKey(item.Id), item, token: cancellationToken);
            }

            return data;
        });

        return data.ToList();
    }

    public async Task<List<PromotionFileInfo>> GetByIdsAsync(IEnumerable<int> ids)
    {
        List<int> idsList = ids.Distinct().ToList();

        List<PromotionFileInfo> cachedPromotionFiles = new();

        for (int i = 0; i < idsList.Count; i++)
        {
            int id = idsList[i];

            string getByIdKey = GetByIdKey(id);

            //PromotionFileInfo? cachedPromotionFile = _cache.GetValueOrDefault<PromotionFileInfo?>(getByIdKey);

            MaybeValue<PromotionFileInfo?> cachedPromotionFile = await _fusionCache.TryGetAsync<PromotionFileInfo?>(getByIdKey);

            if (!cachedPromotionFile.HasValue || cachedPromotionFile.Value is null) continue;

            cachedPromotionFiles.Add(cachedPromotionFile.Value);

            idsList.RemoveAt(i);

            i--;
        }

        if (idsList.Count <= 0) return cachedPromotionFiles;

        List<PromotionFileInfo> promotionFiles = await _promotionFileService.GetByIdsAsync(idsList);

        foreach (PromotionFileInfo promotionFile in promotionFiles)
        {
            string getByIdKey = GetByIdKey(promotionFile.Id);

            //_cache.AddOrUpdate(getByIdKey, promotionFile);
            await _fusionCache.SetAsync(getByIdKey, promotionFile);
        }

        promotionFiles.AddRange(cachedPromotionFiles);

        return promotionFiles;
    }

    public async Task<PromotionFileInfo?> GetByIdAsync(int id)
    {
        string cacheKey = GetByIdKey(id);

        //return await _cache.GetOrAddAsync(cacheKey, async (entry) =>
        //{
        //    PromotionFileInfo? data = await _promotionFileService.GetByIdAsync(id);

        //    if (data is null)
        //    {
        //        entry.AbsoluteExpirationRelativeToNow = EmptyValuesCacheAbsoluteExpiration;

        //        return data;
        //    }

        //    return data;
        //});

        return await _fusionCache.GetOrSetAsync<PromotionFileInfo?>(cacheKey, async (entry, _) =>
        {
            PromotionFileInfo? data = await _promotionFileService.GetByIdAsync(id);

            if (data is null)
            {
                entry.Options.SetDuration(EmptyValuesCacheAbsoluteExpiration);

                return data;
            }

            return data;
        });
    }

    public async Task<Stream?> GetFileDataByIdAsync(int fileInfoId)
    {
        return await _promotionFileService.GetFileDataByIdAsync(fileInfoId);
    }

    public async Task<OneOf<int, ValidationResult, FileAlreadyExistsResult, UnexpectedFailureResult>> InsertAsync(CreatePromotionFileRequest createRequest)
    {
        OneOf<int, ValidationResult, FileAlreadyExistsResult, UnexpectedFailureResult> result = await _promotionFileService.InsertAsync(createRequest);

        if (result.IsT0)
        {
            //_cache.Evict(GetAllKey());

            //_cache.Evict(GetAllByActivityKey(createRequest.Active));

            //_cache.Evict(GetByIdKey(result.AsT0));

            await _fusionCache.RemoveAsync(GetAllKey());

            await _fusionCache.RemoveAsync(GetAllByActivityKey(createRequest.Active));

            await _fusionCache.RemoveAsync(GetByIdKey(result.AsT0));
        }
        
        return result;
    }

    public async Task<OneOf<Success, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> UpdateAsync(UpdatePromotionFileRequest updateRequest)
    {
        OneOf<Success, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> result =
            await _promotionFileService.UpdateAsync(updateRequest);

        if (result.IsT0)
        {
            //_cache.Evict(GetAllKey());

            //_cache.Evict(GetAllByActivityKey(updateRequest.Active));

            //_cache.Evict(GetByIdKey(updateRequest.Id));

            await _fusionCache.RemoveAsync(GetAllKey());

            await _fusionCache.RemoveAsync(GetAllByActivityKey(updateRequest.Active));

            await _fusionCache.RemoveAsync(GetByIdKey(updateRequest.Id));
        }

        return result;
    }

    public async Task<OneOf<Success, NotFound, PromotionFileHasRelationsResult, FileDoesntExistResult>> DeleteAsync(int promotionFileInfoId)
    {
        PromotionFileInfo? oldPromotionFileInfo = await GetByIdAsync(promotionFileInfoId);

        if (oldPromotionFileInfo is null) return new NotFound();

        OneOf<Success, NotFound, PromotionFileHasRelationsResult, FileDoesntExistResult> result =
            await _promotionFileService.DeleteAsync(promotionFileInfoId);

        if (result.IsT0)
        {
            //_cache.Evict(GetAllKey());

            //_cache.Evict(GetAllByActivityKey(oldPromotionFileInfo.Active));

            //_cache.Evict(GetByIdKey(promotionFileInfoId));

            await _fusionCache.RemoveAsync(GetAllKey());

            await _fusionCache.RemoveAsync(GetAllByActivityKey(oldPromotionFileInfo.Active));

            await _fusionCache.RemoveAsync(GetByIdKey(promotionFileInfoId));
        }

        return result;
    }
}