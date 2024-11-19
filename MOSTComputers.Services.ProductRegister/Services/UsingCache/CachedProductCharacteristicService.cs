using FluentValidation;
using OneOf.Types;
using OneOf;
using FluentValidation.Results;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.Caching.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Models.Grouping;
using static MOSTComputers.Services.ProductRegister.StaticUtilities.CacheKeyUtils.ProductCharacteristic;
using static MOSTComputers.Services.ProductRegister.StaticUtilities.ProductDataCloningUtils;
using MOSTComputers.Services.DAL.Models.Requests.ProductCharacteristic;

namespace MOSTComputers.Services.ProductRegister.Services.UsingCache;

internal sealed class CachedProductCharacteristicService : IProductCharacteristicService
{
    public CachedProductCharacteristicService(
        ProductCharacteristicService productCharacteristicService,
        ICache<string> cache)
    {
        _productCharacteristicService = productCharacteristicService;
        _cache = cache;
    }

    private readonly ProductCharacteristicService _productCharacteristicService;
    private readonly ICache<string> _cache;

    public IEnumerable<ProductCharacteristic> GetAllByCategoryId(int categoryId)
    {
        IEnumerable<ProductCharacteristic> productCharacteristicsWithCategoryId = _cache.GetOrAdd(GetByCategoryIdKey(categoryId),
            () => _productCharacteristicService.GetAllByCategoryId(categoryId));

        return CloneAll(productCharacteristicsWithCategoryId);
    }

    public IEnumerable<ProductCharacteristic> GetCharacteristicsOnlyByCategoryId(int categoryId)
    {
        IEnumerable<ProductCharacteristic>? cachedCategoryData
            = _cache.GetValueOrDefault<IEnumerable<ProductCharacteristic>>(GetByCategoryIdKey(categoryId));

        if (cachedCategoryData is not null)
        {
            cachedCategoryData = cachedCategoryData.Where(x => x.KWPrCh == ProductCharacteristicTypeEnum.ProductCharacteristic);

            if (cachedCategoryData.Any())
            {
                return CloneAll(cachedCategoryData);
            }
        }

        return _productCharacteristicService.GetCharacteristicsOnlyByCategoryId(categoryId);
    }

    public IEnumerable<ProductCharacteristic> GetSearchStringAbbreviationsOnlyByCategoryId(int categoryId)
    {
        IEnumerable<ProductCharacteristic>? cachedCategoryData
            = _cache.GetValueOrDefault<IEnumerable<ProductCharacteristic>>(GetByCategoryIdKey(categoryId));

        if (cachedCategoryData is not null)
        {
            cachedCategoryData = cachedCategoryData.Where(x => x.KWPrCh == ProductCharacteristicTypeEnum.SearchStringAbbreviation);

            if (cachedCategoryData.Any())
            {
                return CloneAll(cachedCategoryData);
            }
        }

        return _productCharacteristicService.GetSearchStringAbbreviationsOnlyByCategoryId(categoryId);
    }

    public IEnumerable<IGrouping<int, ProductCharacteristic>> GetAllForSelectionOfCategoryIds(IEnumerable<int> categoryIds)
    {
        Dictionary<int, IEnumerable<ProductCharacteristic>>? savedCategories = null;

        foreach (int categoryId in categoryIds)
        {
            IEnumerable<ProductCharacteristic>? productCharacteristicsForCategory
                = _cache.GetValueOrDefault<IEnumerable<ProductCharacteristic>>(GetByCategoryIdKey(categoryId));

            if (productCharacteristicsForCategory is not null)
            {
                savedCategories ??= new();

                savedCategories.Add(categoryId, productCharacteristicsForCategory);
            }
        }

        if (savedCategories is not null)
        {
            categoryIds = categoryIds.Except(savedCategories.Keys);

            if (!categoryIds.Any())
            {
                return savedCategories.Select(kvp => new Grouping<int, ProductCharacteristic>(kvp.Key, CloneAll(kvp.Value)));
            }
        }

        IEnumerable<IGrouping<int, ProductCharacteristic>> output = _productCharacteristicService.GetAllForSelectionOfCategoryIds(categoryIds);

        foreach (IGrouping<int, ProductCharacteristic> group in output)
        {
            _cache.Add<IEnumerable<ProductCharacteristic>>(GetByCategoryIdKey(group.Key), CloneAll(group));
        }

        if (savedCategories is not null)
        {
            IEnumerable<IGrouping<int, ProductCharacteristic>> savedPartOfOutput = savedCategories
                .Select(kvp => new Grouping<int, ProductCharacteristic>(kvp.Key, kvp.Value));

            output = output.Concat(savedPartOfOutput);
        }

        return output;
    }

