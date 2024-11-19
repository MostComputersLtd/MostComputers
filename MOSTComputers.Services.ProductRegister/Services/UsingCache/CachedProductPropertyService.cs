using FluentValidation;
using OneOf.Types;
using OneOf;
using FluentValidation.Results;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.Caching.Services.Contracts;
using MOSTComputers.Services.ProductRegister.StaticUtilities;
using static MOSTComputers.Services.ProductRegister.StaticUtilities.CacheKeyUtils.ProductProperty;
using static MOSTComputers.Services.ProductRegister.StaticUtilities.ProductDataCloningUtils;
using MOSTComputers.Services.DAL.Models.Requests.ProductProperty;

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

    public IEnumerable<ProductProperty> GetAllInProduct(int productId)
    {
        if (productId <= 0) return Enumerable.Empty<ProductProperty>();

        IEnumerable<ProductProperty> productProperties = _cache.GetOrAdd(GetByProductIdKey(productId),
            () => _productPropertyService.GetAllInProduct(productId),
            _allCachedItemsTokenSource.Token);

        return CloneAll(productProperties);
    }

    public ProductProperty? GetByNameAndProductId(string name, int productId)
    {
        if (productId <= 0) return null;

        IEnumerable<ProductProperty>? cachedProductProperties = _cache.GetValueOrDefault<IEnumerable<ProductProperty>>(GetByProductIdKey(productId));

        if (cachedProductProperties is not null)
        {
            ProductProperty cachedPropertyWithSameName = cachedProductProperties.First(x => x.Characteristic == name);

            return Clone(cachedPropertyWithSameName);
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

            _cache.Evict(CacheKeyUtils.ForProduct.GetByIdKey(createRequest.ProductId));
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

            _cache.Evict(CacheKeyUtils.ForProduct.GetByIdKey(createRequest.ProductId));
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

            _cache.Evict(CacheKeyUtils.ForProduct.GetByIdKey(updateRequest.ProductId));
        }

        return result;
    }

    public bool Delete(int productId, int characteristicId)
    {
        if (productId <= 0 || characteristicId <= 0) return false;

        bool success = _productPropertyService.Delete(productId, characteristicId);

        if (success)
        {
            string key = GetByProductIdKey(productId);

            _cache.Evict(key);

            _cache.Evict(CacheKeyUtils.ForProduct.GetByIdKey(productId));

            IEnumerable<ProductProperty>? cachedProductProperties
                = _cache.GetValueOrDefault<IEnumerable<ProductProperty>>(key);

            if (cachedProductProperties is null) return success;

            _cache.Add(key, cachedProductProperties.Where(x => x.ProductCharacteristicId != characteristicId));
        }

        return success;
    }

    public bool DeleteAllForProduct(int productId)
    {
        if (productId <= 0) return false;

        bool success = _productPropertyService.DeleteAllForProduct(productId);

        if (success)
        {
            _cache.Evict(GetByProductIdKey(productId));

            _cache.Evict(CacheKeyUtils.ForProduct.GetByIdKey(productId));
        }

        return success;
    }

    public bool DeleteAllForCharacteristic(int characteristicId)
    {
        if (characteristicId <= 0) return false;

        bool success = _productPropertyService.DeleteAllForCharacteristic(characteristicId);

        if (success)
        {
            _allCachedItemsTokenSource.Cancel();
        }

        return success;
    }
}