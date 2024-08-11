using System.Collections.Concurrent;
using FluentValidation;
using FluentValidation.Results;
using OneOf;
using OneOf.Types;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.Caching.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.StaticUtilities;
using static MOSTComputers.Services.ProductRegister.StaticUtilities.CacheKeyUtils.ForProduct;
using static MOSTComputers.Services.ProductRegister.StaticUtilities.ProductDataCloningUtils;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Services.UsingCache;

internal sealed class CachedProductService : IProductService
{
    public CachedProductService(
        ProductService productService,
        IProductImageService productImageService,
        IProductImageFileNameInfoService productImageFileNameInfoService,
        IProductPropertyService productPropertyService,
        ICache<string> cache)
    {
        _productService = productService;
        _productImageService = productImageService;
        _productImageFileNameInfoService = productImageFileNameInfoService;
        _productPropertyService = productPropertyService;
        _cache = cache;
    }

    private readonly ProductService _productService;
    private readonly IProductImageService _productImageService;
    private readonly IProductImageFileNameInfoService _productImageFileNameInfoService;
    private readonly IProductPropertyService _productPropertyService;
    private readonly ICache<string> _cache;

    private static readonly ConcurrentDictionary<string, CancellationTokenSource> _cancellationTokenSourcesForSearchStrings = new();

    private static readonly ConcurrentDictionary<string, CancellationTokenSource> _cancellationTokenSourcesForNames = new();

    private static CancellationTokenSource _cancellationTokenSourceForOrderSaves = new();

    public IEnumerable<Product> GetAllWithoutImagesAndProps()
    {
        return _productService.GetAllWithoutImagesAndProps();
    }

    public IEnumerable<Product> GetAllWhereSearchStringMatches(string searchStringParts)
    {
        if (string.IsNullOrWhiteSpace(searchStringParts)) return Enumerable.Empty<Product>();

        IEnumerable<Product> productsWhereSearchStringMatches = _cache.GetOrAdd(GetBySearchStringKey(searchStringParts),
            () => _productService.GetAllWhereSearchStringMatches(searchStringParts),
            _cancellationTokenSourcesForSearchStrings.GetOrAdd(searchStringParts, new CancellationTokenSource()).Token);

        return CloneAll(productsWhereSearchStringMatches);
    }

    public IEnumerable<Product> GetAllWhereNameMatches(string subString)
    {
        if (string.IsNullOrWhiteSpace(subString)) return Enumerable.Empty<Product>();

        IEnumerable<Product> productsWhereNameMatches = _cache.GetOrAdd(GetByNameKey(subString),
            () => _productService.GetAllWhereNameMatches(subString),
            _cancellationTokenSourcesForNames.GetOrAdd(subString, new CancellationTokenSource()).Token);

        return CloneAll(productsWhereNameMatches);
    }

