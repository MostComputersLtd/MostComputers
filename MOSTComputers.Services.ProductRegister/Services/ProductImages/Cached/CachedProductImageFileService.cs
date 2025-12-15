using FluentValidation.Results;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileData;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using OneOf;
using OneOf.Types;
using ZiggyCreatures.Caching.Fusion;
using static MOSTComputers.Services.ProductRegister.Utils.Caching.CacheKeyUtils.ForProductImageFile;
using static MOSTComputers.Services.ProductRegister.Utils.Caching.CachingDefaults;

namespace MOSTComputers.Services.ProductRegister.Services.ProductImages.Cached;
internal sealed class CachedProductImageFileService : IProductImageFileService
{
    public CachedProductImageFileService(
        IProductImageFileService productImageFileService,
        //ICache<string> cache,
        IFusionCache fusionCache)
    {
        _productImageFileService = productImageFileService;
        //_cache = cache;
        _fusionCache = fusionCache;
    }

    private const string _fileDoesNotExistErrorMessage = "File does not exist";

    private readonly IProductImageFileService _productImageFileService;
    //private readonly ICache<string> _cache;
    private readonly IFusionCache _fusionCache;

    public async Task<List<ProductImageFileData>> GetAllAsync()
    {
        List<ProductImageFileData> retrievedProductImageFiles = await _productImageFileService.GetAllAsync();

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

        List<IGrouping<int, ProductImageFileData>> retrievedProperties = await _productImageFileService.GetAllInProductsAsync(productIdsListMissingInCache);

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
                List<ProductImageFileData> data = await _productImageFileService.GetAllInProductAsync(productId);

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
            ProductImageFileData? data = await _productImageFileService.GetByIdAsync(id);

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
            ProductImageFileData? data = await _productImageFileService.GetByProductIdAndImageIdAsync(productId, imageId);

            if (data is null) return null;

            string getByIdKey = GetByIdKey(data.Id);

            await _fusionCache.SetAsync(getByIdKey, data, token: cancellationToken);

            return data;
        });

        return retrievedProductImageFile;
    }

    public async Task<OneOf<int, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult>> InsertFileAsync(ProductImageFileCreateRequest createRequest)
    {
        OneOf<int, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult> insertResult
            = await _productImageFileService.InsertFileAsync(createRequest);

        if (insertResult.IsT0)
        {
            //_cache.Evict(GetAllByProductIdKey(createRequest.ProductId));
            //_cache.Evict(GetByIdKey(insertResult.AsT0));

            //if (createRequest.ImageId is not null)
            //{
            //    _cache.Evict(GetByProductIdAndImageIdKey(createRequest.ProductId, createRequest.ImageId.Value));
            //}

            await _fusionCache.RemoveAsync(GetAllByProductIdKey(createRequest.ProductId));
            await _fusionCache.RemoveAsync(GetByIdKey(insertResult.AsT0));

            if (createRequest.ImageId is not null)
            {
                await _fusionCache.RemoveAsync(GetByProductIdAndImageIdKey(createRequest.ProductId, createRequest.ImageId.Value));
            }
        }

        return insertResult;
    }

    public async Task<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> UpdateFileAsync(
        ProductImageFileUpdateRequest updateRequest)
    {
        ProductImageFileData? oldProductImageFile = await GetByIdAsync(updateRequest.Id);

        if (oldProductImageFile is null)
        {
            ValidationFailure validationFailure = new(nameof(ProductImageFileUpdateRequest.Id), _fileDoesNotExistErrorMessage);

            return new ValidationResult([validationFailure]);
        }

        OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> updateResult
            = await _productImageFileService.UpdateFileAsync(updateRequest);
        
        if (updateResult.IsT0)
        {
            //_cache.Evict(GetByIdKey(updateRequest.Id));

            //_cache.Evict(GetAllByProductIdKey(oldProductImageFile.ProductId));

            //if (oldProductImageFile.ImageId is not null)
            //{
            //    _cache.Evict(GetByProductIdAndImageIdKey(oldProductImageFile.ProductId, oldProductImageFile.ImageId.Value));
            //}

            //if (updateRequest.UpdateImageIdRequest.IsT0)
            //{
            //    _cache.Evict(GetByProductIdAndImageIdKey(oldProductImageFile.ProductId, updateRequest.UpdateImageIdRequest.AsT0!.Value));
            //}

            await _fusionCache.RemoveAsync(GetByIdKey(updateRequest.Id));

            await _fusionCache.RemoveAsync(GetAllByProductIdKey(oldProductImageFile.ProductId));

            if (oldProductImageFile.ImageId is not null)
            {
                await _fusionCache.RemoveAsync(GetByProductIdAndImageIdKey(oldProductImageFile.ProductId, oldProductImageFile.ImageId.Value));
            }

            if (updateRequest.UpdateImageIdRequest.IsT0)
            {
                await _fusionCache.RemoveAsync(GetByProductIdAndImageIdKey(oldProductImageFile.ProductId, updateRequest.UpdateImageIdRequest.AsT0!.Value));
            }
        }

        return updateResult;
    }

    public async Task<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> ChangeFileAsync(
        ProductImageFileChangeRequest changeFileRequest)
    {
        ProductImageFileData? oldProductImageFile = await GetByIdAsync(changeFileRequest.Id);

        if (oldProductImageFile is null)
        {
            ValidationFailure validationFailure = new(nameof(ProductImageFileUpdateRequest.Id), _fileDoesNotExistErrorMessage);

            return new ValidationResult([validationFailure]);
        }

        OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> changeFileResult
            = await _productImageFileService.ChangeFileAsync(changeFileRequest);

        if (changeFileResult.IsT0)
        {
            //_cache.Evict(GetByIdKey(changeFileRequest.Id));

            //_cache.Evict(GetAllByProductIdKey(oldProductImageFile.ProductId));

            //if (oldProductImageFile.ImageId is not null)
            //{
            //    _cache.Evict(GetByProductIdAndImageIdKey(oldProductImageFile.ProductId, oldProductImageFile.ImageId.Value));
            //}

            await _fusionCache.RemoveAsync(GetByIdKey(changeFileRequest.Id));

            await _fusionCache.RemoveAsync(GetAllByProductIdKey(oldProductImageFile.ProductId));

            if (oldProductImageFile.ImageId is not null)
            {
                await _fusionCache.RemoveAsync(GetByProductIdAndImageIdKey(oldProductImageFile.ProductId, oldProductImageFile.ImageId.Value));
            }
        }

        return changeFileResult;
    }

    public async Task<OneOf<Success, ValidationResult, NotFound, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> RenameFileAsync(
        ProductImageFileRenameRequest renameRequest)
    {
        ProductImageFileData? oldProductImageFile = await GetByIdAsync(renameRequest.Id);

        if (oldProductImageFile is null)
        {
            ValidationFailure validationFailure = new(nameof(ProductImageFileUpdateRequest.Id), _fileDoesNotExistErrorMessage);

            return new ValidationResult([validationFailure]);
        }

        OneOf<Success, ValidationResult, NotFound, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> renameFileResult
            = await _productImageFileService.RenameFileAsync(renameRequest);

        if (renameFileResult.IsT0)
        {
            //_cache.Evict(GetByIdKey(renameRequest.Id));

            //if (oldProductImageFile is not null)
            //{
            //    _cache.Evict(GetAllByProductIdKey(oldProductImageFile.ProductId));

            //    if (oldProductImageFile.ImageId is not null)
            //    {
            //        _cache.Evict(GetByProductIdAndImageIdKey(oldProductImageFile.ProductId, oldProductImageFile.ImageId.Value));
            //    }
            //}

            await _fusionCache.RemoveAsync(GetByIdKey(renameRequest.Id));

            if (oldProductImageFile is not null)
            {
                await _fusionCache.RemoveAsync(GetAllByProductIdKey(oldProductImageFile.ProductId));

                if (oldProductImageFile.ImageId is not null)
                {
                    await _fusionCache.RemoveAsync(GetByProductIdAndImageIdKey(oldProductImageFile.ProductId, oldProductImageFile.ImageId.Value));
                }
            }
        }

        return renameFileResult;
    }

    public async Task<OneOf<Success, NotFound, FileDoesntExistResult, ValidationResult, UnexpectedFailureResult>> DeleteFileAsync(
        int id, string deleteUserName)
    {
        ProductImageFileData? productImageFile = await GetByIdAsync(id);

        if (productImageFile is null) return new NotFound();

        OneOf<Success, NotFound, FileDoesntExistResult, ValidationResult, UnexpectedFailureResult> deleteResult
            = await _productImageFileService.DeleteFileAsync(id, deleteUserName);

        if (deleteResult.IsT0)
        {
            //_cache.Evict(GetAllByProductIdKey(productImageFile.ProductId));

            //_cache.Evict(GetByIdKey(productImageFile.Id));

            //if (productImageFile.ImageId is not null)
            //{
            //    _cache.Evict(GetByProductIdAndImageIdKey(productImageFile.ProductId, productImageFile.ImageId.Value));
            //}

            await _fusionCache.RemoveAsync(GetAllByProductIdKey(productImageFile.ProductId));

            await _fusionCache.RemoveAsync(GetByIdKey(productImageFile.Id));

            if (productImageFile.ImageId is not null)
            {
                await _fusionCache.RemoveAsync(GetByProductIdAndImageIdKey(productImageFile.ProductId, productImageFile.ImageId.Value));
            }
        }

        return deleteResult;
    }

    public async Task<OneOf<Success, NotFound, FileDoesntExistResult, ValidationResult, UnexpectedFailureResult>> DeleteFileAsync(
        ProductImageFileData fileNameInfo, string deleteUserName)
    {
        ProductImageFileData? realProductImageFile = await GetByIdAsync(fileNameInfo.Id);

        if (realProductImageFile is null) return new NotFound();

        OneOf<Success, NotFound, FileDoesntExistResult, ValidationResult, UnexpectedFailureResult> deleteResult
            = await _productImageFileService.DeleteFileAsync(fileNameInfo, deleteUserName);

        if (deleteResult.IsT0)
        {
            //_cache.Evict(GetAllByProductIdKey(realProductImageFile.ProductId));

            //_cache.Evict(GetByIdKey(realProductImageFile.Id));

            //if (realProductImageFile.ImageId is not null)
            //{
            //    _cache.Evict(GetByProductIdAndImageIdKey(realProductImageFile.ProductId, realProductImageFile.ImageId.Value));
            //}

            await _fusionCache.RemoveAsync(GetAllByProductIdKey(realProductImageFile.ProductId));

            await _fusionCache.RemoveAsync(GetByIdKey(realProductImageFile.Id));

            if (realProductImageFile.ImageId is not null)
            {
                await _fusionCache.RemoveAsync(GetByProductIdAndImageIdKey(realProductImageFile.ProductId, realProductImageFile.ImageId.Value));
            }
        }

        return deleteResult;
    }

    public async Task<OneOf<Success, NotFound, FileDoesntExistResult, ValidationResult, UnexpectedFailureResult>> DeleteFileByProductIdAndImageIdAsync(
        int productId, int imageId, string deleteUserName)
    {
        ProductImageFileData? productImageFile = await GetByProductIdAndImageIdAsync(productId, imageId);

        if (productImageFile is null) return new NotFound();

        OneOf<Success, NotFound, FileDoesntExistResult, ValidationResult, UnexpectedFailureResult> deleteResult
            = await _productImageFileService.DeleteFileByProductIdAndImageIdAsync(productId, imageId, deleteUserName);

        if (deleteResult.IsT0)
        {
            //_cache.Evict(GetAllByProductIdKey(productId));

            //_cache.Evict(GetByIdKey(productImageFile.Id));

            //_cache.Evict(GetByProductIdAndImageIdKey(productId, imageId));

            await _fusionCache.RemoveAsync(GetAllByProductIdKey(productId));

            await _fusionCache.RemoveAsync(GetByIdKey(productImageFile.Id));

            await _fusionCache.RemoveAsync(GetByProductIdAndImageIdKey(productId, imageId));
        }

        return deleteResult;
    }

    public async Task<OneOf<Success, NotFound, FileDoesntExistResult, ValidationResult, UnexpectedFailureResult>> DeleteAllFilesForProductAsync(
        int productId, string deleteUserName)
    {
        List<ProductImageFileData> productImageFilesForProduct = await GetAllInProductAsync(productId);

        OneOf<Success, NotFound, FileDoesntExistResult, ValidationResult, UnexpectedFailureResult> deleteAllResult
            = await _productImageFileService.DeleteAllFilesForProductAsync(productId, deleteUserName);

        if (deleteAllResult.IsT0)
        {
            //_cache.Evict(GetAllByProductIdKey(productId));

            //foreach (ProductImageFileNameInfo productImageFile in productImageFilesForProduct)
            //{
            //    _cache.Evict(GetByIdKey(productImageFile.Id));

            //    if (productImageFile.ImageId is not null)
            //    {
            //        _cache.Evict(GetByProductIdAndImageIdKey(productImageFile.ProductId, productImageFile.ImageId.Value));
            //    }
            //}

            await _fusionCache.RemoveAsync(GetAllByProductIdKey(productId));

            foreach (ProductImageFileData productImageFile in productImageFilesForProduct)
            {
                await _fusionCache.RemoveAsync(GetByIdKey(productImageFile.Id));

                if (productImageFile.ImageId is not null)
                {
                    await _fusionCache.RemoveAsync(GetByProductIdAndImageIdKey(productImageFile.ProductId, productImageFile.ImageId.Value));
                }
            }
        }

        return deleteAllResult;
    }

    //private void AddOrUpdateByIdAndByImageIdCacheEntries(ProductImageFileNameInfo productImageFile)
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