    public IEnumerable<IGrouping<int, ProductCharacteristic>> GetCharacteristicsOnlyForSelectionOfCategoryIds(IEnumerable<int> categoryIds)
    {
        Dictionary<int, IEnumerable<ProductCharacteristic>>? savedCategories = GetCachedDataForGroupedParts(categoryIds,
            x => x.KWPrCh == ProductCharacteristicTypeEnum.ProductCharacteristic);

        if (savedCategories is not null)
        {
            categoryIds = categoryIds.Except(savedCategories.Keys);

            if (!categoryIds.Any())
            {
                return savedCategories.Select(kvp => new Grouping<int, ProductCharacteristic>(kvp.Key, CloneAll(kvp.Value)));
            }
        }

        IEnumerable<IGrouping<int, ProductCharacteristic>> output
            = _productCharacteristicService.GetCharacteristicsOnlyForSelectionOfCategoryIds(categoryIds);

        UpdateCacheForGroupedParts(output);

        if (savedCategories is not null)
        {
            IEnumerable<IGrouping<int, ProductCharacteristic>> savedPartOfOutput = savedCategories
                .Select(kvp => new Grouping<int, ProductCharacteristic>(kvp.Key, kvp.Value));

            output = output.Concat(savedPartOfOutput);
        }

        return output.Select(grouping => new Grouping<int, ProductCharacteristic>(grouping.Key, CloneAll(grouping)));
    }

    public IEnumerable<IGrouping<int, ProductCharacteristic>> GetSearchStringAbbreviationsOnlyForSelectionOfCategoryIds(IEnumerable<int> categoryIds)
    {
        Dictionary<int, IEnumerable<ProductCharacteristic>>? savedCategories = GetCachedDataForGroupedParts(categoryIds,
            x => x.KWPrCh == ProductCharacteristicTypeEnum.SearchStringAbbreviation);

        if (savedCategories is not null)
        {
            categoryIds = categoryIds.Except(savedCategories.Keys);

            if (!categoryIds.Any())
            {
                return savedCategories.Select(kvp => new Grouping<int, ProductCharacteristic>(kvp.Key, CloneAll(kvp.Value)));
            }
        }

        IEnumerable<IGrouping<int, ProductCharacteristic>> output
            = _productCharacteristicService.GetSearchStringAbbreviationsOnlyForSelectionOfCategoryIds(categoryIds);

        UpdateCacheForGroupedParts(output);

        if (savedCategories is not null)
        {
            IEnumerable<IGrouping<int, ProductCharacteristic>> savedPartOfOutput = savedCategories
                .Select(kvp => new Grouping<int, ProductCharacteristic>(kvp.Key, kvp.Value));

            output = output.Concat(savedPartOfOutput);
        }

        return output.Select(grouping => new Grouping<int, ProductCharacteristic>(grouping.Key, CloneAll(grouping)));
    }


    private Dictionary<int, IEnumerable<ProductCharacteristic>>? GetCachedDataForGroupedParts(IEnumerable<int> categoryIds,
        Predicate<ProductCharacteristic> partsAcceptablePredicate)
    {
        Dictionary<int, IEnumerable<ProductCharacteristic>>? savedCategories = null;

        foreach (int categoryId in categoryIds)
        {
            IEnumerable<ProductCharacteristic>? productCharacteristicsForCategory
                = _cache.GetValueOrDefault<IEnumerable<ProductCharacteristic>>(GetByCategoryIdKey(categoryId));

            List<ProductCharacteristic> characteristicsForEachCategory = new();

            if (productCharacteristicsForCategory is not null)
            {
                foreach (ProductCharacteristic characteristicForCategory in productCharacteristicsForCategory)
                {
                    if (partsAcceptablePredicate(characteristicForCategory))
                    {
                        characteristicsForEachCategory.Add(characteristicForCategory);
                    }
                }

                if (characteristicsForEachCategory.Count > 0)
                {
                    savedCategories ??= new();

                    savedCategories.Add(categoryId, characteristicsForEachCategory);
                }

            }
        }

        return savedCategories;
    }

    private void UpdateCacheForGroupedParts(IEnumerable<IGrouping<int, ProductCharacteristic>> output)
    {
        foreach (IGrouping<int, ProductCharacteristic> group in output)
        {
            string key = GetByCategoryIdKey(group.Key);

            IEnumerable<ProductCharacteristic>? cacheForOutput
                = _cache.GetValueOrDefault<IEnumerable<ProductCharacteristic>>(key);

            if (cacheForOutput is null)
            {
                _cache.Add<IEnumerable<ProductCharacteristic>>(key, group);

                continue;
            }

            cacheForOutput = cacheForOutput.Concat(group);

            _cache.Evict(key);

            _cache.Add(key, cacheForOutput);
        }
    }

    public ProductCharacteristic? GetById(int id)
    {
        if (id <= 0) return null;

        ProductCharacteristic? productCharacteristic = _productCharacteristicService.GetById(id);

        if (productCharacteristic is null) return null;

        return Clone(productCharacteristic);
    }

    public ProductCharacteristic? GetByCategoryIdAndName(int categoryId, string name)
    {
        ProductCharacteristic? productCharacteristic = _cache.GetOrAdd(GetByCategoryIdAndNameKey(categoryId, name),
            () => _productCharacteristicService.GetByCategoryIdAndName(categoryId, name));

        if (productCharacteristic is null) return null;

        return Clone(productCharacteristic);
    }