    public IEnumerable<Product> GetFirstInRangeWhereSearchStringMatches(ProductRangeSearchRequest productRangeSearchRequest, string subString)
    {
        if (productRangeSearchRequest.Start <= 0
            || productRangeSearchRequest.Length == 0
            || string.IsNullOrWhiteSpace(subString)) return Enumerable.Empty<Product>();

        IEnumerable<Product>? allProductsWithThatSearchString = _cache.GetValueOrDefault<IEnumerable<Product>>(GetBySearchStringKey(subString));

        if (allProductsWithThatSearchString is not null)
        {
            IEnumerable<Product> productsInRangeWhereSearchStringMatches = allProductsWithThatSearchString
                .Skip(productRangeSearchRequest.Start)
                .Take((int)productRangeSearchRequest.Length);

            return CloneAll(productsInRangeWhereSearchStringMatches);
        }

        int emptyRegions = 0;

        int startInnerRegion = 0;
        uint endInnerRegion = 0;

        bool wasLastEmpty = false;

        List<Product>? cachedProducts = null;

        for (int i = productRangeSearchRequest.Start; i < productRangeSearchRequest.Start + productRangeSearchRequest.Length; i++)
        {
            Product? cachedProduct = _cache.GetValueOrDefault<Product>(GetBySearchStringAndOrderKey(subString, i));

            if (cachedProduct is not null)
            {
                cachedProducts ??= new();

                cachedProducts.Add(cachedProduct);

                wasLastEmpty = false;

                endInnerRegion = (uint)i;

                continue;
            }

            if (!wasLastEmpty)
            {
                emptyRegions++;

                if (emptyRegions > 1) break;

                startInnerRegion = i;
            }

            wasLastEmpty = true;
        }

        if (endInnerRegion == 0)
        {
            endInnerRegion = productRangeSearchRequest.Length;
        }

        if (emptyRegions == 1)
        {
            ProductRangeSearchRequest innerRangeSearchRequest = new() { Start = startInnerRegion, Length = endInnerRegion };

            IEnumerable<Product> missingProducts = _productService.GetFirstInRangeWhereSearchStringMatches(innerRangeSearchRequest, subString);

            if (!missingProducts.Any()) return CloneAll(missingProducts);

            CancellationTokenSource cancellationTokenSourceLocal = _cancellationTokenSourcesForSearchStrings.GetOrAdd(subString, new CancellationTokenSource());

            int orderLocal = startInnerRegion;

            foreach (Product product in missingProducts)
            {
                _cache.Add(GetByIdKey(product.Id), product);
                _cache.Add(GetBySearchStringAndOrderKey(subString, orderLocal), product, cancellationTokenSourceLocal.Token);

                orderLocal++;
            }

            if (cachedProducts is not null)
            {
                cachedProducts.AddRange(missingProducts);

                List<Product> productsClone = CloneAll(cachedProducts);

                return productsClone.OrderBy(x => x.DisplayOrder);
            }

            return CloneAll(missingProducts);
        }

        IEnumerable<Product> products = _productService.GetFirstInRangeWhereSearchStringMatches(productRangeSearchRequest, subString);

        if (!products.Any()) return CloneAll(products);

        CancellationTokenSource cancellationTokenSource = _cancellationTokenSourcesForSearchStrings.GetOrAdd(subString, new CancellationTokenSource());

        int order = productRangeSearchRequest.Start;

        foreach (Product product in products)
        {
            _cache.Add(GetByIdKey(product.Id), product);
            _cache.Add(GetBySearchStringAndOrderKey(subString, order), product, cancellationTokenSource.Token);

            order++;
        }

        return CloneAll(products);
    }

