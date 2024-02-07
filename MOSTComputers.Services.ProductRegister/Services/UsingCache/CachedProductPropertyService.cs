using FluentValidation;
using OneOf.Types;
using OneOf;
using FluentValidation.Results;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Models.Product.Models.Requests.ProductProperty;
using MOSTComputers.Services.Caching.Services.Contracts;
using static MOSTComputers.Services.ProductRegister.StaticUtilities.CacheKeyUtils.ProductProperty;
using MOSTComputers.Services.ProductRegister.StaticUtilities;

namespace MOSTComputers.Services.ProductRegister.Services.UsingCache;

internal sealed class CachedProductPropertyService : IProductPropertyService
{
    public CachedProductPropertyService(
        ProductPropertyService productPropertyService,
        ICache<string> cache)
    {
        _productPropertyService = productPropertyService;
        _cache = cache;

        _allCachedItemsTokenSource = new();
    }

    private readonly ProductPropertyService _productPropertyService;
    private readonly ICache<string> _cache;

    private readonly CancellationTokenSource _allCachedItemsTokenSource = new();

    public IEnumerable<ProductProperty> GetAllInProduct(uint productId)
    {
        return _cache.GetOrAdd(GetByProductIdKey((int)productId),
            () => _productPropertyService.GetAllInProduct(productId),
            _allCachedItemsTokenSource.Token);
    }

    public ProductProperty? GetByNameAndProductId(string name, uint productId)
    {
        IEnumerable<ProductProperty>? cachedProductProperties = _cache.GetValueOrDefault<IEnumerable<ProductProperty>>(GetByProductIdKey((int)productId));

        if (cachedProductProperties is not null)
        {
            return cachedProductProperties.First(x => x.Characteristic == name);
        }

        return _productPropertyService.GetByNameAndProductId(name, productId);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertWithCharacteristicId(ProductPropertyByCharacteristicIdCreateRequest createRequest,
        IValidator<ProductPropertyByCharacteristicIdCreateRequest>? validator = null)
    {
        OneOf<Success, ValidationResult, UnexpectedFailureResult> result = _productPropertyService.InsertWithCharacteristicId(createRequest, validator);

        bool successFromResult = result.Match(
            success => true,
            _ => false,
        _ => false);

        if (successFromResult)
        {
            _cache.Evict(GetByProductIdKey(createRequest.ProductId));

            _cache.Evict(CacheKeyUtils.Product.GetByIdKey(createRequest.ProductId));
        }

        return result;
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertWithCharacteristicName(ProductPropertyByCharacteristicNameCreateRequest createRequest,
        IValidator<ProductPropertyByCharacteristicNameCreateRequest>? validator = null)
    {
        OneOf<Success, ValidationResult, UnexpectedFailureResult> result = _productPropertyService.InsertWithCharacteristicName(createRequest, validator);

        bool successFromResult = result.Match(
            success => true,
            _ => false,
            _ => false);

        if (successFromResult)
        {
            _cache.Evict(GetByProductIdKey(createRequest.ProductId));

            _cache.Evict(CacheKeyUtils.Product.GetByIdKey(createRequest.ProductId));
        }

        return result;
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ProductPropertyUpdateRequest updateRequest,
        IValidator<ProductPropertyUpdateRequest>? validator = null)
    {
        OneOf<Success, ValidationResult, UnexpectedFailureResult> result = _productPropertyService.Update(updateRequest, validator);

        bool successFromResult = result.Match(
            success => true,
            _ => false,
            _ => false);

        if (successFromResult)
        {
            _cache.Evict(GetByProductIdKey(updateRequest.ProductId));

            _cache.Evict(CacheKeyUtils.Product.GetByIdKey(updateRequest.ProductId));
        }

        return result;
    }

    public bool Delete(uint productId, uint characteristicId)
    {
        bool success = _productPropertyService.Delete(productId, characteristicId);

        if (success)
        {
            string key = GetByProductIdKey((int)productId);

            IEnumerable<ProductProperty>? cachedProductProperties
                = _cache.GetValueOrDefault<IEnumerable<ProductProperty>>(key);

            if (cachedProductProperties is null) return success;

            _cache.Evict(key);

            _cache.Evict(CacheKeyUtils.Product.GetByIdKey((int)productId));

            _cache.Add(key, cachedProductProperties.Where(x => x.ProductCharacteristicId != (int)characteristicId));
        }

        return success;
    }

    public bool DeleteAllForProduct(uint productId)
    {
        bool success = _productPropertyService.DeleteAllForProduct(productId);

        if (success)
        {
            _cache.Evict(GetByProductIdKey((int)productId));

            _cache.Evict(CacheKeyUtils.Product.GetByIdKey((int)productId));
        }

        return success;
    }

    public bool DeleteAllForCharacteristic(uint characteristicId)
    {
        bool success = _productPropertyService.DeleteAllForCharacteristic(characteristicId);

        if (success)
        {
            _allCachedItemsTokenSource.Cancel();
        }

        return success;
    }
}