    public IEnumerable<ProductCharacteristic> GetSelectionByCategoryIdAndNames(int categoryId, List<string> names)
    {
        List<ProductCharacteristic> output = new();

        for (int i = 0; i < names.Count; i++)
        {
            string name = names[i];

            ProductCharacteristic? characteristicWithNameAndCategoryId
                = _cache.GetValueOrDefault<ProductCharacteristic>(GetByCategoryIdAndNameKey(categoryId, name));

            if (characteristicWithNameAndCategoryId is not null)
            {
                ProductCharacteristic characteristicClone = Clone(characteristicWithNameAndCategoryId);

                output.Add(characteristicClone);

                names.Remove(name);

                i--;
            }
        }

        if (names.Count <= 0) return output;

        IEnumerable<ProductCharacteristic> characteristicsThatArentCached
            = _productCharacteristicService.GetSelectionByCategoryIdAndNames(categoryId, names);

        foreach (ProductCharacteristic characteristic in characteristicsThatArentCached)
        {
            if (characteristic.Name is null) continue;

            _cache.Add(GetByCategoryIdAndNameKey(categoryId, characteristic.Name), Clone(characteristic));
        }

        output.AddRange(characteristicsThatArentCached);

        return output;
    }

    public IEnumerable<ProductCharacteristic> GetSelectionByCharacteristicIds(IEnumerable<int> ids)
    {
        IEnumerable<ProductCharacteristic> characteristics
            = _productCharacteristicService.GetSelectionByCharacteristicIds(ids);

        foreach (ProductCharacteristic characteristic in characteristics)
        {
            if (characteristic.CategoryId is null || characteristic.Name is null) continue;

            _cache.Add(GetByCategoryIdAndNameKey(characteristic.CategoryId.Value, characteristic.Name), Clone(characteristic));
        }

        return characteristics;
    }

    public ProductCharacteristic? GetByCategoryIdAndNameAndCharacteristicType(
        int categoryId,
        string name,
        ProductCharacteristicTypeEnum productCharacteristicType)
    {
        return _productCharacteristicService.GetByCategoryIdAndNameAndCharacteristicType(categoryId, name, productCharacteristicType);
    }

    public OneOf<int, ValidationResult, UnexpectedFailureResult> Insert(ProductCharacteristicCreateRequest createRequest, IValidator<ProductCharacteristicCreateRequest>? validator = null)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult = _productCharacteristicService.Insert(createRequest);

        bool successOfResult = insertResult.Match(
            id => true,
            _ => false,
            _ => false);

        if (successOfResult)
        {
            HandleCacheWhenInsertOrUpdateSuccessful(createRequest.CategoryId, createRequest.Name);
        }

        return insertResult;
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateById(ProductCharacteristicByIdUpdateRequest updateRequest, IValidator<ProductCharacteristicByIdUpdateRequest>? validator = null)
    {
        OneOf<Success, ValidationResult, UnexpectedFailureResult> updateResult = _productCharacteristicService.UpdateById(updateRequest);

        return updateResult;
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByNameAndCategoryId(ProductCharacteristicByNameAndCategoryIdUpdateRequest updateRequest, IValidator<ProductCharacteristicByNameAndCategoryIdUpdateRequest>? validator = null)
    {
        OneOf<Success, ValidationResult, UnexpectedFailureResult> updateResult = _productCharacteristicService.UpdateByNameAndCategoryId(updateRequest);

        bool successOfResult = updateResult.Match(
            success => true,
            _ => false,
            _ => false);

        if (successOfResult)
        {
            HandleCacheWhenInsertOrUpdateSuccessful(updateRequest.CategoryId, updateRequest.Name);
        }

        return updateResult;
    }

    private void HandleCacheWhenInsertOrUpdateSuccessful(int? categoryId, string? name)
    {
        if (categoryId is not null)
        {
            _cache.Evict(GetByCategoryIdKey(categoryId.Value));

            if (name is null) return;

            _cache.Evict(GetByCategoryIdAndNameKey(categoryId.Value, name));
        }
    }

    public bool Delete(int id)
    {
        if (id <= 0) return false;

        ProductCharacteristic? characteristic = GetById(id);

        bool success = _productCharacteristicService.Delete(id);

        if (success)
        {
            HandleCacheWhenInsertOrUpdateSuccessful(characteristic!.CategoryId, characteristic.Name);
        }

        return success;
    }

    public bool DeleteAllForCategory(int categoryId)
    {
        IEnumerable<ProductCharacteristic> characteristicsInCategory = GetAllByCategoryId(categoryId);

        if (!characteristicsInCategory.Any()) return false;

        bool success = _productCharacteristicService.DeleteAllForCategory(categoryId);

        if (success)
        {
            _cache.Evict(GetByCategoryIdKey(categoryId));

            foreach (ProductCharacteristic oldCachedCharacteristic in characteristicsInCategory)
            {
                if (oldCachedCharacteristic.Name is null) continue;

                _cache.Evict(GetByCategoryIdAndNameKey(categoryId, oldCachedCharacteristic.Name));
            }
        }

        return success;
    }
}