    public IEnumerable<Product> GetFirstInRangeWhereNameMatches(ProductRangeSearchRequest productRangeSearchRequest, string subString)
    {
        if (productRangeSearchRequest.Start <= 0
            || productRangeSearchRequest.Length == 0
            || string.IsNullOrWhiteSpace(subString)) return Enumerable.Empty<Product>();

        IEnumerable<Product>? allProductsWithThatName = _cache.GetValueOrDefault<IEnumerable<Product>>(GetByNameKey(subString));

        if (allProductsWithThatName is not null)
        {
            IEnumerable<Product> productsInRangeWhereNameMatches = allProductsWithThatName
                .Skip(productRangeSearchRequest.Start)
                .Take((int)productRangeSearchRequest.Length);

            return CloneAll(productsInRangeWhereNameMatches);
        }

        if (productRangeSearchRequest.Length == 0) return Enumerable.Empty<Product>();

        int emptyRegions = 0;

        int startInnerRegion = 0;
        uint endInnerRegion = 0;

        bool wasLastEmpty = false;

        List<Product>? cachedProducts = null;

        for (int i = productRangeSearchRequest.Start; i < productRangeSearchRequest.Start + productRangeSearchRequest.Length; i++)
        {
            Product? cachedProduct = _cache.GetValueOrDefault<Product>(GetByNameAndOrderKey(subString, (int)i));

            if (cachedProduct is not null)
            {
                cachedProducts ??= new();

                cachedProducts.Add(cachedProduct);

                wasLastEmpty = false;

                endInnerRegion = (uint)i;

                continue;
            }

            if (!wasLastEmpty)
            {
                emptyRegions++;

                if (emptyRegions > 1) break;

                startInnerRegion = i;
            }

            wasLastEmpty = true;
        }

        if (endInnerRegion == 0)
        {
            endInnerRegion = productRangeSearchRequest.Length;
        }

        if (emptyRegions == 1)
        {
            ProductRangeSearchRequest innerRangeSearchRequest = new() { Start = startInnerRegion, Length = endInnerRegion };

            IEnumerable<Product> missingProducts = _productService.GetFirstInRangeWhereNameMatches(innerRangeSearchRequest, subString);

            if (!missingProducts.Any()) return CloneAll(missingProducts);

            CancellationTokenSource cancellationTokenSourceLocal = _cancellationTokenSourcesForNames.GetOrAdd(subString, new CancellationTokenSource());

            int orderLocal = (int)startInnerRegion;

            foreach (Product product in missingProducts)
            {
                _cache.Add(GetByIdKey(product.Id), product);
                _cache.Add(GetByNameAndOrderKey(subString, orderLocal), product, cancellationTokenSourceLocal.Token);

                orderLocal++;
            }

            if (cachedProducts is not null)
            {
                cachedProducts.AddRange(missingProducts);

                List<Product> productsClone = CloneAll(cachedProducts);

                return productsClone.OrderBy(x => x.DisplayOrder);
            }

            return CloneAll(missingProducts);
        }

        IEnumerable<Product> products = _productService.GetFirstInRangeWhereNameMatches(productRangeSearchRequest, subString);

        if (!products.Any()) return CloneAll(products);

        CancellationTokenSource cancellationTokenSource = _cancellationTokenSourcesForNames.GetOrAdd(subString, new CancellationTokenSource());

        int order = productRangeSearchRequest.Start;

        foreach (Product product in products)
        {
            _cache.Add(GetByIdKey(product.Id), product);
            _cache.Add(GetByNameAndOrderKey(subString, order), product, cancellationTokenSource.Token);

            order++;
        }

        return CloneAll(products);
    }

    public IEnumerable<Product> GetFirstInRangeWhereAllConditionsAreMet(ProductRangeSearchRequest productRangeSearchRequest, ProductConditionalSearchRequest productConditionalSearchRequest)
    {
        if (productRangeSearchRequest.Start <= 0
            || productRangeSearchRequest.Length == 0) return Enumerable.Empty<Product>();

        return _productService.GetFirstInRangeWhereAllConditionsAreMet(productRangeSearchRequest, productConditionalSearchRequest);
    }

    public IEnumerable<Product> GetSelectionWithoutImagesAndProps(List<int> ids)
    {
        if (ids.Count <= 0) return Enumerable.Empty<Product>();

        ids = RemoveValuesSmallerThanOne(ids);

        List<Product>? cachedProducts = null;

        for (int i = 0; i < ids.Count; i++)
        {
            int id = ids[i];

            Product? cachedProduct = _cache.GetValueOrDefault<Product>(GetByIdKey(id));

            if (cachedProduct is null) continue;

            cachedProducts ??= new();

            cachedProducts.Add(cachedProduct);

            ids.RemoveAt(i);

            i--;
        }

        if (ids.Count <= 0) return CloneAll(cachedProducts!);

        IEnumerable<Product> products = _productService.GetSelectionWithoutImagesAndProps(ids);

        foreach (Product product in products)
        {
            _cache.Add(GetByIdKey(product.Id), product);
        }

        if (cachedProducts is null) return CloneAll(products);

        cachedProducts.AddRange(products);

        List<Product> productsClone = CloneAll(cachedProducts);

        return productsClone.OrderBy(x => x.DisplayOrder);
    }

    public IEnumerable<Product> GetSelectionWithFirstImage(List<int> ids)
    {
        if (ids.Count <= 0) return Enumerable.Empty<Product>();

        ids = RemoveValuesSmallerThanOne(ids);

        List<Product>? cachedProducts = null;

        for (int i = 0; i < ids.Count; i++)
        {
            int id = ids[i];

            Product? cachedProduct = _cache.GetValueOrDefault<Product>(GetByIdKey(id));

            if (cachedProduct is null) continue;

            if (cachedProduct.Images is not null)
            {
                cachedProducts ??= new();

                cachedProducts.Add(cachedProduct);

                ids.RemoveAt(i);

                i--;

                continue;
            }

            ProductImage? cachedProductFirstImage
                = _cache.GetValueOrDefault<ProductImage>(CacheKeyUtils.ProductImage.GetInFirstImagesByIdKey(id));

            if (cachedProductFirstImage is null) continue;

            cachedProducts ??= new();

            cachedProduct.Images = new List<ProductImage>() { Clone(cachedProductFirstImage) };

            cachedProducts.Add(cachedProduct);

            ids.RemoveAt(i);

            i--;
        }

        if (ids.Count <= 0) return CloneAll(cachedProducts!);

        IEnumerable<Product> products = _productService.GetSelectionWithFirstImage(ids);

        foreach (Product product in products)
        {
            if (product.Images is not null
                && product.Images.Count > 0)
            {
                _cache.Add(CacheKeyUtils.ProductImage.GetInFirstImagesByIdKey(product.Id), product.Images[0]);
            }

            _cache.Add(GetByIdKey(product.Id), product);
        }

        if (cachedProducts is null) return CloneAll(products);

        cachedProducts.AddRange(products);

        List<Product> productsClone = CloneAll(cachedProducts);

        return productsClone.OrderBy(x => x.DisplayOrder);
    }

    public IEnumerable<Product> GetSelectionWithProps(List<int> ids)
    {
        if (ids.Count <= 0) return Enumerable.Empty<Product>();

        ids = RemoveValuesSmallerThanOne(ids);

        List<Product>? cachedProducts = null;

        for (int i = 0; i < ids.Count; i++)
        {
            int id = ids[i];

            Product? cachedProduct = _cache.GetValueOrDefault<Product>(GetByIdKey(id));

            if (cachedProduct is null) continue;

            if (cachedProduct.Properties is not null)
            {
                cachedProducts ??= new();

                cachedProducts.Add(cachedProduct);

                ids.RemoveAt(i);

                i--;

                continue;
            }

            IEnumerable<ProductProperty>? cachedProductProps
                = _cache.GetValueOrDefault<IEnumerable<ProductProperty>>(CacheKeyUtils.ProductProperty.GetByProductIdKey(id));

            if (cachedProductProps is null) continue;

            cachedProducts ??= new();

            cachedProduct.Properties = CloneAll(cachedProductProps);

            cachedProducts.Add(cachedProduct);

            ids.RemoveAt(i);

            i--;
        }

        if (ids.Count <= 0) return CloneAll(cachedProducts!);

        IEnumerable<Product> products = _productService.GetSelectionWithProps(ids);

        foreach (Product product in products)
        {
            if (product.Properties is not null)
            {
                _cache.Add(CacheKeyUtils.ProductProperty.GetByProductIdKey(product.Id), product.Properties);
            }

            _cache.Add(GetByIdKey(product.Id), product);
        }

        if (cachedProducts is null) return CloneAll(products);

        cachedProducts.AddRange(products);

        List<Product> productsClone = CloneAll(cachedProducts);

        return productsClone.OrderBy(x => x.DisplayOrder);
    }

    public IEnumerable<Product> GetFirstItemsBetweenStartAndEnd(ProductRangeSearchRequest rangeSearchRequest)
    {
        if (rangeSearchRequest.Start <= 0
            || rangeSearchRequest.Length == 0) return Enumerable.Empty<Product>();

        Dictionary<int, Product>? orderedProducts = null;
        Dictionary<int, int> rangesThatArentCached = new();

        for (int i = rangeSearchRequest.Start; i < rangeSearchRequest.Start + rangeSearchRequest.Length; i++)
        {
            Product? cachedProduct = _cache.GetValueOrDefault<Product>(GetByOrderKey(i));

            if (cachedProduct is not null)
            {
                orderedProducts ??= new();

                orderedProducts.Add(i, cachedProduct);

                continue;
            }

            if (rangesThatArentCached.ContainsValue(i - 1))
            {
                rangesThatArentCached[rangesThatArentCached.Count - 1] = i;

                continue;
            }

            rangesThatArentCached.Add(i, i + 1);
        }

        if (rangesThatArentCached.Count == 0
            && orderedProducts is not null) return orderedProducts.Values;

        if (rangesThatArentCached.Count > 1)
        {
            IEnumerable<Product> products = _productService.GetFirstItemsBetweenStartAndEnd(rangeSearchRequest);

            int j = 0;

            foreach (Product product in products)
            {
                if (orderedProducts?.ContainsKey(j) ?? false) continue;

                _cache.Add(GetByOrderKey(j + rangeSearchRequest.Start), product, _cancellationTokenSourceForOrderSaves.Token);
                _cache.Add(GetByIdKey(product.Id), product);

                j++;
            }

            return CloneAll(products);
        }

        KeyValuePair<int, int> kvp = rangesThatArentCached.First();

        ProductRangeSearchRequest newRangeSearchRequest = new() { Start = kvp.Key, Length = (uint)kvp.Value };

        IEnumerable<Product> missingProducts = _productService.GetFirstItemsBetweenStartAndEnd(newRangeSearchRequest);

        int k = 0;

        orderedProducts = new();

        foreach (Product product in missingProducts)
        {
            int order = k + rangeSearchRequest.Start;

            _cache.Add(GetByOrderKey(order), product, _cancellationTokenSourceForOrderSaves.Token);
            _cache.Add(GetByIdKey(product.Id), product);

            orderedProducts.Add(order, product);

            k++;
        }

        return orderedProducts
            .OrderBy(kvp => kvp.Key)
            .Select(x => Clone(x.Value));
    }

    public Product? GetByIdWithFirstImage(int id)
    {
        if (id <= 0) return null;

        string productKey = GetByIdKey(id);

        string productFirstImageKey = CacheKeyUtils.ProductImage.GetInFirstImagesByIdKey(id);

        Product? cached = _cache.GetValueOrDefault<Product>(productKey);

        if (cached is not null
            && cached.Images is not null)
        {
            return Clone(cached);
        }

        ProductImage? productFirstImageCached
            = _cache.GetValueOrDefault<ProductImage>(productFirstImageKey);

        if (cached is not null
            && productFirstImageCached is not null)
        {
            cached.Images = new List<ProductImage>() { productFirstImageCached };

            return Clone(cached);
        }

        Product? product = _productService.GetByIdWithFirstImage(id);

        if (product is null) return null;

        if (product.Images is not null
            && product.Images.Count > 0)
        {
            _cache.Add(productFirstImageKey, product.Images[0]);
        }

        if (cached is null)
        {
            _cache.Add(productKey, product);

            return Clone(product);
        }

        if (product.Images is not null
            && product.Images.Count > 0)
        {
            ProductImage productFirstImageClone = Clone(product.Images[0]);

            cached.Images = new() { productFirstImageClone };
        }

        return Clone(product);
    }

    public Product? GetByIdWithProps(int id)
    {
        if (id <= 0) return null;

        string productKey = GetByIdKey(id);

        string productPropertiesKey = CacheKeyUtils.ProductProperty.GetByProductIdKey(id);

        Product? cached = _cache.GetValueOrDefault<Product>(productKey);

        if (cached is not null
            && cached.Properties is not null)
        {
            return Clone(cached);
        }

        IEnumerable<ProductProperty>? productPropsCached
            = _cache.GetValueOrDefault<IEnumerable<ProductProperty>>(productPropertiesKey);

        if (cached is not null
            && productPropsCached is not null)
        {
            cached.Properties = productPropsCached
                .ToList();

            return Clone(cached);
        }

        Product? product = _productService.GetByIdWithProps(id);

        if (product is null) return null;

        if (product.Properties is not null)
        {
            _cache.Add(productPropertiesKey, product.Properties);
        }

        if (cached is null)
        {
            _cache.Add(productKey, product);

            return Clone(product);
        }

        if (product.Properties is not null)
        {
            List<ProductProperty> productPropertiesClone = CloneAll(product.Properties);

            cached.Properties = productPropertiesClone;
        }

        return Clone(product);
    }

    public Product? GetByIdWithImages(int id)
    {
        if (id <= 0) return null;

        string productKey = GetByIdKey(id);

        string productImagesKey = CacheKeyUtils.ProductImage.GetInAllImagesByProductIdKey(id);

        Product? cached = _cache.GetValueOrDefault<Product>(productKey);

        if (cached is not null
            && cached.Images is not null)
        {
            return Clone(cached);
        }

        IEnumerable<ProductImage>? productImagesCached
            = _cache.GetValueOrDefault<IEnumerable<ProductImage>>(productImagesKey);

        if (cached is not null
            && productImagesCached is not null)
        {
            cached.Images = productImagesCached.ToList();

            return Clone(cached);
        }

        Product? product = _productService.GetByIdWithImages(id);

        if (product is null) return null;

        if (product.Images is not null)
        {
            _cache.Add(productImagesKey, product.Images);
        }

        if (cached is null)
        {
            _cache.Add(productKey, product);

            return Clone(product);
        }

        if (product.Images is not null)
        {
            cached.Images = CloneAll(product.Images);
        }

        return Clone(product);
    }

    public Product? GetProductWithHighestId()
    {
        return _productService.GetProductWithHighestId();
    }

    public Product? GetProductFull(int productId)
    {
        if (productId <= 0) return null;

        string productKey = GetByIdKey(productId);

        string productImagesKey = CacheKeyUtils.ProductImage.GetInAllImagesByProductIdKey(productId);

        Product? cachedProduct = _cache.GetValueOrDefault<Product>(productImagesKey);

        if (cachedProduct is not null)
        {
            cachedProduct.Images ??= _productImageService.GetAllInProduct(productId)
                .ToList();

            cachedProduct.ImageFileNames ??= _productImageFileNameInfoService.GetAllInProduct(productId)
                .ToList();

            cachedProduct.Properties ??= _productPropertyService.GetAllInProduct(productId)
                .ToList();

            return Clone(cachedProduct);
        }

        Product? product = _productService.GetProductFull(productId);

        if (product is null) return null;

        _cache.Add(productKey, product);

        return Clone(product);
    }

    public OneOf<int, ValidationResult, UnexpectedFailureResult> Insert(ProductCreateRequest createRequest,
        IValidator<ProductCreateRequest>? validator = null)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> result = _productService.Insert(createRequest, validator);

        int? idFromResult = result.Match<int?>(
            id => (int)id,
            _ => null,
            _ => null);

        if (idFromResult is not null)
        {
            _cache.Evict(GetByIdKey(idFromResult.Value));
            _cancellationTokenSourceForOrderSaves.Cancel();

            if (_cancellationTokenSourceForOrderSaves.IsCancellationRequested)
            {
                _cancellationTokenSourceForOrderSaves.Dispose();
                _cancellationTokenSourceForOrderSaves = new();
            }

            _cache.Evict(CacheKeyUtils.ProductImage.GetInAllImagesByProductIdKey(idFromResult.Value));
            _cache.Evict(CacheKeyUtils.ProductImage.GetInFirstImagesByIdKey(idFromResult.Value));
            _cache.Evict(CacheKeyUtils.ProductProperty.GetByProductIdKey(idFromResult.Value));
            _cache.Evict(CacheKeyUtils.ProductImageFileNameInfo.GetByProductIdKey(idFromResult.Value));
            _cache.Evict(CacheKeyUtils.ProductStatuses.GetByProductIdKey(idFromResult.Value));

            if (createRequest.SearchString is not null)
            {
                foreach (KeyValuePair<string, CancellationTokenSource> kvp in _cancellationTokenSourcesForSearchStrings)
                {
                    if (!SearchStringSearchUtils.SearchStringContainsParts(createRequest.SearchString, kvp.Key)) continue;

                    kvp.Value.Cancel();

                    if (kvp.Value.IsCancellationRequested)
                    {
                        kvp.Value.Dispose();

                        _cancellationTokenSourcesForSearchStrings[kvp.Key] = new CancellationTokenSource();
                    }

                    _cache.Evict(GetBySearchStringKey(kvp.Key));
                }
            }

            if (createRequest.Name is not null)
            {
                foreach (KeyValuePair<string, CancellationTokenSource> kvp in _cancellationTokenSourcesForNames)
                {
                    if (!createRequest.Name.Contains(kvp.Key)) continue;

                    kvp.Value.Cancel();

                    if (kvp.Value.IsCancellationRequested)
                    {
                        kvp.Value.Dispose();

                        _cancellationTokenSourcesForNames[kvp.Key] = new CancellationTokenSource();
                    }

                    _cache.Evict(GetByNameKey(kvp.Key));
                }
            }
        }

        return result;
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ProductUpdateRequest updateRequest, IValidator<ProductUpdateRequest>? validator = null)
    {
        int productId = updateRequest.Id;

        Product? productBeforeUpdate = GetByIdWithImages(productId);

        if (productBeforeUpdate is null)
        {
            ValidationResult productDoesntExistValidationResult = new();

            productDoesntExistValidationResult.Errors.Add(new(nameof(updateRequest.Id), "Argument does not correspond to any product Id"));

            return productDoesntExistValidationResult;
        }

        productBeforeUpdate.Properties ??= _productPropertyService.GetAllInProduct(updateRequest.Id)
            .ToList();

        productBeforeUpdate.ImageFileNames ??= _productImageFileNameInfoService.GetAllInProduct(updateRequest.Id)
            .ToList();

        string updatedProductKey = GetUpdatedByIdKey(updateRequest.Id);

        _cache.Add(updatedProductKey, productBeforeUpdate);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> result = _productService.Update(updateRequest, validator);

        bool successFromResult = result.Match(
            success => true,
            _ => false,
            _ => false);

        if (!successFromResult)
        {
            _cache.Evict(updatedProductKey);

            return result;
        }

        _cache.Evict(GetByIdKey(productId));
        _cancellationTokenSourceForOrderSaves.Cancel();

        if (_cancellationTokenSourceForOrderSaves.IsCancellationRequested)
        {
            _cancellationTokenSourceForOrderSaves.Dispose();
            _cancellationTokenSourceForOrderSaves = new();
        }

        _cache.Evict(CacheKeyUtils.ProductStatuses.GetByProductIdKey(productId));

        if (updateRequest.Images is not null
            && updateRequest.Images.Count > 0)
        {
            _cache.Evict(CacheKeyUtils.ProductImage.GetInAllImagesByProductIdKey(productId));

            _cache.Evict(CacheKeyUtils.ProductImage.GetInFirstImagesByIdKey(productId));

            if (productBeforeUpdate.Images is not null)
            {
                foreach (ProductImage image in productBeforeUpdate.Images)
                {
                    _cache.Evict(CacheKeyUtils.ProductImage.GetInAllImagesByIdKey(image.Id));
                }
            }
        }

        if (updateRequest.Properties is not null
            && updateRequest.Properties.Count > 0)
        {
            _cache.Evict(CacheKeyUtils.ProductProperty.GetByProductIdKey(productId));
        }

        if (updateRequest.ImageFileNames is not null
            && updateRequest.ImageFileNames.Count > 0)
        {
            _cache.Evict(CacheKeyUtils.ProductImageFileNameInfo.GetByProductIdKey(productId));
            _cache.Evict(CacheKeyUtils.ProductImageFileNameInfo.GetAllKey);
        }

        if (updateRequest.SearchString is not null)
        {
            foreach (KeyValuePair<string, CancellationTokenSource> kvp in _cancellationTokenSourcesForSearchStrings)
            {
                if (!SearchStringSearchUtils.SearchStringContainsParts(updateRequest.SearchString, kvp.Key)) continue;

                kvp.Value.Cancel();

                if (kvp.Value.IsCancellationRequested)
                {
                    kvp.Value.Dispose();

                    _cancellationTokenSourcesForSearchStrings[kvp.Key] = new CancellationTokenSource();
                }

                _cache.Evict(GetBySearchStringKey(kvp.Key));
            }
        }

        if (updateRequest.Name is not null)
        {
            foreach (KeyValuePair<string, CancellationTokenSource> kvp in _cancellationTokenSourcesForNames)
            {
                if (!updateRequest.Name.Contains(kvp.Key)) continue;

                kvp.Value.Cancel();

                if (kvp.Value.IsCancellationRequested)
                {
                    kvp.Value.Dispose();

                    _cancellationTokenSourcesForNames[kvp.Key] = new CancellationTokenSource();
                }

                _cache.Evict(GetByNameKey(kvp.Key));
            }
        }
        
        return result;
    }

    public bool Delete(int id)
    {
        if (id <= 0) return false;

        Product? productBeforeUpdate = GetByIdWithFirstImage(id);

        if (productBeforeUpdate is null) return true;

        productBeforeUpdate.Properties ??= _productPropertyService.GetAllInProduct(id)
            .ToList();

        productBeforeUpdate.ImageFileNames ??= _productImageFileNameInfoService.GetAllInProduct(id)
            .ToList();

        bool success = _productService.Delete(id);

        if (success)
        {
            _cache.Evict(GetByIdKey(id));
            _cancellationTokenSourceForOrderSaves.Cancel();

            if (_cancellationTokenSourceForOrderSaves.IsCancellationRequested)
            {
                _cancellationTokenSourceForOrderSaves.Dispose();
                _cancellationTokenSourceForOrderSaves = new();
            }

            _cache.Evict(CacheKeyUtils.ProductImage.GetInAllImagesByProductIdKey(id));
            _cache.Evict(CacheKeyUtils.ProductImage.GetInFirstImagesByIdKey(id));
            _cache.Evict(CacheKeyUtils.ProductProperty.GetByProductIdKey(id));
            _cache.Evict(CacheKeyUtils.ProductImageFileNameInfo.GetByProductIdKey(id));
            _cache.Evict(CacheKeyUtils.ProductImageFileNameInfo.GetAllKey);
            _cache.Evict(CacheKeyUtils.ProductStatuses.GetByProductIdKey(id));

            if (productBeforeUpdate.Images is not null)
            {
                foreach (ProductImage image in productBeforeUpdate.Images)
                {
                    _cache.Evict(CacheKeyUtils.ProductImage.GetInAllImagesByIdKey(image.Id));
                }
            }

            if (productBeforeUpdate.SearchString is not null)
            {
                foreach (KeyValuePair<string, CancellationTokenSource> kvp in _cancellationTokenSourcesForSearchStrings)
                {
                    if (!SearchStringSearchUtils.SearchStringContainsParts(productBeforeUpdate.SearchString, kvp.Key)) continue;

                    kvp.Value.Cancel();

                    if (kvp.Value.IsCancellationRequested)
                    {
                        kvp.Value.Dispose();

                        _cancellationTokenSourcesForSearchStrings[kvp.Key] = new CancellationTokenSource();
                    }

                    _cache.Evict(GetBySearchStringKey(kvp.Key));
                }
            }

            if (productBeforeUpdate.Name is not null)
            {
                foreach (KeyValuePair<string, CancellationTokenSource> kvp in _cancellationTokenSourcesForNames)
                {
                    if (!productBeforeUpdate.Name.Contains(kvp.Key)) continue;

                    kvp.Value.Cancel();

                    if (kvp.Value.IsCancellationRequested)
                    {
                        kvp.Value.Dispose();

                        _cancellationTokenSourcesForNames[kvp.Key] = new CancellationTokenSource();
                    }

                    _cache.Evict(GetByNameKey(kvp.Key));
                }
            }
        }

        return success;
    }
}