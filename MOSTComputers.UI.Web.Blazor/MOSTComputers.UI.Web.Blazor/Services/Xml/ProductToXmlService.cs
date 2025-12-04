using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.Promotions;
using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Services.Currencies.Contracts;
using MOSTComputers.Services.Currencies.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.ProductData;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.New.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductProperties.Contacts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;
using MOSTComputers.Services.SearchStringOrigin.Models;
using MOSTComputers.Services.SearchStringOrigin.Services.Contracts;
using MOSTComputers.UI.Web.Blazor.Endpoints.Images;
using MOSTComputers.UI.Web.Blazor.Models.Search.Product;
using MOSTComputers.UI.Web.Blazor.Services.Search.Contracts;
using MOSTComputers.UI.Web.Blazor.Services.Xml.Contracts;
using OneOf;
using OneOf.Types;
using static MOSTComputers.Utils.Files.FilePathUtils;

namespace MOSTComputers.UI.Web.Blazor.Services.Xml;

public sealed class ProductToXmlService : IProductToXmlService
{
    public sealed class ProductXmlOptions
    {
        public string? ApplicationRootPath { get; set; }
        public Currency? PrefferedPriceCurrency { get; set; }
    }

    private readonly IProductService _productService;
    private readonly IProductSearchService _productSearchService;
    private readonly IProductPropertyService _productPropertyService;
    private readonly IProductImageService _productImageService;
    private readonly IProductImageFileService _productImageFileService;
    private readonly IPromotionService _promotionService;
    private readonly IManufacturerToPromotionGroupRelationService _manufacturerToPromotionGroupRelationService;
    private readonly ISubCategoryService _subCategoryService;
    private readonly IExchangeRateService _exchangeRateService;
    private readonly ICurrencyConversionService _currencyConversionService;
    private readonly IProductXmlService _productXmlService;
    private readonly ISearchStringOriginService _searchStringOriginService;

    private readonly IProductToXmlProductMappingService _productToXmlProductMappingService;

    private const string _pidPromotionTypeInXml = "Promotion";
    private const string _ridPromotionTypeInXml = "Consignation";

    private static readonly Dictionary<int, string> _promotionImageIdToInfoStringInXml = new()
    {
        { 1, "New" },
        { 2, "Price Down" },
        { 3, "Price Up" },
        { 4, "Promotion" },
        { 5, "Coming Soon" },
        { 6, "Special Offer" },
        { 7, "Gift" },
        { 8, "Best Offer" },
        { 9, "Only Today" },
        { 10, "Limited" },
        { 11, "On Sale" },
        { 12, "Voucher" },
    };

    public ProductToXmlService(
        IProductService productService,
        IProductSearchService productSearchService,
        IProductPropertyService productPropertyService,
        IProductImageService productImageService,
        IProductImageFileService productImageFileService,
        IPromotionService promotionService,
        IManufacturerToPromotionGroupRelationService manufacturerToPromotionGroupRelationService,
        ISubCategoryService subCategoryService,
        IExchangeRateService exchangeRateService,
        ICurrencyConversionService currencyConversionService,
        IProductXmlService productXmlService,
        ISearchStringOriginService searchStringOriginService,
        IProductToXmlProductMappingService productToXmlProductMappingService)
    {
        _productService = productService;
        _productPropertyService = productPropertyService;
        _productImageService = productImageService;
        _productImageFileService = productImageFileService;
        _promotionService = promotionService;
        _subCategoryService = subCategoryService;
        _exchangeRateService = exchangeRateService;
        _productXmlService = productXmlService;
        _searchStringOriginService = searchStringOriginService;
        _productToXmlProductMappingService = productToXmlProductMappingService;
        _manufacturerToPromotionGroupRelationService = manufacturerToPromotionGroupRelationService;
        _productSearchService = productSearchService;
        _currencyConversionService = currencyConversionService;
    }

    public async Task<OneOf<string, InvalidXmlResult>> TryGetXmlForAllProductsAsync(ProductXmlOptions? productXmlOptions = null)
    {
        List<XmlProduct> xmlProducts = await GetXmlProductsForAllProductsAsync(productXmlOptions);

        ProductsXmlFullData xmlObjectData = await GetXmlObjectDataForProductsAsync(xmlProducts);

        OneOf<string, InvalidXmlResult> serializeProductXmlResult
            = await _productXmlService.TrySerializeProductsXmlAsync(xmlObjectData);

        return serializeProductXmlResult;
    }

    public async Task TryGetXmlForAllProductsAsync(Stream outputStream, ProductXmlOptions? productXmlOptions = null)
    {
        List<XmlProduct> xmlProducts = await GetXmlProductsForAllProductsAsync(productXmlOptions);

        ProductsXmlFullData xmlObjectData = await GetXmlObjectDataForProductsAsync(xmlProducts);

        await _productXmlService.TrySerializeProductsXmlAsync(outputStream, xmlObjectData);
    }

    public async Task<OneOf<string, InvalidXmlResult>> TryGetXmlForProductsAsync(List<int> productIds, ProductXmlOptions? productXmlOptions = null)
    {
        List<XmlProduct> xmlProducts = await GetXmlProductsByProductIdsAsync(productIds, productXmlOptions);

        ProductsXmlFullData xmlObjectData = await GetXmlObjectDataForProductsAsync(xmlProducts);

        OneOf<string, InvalidXmlResult> serializeProductXmlResult
            = await _productXmlService.TrySerializeProductsXmlAsync(xmlObjectData);

        return serializeProductXmlResult;
    }

    public async Task TryGetXmlForProductsAsync(Stream outputStream, List<int> productIds, ProductXmlOptions? productXmlOptions = null)
    {
        List<XmlProduct> xmlProducts = await GetXmlProductsByProductIdsAsync(productIds, productXmlOptions);

        ProductsXmlFullData xmlObjectData = await GetXmlObjectDataForProductsAsync(xmlProducts);

        await _productXmlService.TrySerializeProductsXmlAsync(outputStream, xmlObjectData);
    }

    public async Task<ProductsXmlFullData> GetXmlObjectDataForProductsAsync(List<XmlProduct> xmlProducts)
    {
        ExchangeRate? usdToEurExchangeRate = await _exchangeRateService.GetForCurrenciesAsync(Currency.EUR, Currency.USD);

        DateTime currentDateTime = DateTime.Now;

        DateTime exchangeRateValid = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, currentDateTime.Hour, 0, 0, currentDateTime.Kind)
            .AddHours(1);

        XmlExchangeRates xmlExchangeRates = new()
        {
            ExchangeRateUSD = (usdToEurExchangeRate is not null) ? new() { ExchangeRatePerEUR = usdToEurExchangeRate.Rate } : null,
            ValidTo = exchangeRateValid,
        };

        ProductsXmlFullData xmlObjectData = new()
        {
            Products = xmlProducts,
            DateOfExport = DateTime.Now,
            ExchangeRates = xmlExchangeRates,
        };

        return xmlObjectData;
    }

    private async Task<List<XmlProduct>> GetXmlProductsForAllProductsAsync(ProductXmlOptions? productXmlOptions = null)
    {
        ProductSearchRequest productSearchRequest = new()
        {
            OnlyVisibleByEndUsers = true,
            ProductStatus = ProductStatusSearchOptions.AvailableAndCall
        };

        List<Product> products = await _productSearchService.SearchProductsAsync(productSearchRequest);

        Dictionary<int, List<SearchStringPartOriginData>> searchStringOriginDatas
            = await _searchStringOriginService.GetSearchStringPartsAndDataAboutTheirOriginForProductsAsync(products);

        List<IGrouping<int, ProductProperty>> productProperties = await _productPropertyService.GetAllAsync();
        List<IGrouping<int, ProductImageData>> productImages = await _productImageService.GetAllWithoutFileDataAsync();

        List<ProductImageFileData> productImageFileNameInfos = await _productImageFileService.GetAllAsync();

        List<IGrouping<int, ProductImageFileData>> productImageFileNameInfosGrouped = productImageFileNameInfos.GroupBy(x => x.ProductId)
            .ToList();

        List<Promotion> promotions = await _promotionService.GetAllAsync();

        List<ManufacturerToPromotionGroupRelation> promotionGroupRelations = await _manufacturerToPromotionGroupRelationService.GetAllAsync();

        List<SubCategory> subCategories = await _subCategoryService.GetAllAsync();

        Dictionary<Product, OneOf<decimal, NotFound>>? pricesForProductsInPrefferedCurrency = null;

        if (productXmlOptions?.PrefferedPriceCurrency is not null)
        {
            pricesForProductsInPrefferedCurrency = await GetChangedCurrenciesForProductsAsync(products, productXmlOptions.PrefferedPriceCurrency.Value);
        }

        List<XmlProduct> xmlProducts = new();

        foreach (Product product in products)
        {
            searchStringOriginDatas.TryGetValue(product.Id, out List<SearchStringPartOriginData>? searchStringOriginData);

            List<XmlSearchStringPartInfo>? searchStringParts = null;

            if (searchStringOriginData is not null)
            {
                searchStringParts = GetSearchStringDataForXmlFromSearchStringData(searchStringOriginData);
            }

            SubCategory? productSubCategory = null;

            if (product.SubCategoryId is not null)
            {
                productSubCategory = subCategories.FirstOrDefault(x => x.Id == product.SubCategoryId);
            }

            List<ProductProperty>? relatedProductProperties = productProperties.FirstOrDefault(x => x.Key == product.Id)?
                .ToList();

            List<ProductImageData>? relatedProductImages = productImages.FirstOrDefault(x => x.Key == product.Id)?
                .ToList();

            List<ProductImageFileData>? relatedProductImageFileNameInfos
                = productImageFileNameInfosGrouped.FirstOrDefault(x => x.Key == product.Id)?
                    .ToList();

            List<XmlProductImage> xmlProductImages = GetXmlProductImagesFromProductImages(
                relatedProductImages, relatedProductImageFileNameInfos, productXmlOptions?.ApplicationRootPath);

            IEnumerable<Promotion>? productPromotions = promotions.Where(x =>
            {
                if (x.Id != product.PromotionPid && x.Id != product.PromotionRid) return false;

                return (x.StartDate is null || x.StartDate <= DateTime.Now) && (x.ExpirationDate is null || x.ExpirationDate >= DateTime.Now);
            });

            List<XmlPromotion> xmlPromotions = GetXmlPromotionsFromPromotions(product, productPromotions, productXmlOptions);

            XmlGroupPromotion? xmlGroupPromotion = null;

            ManufacturerToPromotionGroupRelation? promotionGroupRelation
                = promotionGroupRelations.FirstOrDefault(x => x.ManufacturerId == product.ManufacturerId);

            string? promotionGroupImagesPageUrl = null;

            if (promotionGroupRelation is not null)
            {
                promotionGroupImagesPageUrl = GetPromotionGroupImagesPageUrl(promotionGroupRelation.PromotionGroupId, productXmlOptions?.ApplicationRootPath);

                xmlGroupPromotion = new()
                {
                    VendorName = product.Manufacturer?.RealCompanyName,
                    GroupPromotionsUrl = promotionGroupImagesPageUrl,
                };
            }

            XmlProduct xmlProduct = _productToXmlProductMappingService.MapProductDataToXmlProduct(
                product: product,
                productProperties: relatedProductProperties,
                xmlProductImages: xmlProductImages,
                productSubCategory: productSubCategory,
                searchStringPartInfos: searchStringParts,
                xmlProductPromotions: xmlPromotions,
                xmlGroupPromotion: xmlGroupPromotion);

            if (pricesForProductsInPrefferedCurrency is not null
                && pricesForProductsInPrefferedCurrency.TryGetValue(product, out OneOf<decimal, NotFound> getCurrencyResult)
                && getCurrencyResult.TryPickT0(out decimal priceForPrefferedCurrency, out NotFound _))
            {
                xmlProduct.Price = priceForPrefferedCurrency;
                xmlProduct.CurrencyCode = _productToXmlProductMappingService.GetCurrencyCodeFromCurrency(productXmlOptions!.PrefferedPriceCurrency!.Value);
            }

            xmlProducts.Add(xmlProduct);
        }

        return xmlProducts;
    }

    private async Task<List<XmlProduct>> GetXmlProductsByProductIdsAsync(List<int> productIds, ProductXmlOptions? productXmlOptions = null)
    {
        List<Product> productsWithIds = await _productService.GetByIdsAsync(productIds);

        ProductSearchRequest productSearchRequest = new()
        {
            OnlyVisibleByEndUsers = true,
            ProductStatus = ProductStatusSearchOptions.AvailableAndCall
        };

        List<Product> products = await _productSearchService.SearchProductsAsync(productsWithIds, productSearchRequest);

        Dictionary<int, List<SearchStringPartOriginData>> searchStringOriginDatas
            = await _searchStringOriginService.GetSearchStringPartsAndDataAboutTheirOriginForProductsAsync(products);

        List<IGrouping<int, ProductProperty>> productProperties = await _productPropertyService.GetAllInProductsAsync(productIds);
        List<IGrouping<int, ProductImageData>> productImages = await _productImageService.GetAllInProductsWithoutFileDataAsync(productIds);
        List<IGrouping<int, ProductImageFileData>> productImageFileNameInfos = await _productImageFileService.GetAllInProductsAsync(productIds);

        List<int> subCategoryIds = products.Select(x => x.SubCategoryId)
            .Distinct()
            .Where(x => x.HasValue)
            .Cast<int>()
            .ToList();

        List<SubCategory> subCategories = await _subCategoryService.GetByIdsAsync(subCategoryIds);

        List<IGrouping<int?, Promotion>> promotions = await _promotionService.GetAllForSelectionOfProductsAsync(productIds);

        List<ManufacturerToPromotionGroupRelation> promotionGroupRelations = await _manufacturerToPromotionGroupRelationService.GetAllAsync();

        Dictionary<Product, OneOf<decimal, NotFound>>? pricesForProductsInPrefferedCurrency = null;

        if (productXmlOptions?.PrefferedPriceCurrency is not null)
        {
            pricesForProductsInPrefferedCurrency = await GetChangedCurrenciesForProductsAsync(products, productXmlOptions.PrefferedPriceCurrency.Value);
        }

        List<XmlProduct> xmlProducts = new();

        foreach (Product product in products)
        {
            searchStringOriginDatas.TryGetValue(product.Id, out List<SearchStringPartOriginData>? searchStringOriginData);

            List<XmlSearchStringPartInfo>? searchStringParts = null;

            if (searchStringOriginData is not null)
            {
                searchStringParts = GetSearchStringDataForXmlFromSearchStringData(searchStringOriginData);
            }

            SubCategory? productSubCategory = null;

            if (product.SubCategoryId is not null)
            {
                productSubCategory = subCategories.FirstOrDefault(x => x.Id == product.SubCategoryId);
            }

            List<ProductProperty>? relatedProductProperties = productProperties.FirstOrDefault(x => x.Key == product.Id)?
                .ToList();

            List<ProductImageData>? relatedProductImages = productImages.FirstOrDefault(x => x.Key == product.Id)?
                .ToList();

            List<ProductImageFileData>? relatedProductImageFileNameInfos = productImageFileNameInfos.FirstOrDefault(x => x.Key == product.Id)?
                .ToList();

            List<XmlProductImage> xmlProductImages = GetXmlProductImagesFromProductImages(
                relatedProductImages, relatedProductImageFileNameInfos, productXmlOptions?.ApplicationRootPath);

            IEnumerable<Promotion>? productPromotions = promotions
                .FirstOrDefault(x => x.Key == product.Id)?
                .Where(x =>
                {
                    if (x.Id != product.PromotionPid && x.Id != product.PromotionRid) return false;

                    return (x.StartDate is null || x.StartDate <= DateTime.Now) && (x.ExpirationDate is null || x.ExpirationDate >= DateTime.Now);
                });

            List<XmlPromotion> xmlPromotions = GetXmlPromotionsFromPromotions(product, productPromotions, productXmlOptions);

            XmlGroupPromotion? xmlGroupPromotion = null;

            ManufacturerToPromotionGroupRelation? promotionGroupRelation
                = promotionGroupRelations.FirstOrDefault(x => x.ManufacturerId == product.ManufacturerId);

            string? promotionGroupImagesPageUrl = null;

            if (promotionGroupRelation is not null)
            {
                promotionGroupImagesPageUrl = GetPromotionGroupImagesPageUrl(promotionGroupRelation.PromotionGroupId, productXmlOptions?.ApplicationRootPath);

                xmlGroupPromotion = new()
                {
                    VendorName = product.Manufacturer?.RealCompanyName,
                    GroupPromotionsUrl = promotionGroupImagesPageUrl,
                };
            }

            XmlProduct xmlProduct = _productToXmlProductMappingService.MapProductDataToXmlProduct(
                product: product,
                productProperties: relatedProductProperties,
                xmlProductImages: xmlProductImages,
                productSubCategory: productSubCategory,
                searchStringPartInfos: searchStringParts,
                xmlProductPromotions: xmlPromotions,
                xmlGroupPromotion: xmlGroupPromotion);

            if (pricesForProductsInPrefferedCurrency is not null
                && pricesForProductsInPrefferedCurrency.TryGetValue(product, out OneOf<decimal, NotFound> getCurrencyResult)
                && getCurrencyResult.TryPickT0(out decimal priceForPrefferedCurrency, out NotFound _))
            {
                xmlProduct.Price = priceForPrefferedCurrency;
                xmlProduct.CurrencyCode = _productToXmlProductMappingService.GetCurrencyCodeFromCurrency(productXmlOptions!.PrefferedPriceCurrency!.Value);
            }

            xmlProducts.Add(xmlProduct);
        }

        return xmlProducts;
    }

    private async Task<Dictionary<Product, OneOf<decimal, NotFound>>> GetChangedCurrenciesForProductsAsync(List<Product> products, Currency currency)
    {
        Dictionary<Product, ChangeCurrencyRequest> currencyChangeRequestsFromProducts = products
            .Where(product => product.Price is not null && product.Currency is not null)
            .ToDictionary(
                product => product,
                product => new ChangeCurrencyRequest()
                {
                    CurrentCurrency = product.Currency!.Value,
                    NewCurrency = currency,
                    Value = product.Price!.Value
                });

        Dictionary<ChangeCurrencyRequest, OneOf<decimal, NotFound>>? currencyExchangeResults
            = await _currencyConversionService.ChangeCurrenciesAsync(currencyChangeRequestsFromProducts.Values.ToList());

        Dictionary<Product, OneOf<decimal, NotFound>> currenciesForProducts = new(currencyChangeRequestsFromProducts.Count);

        foreach (KeyValuePair<Product, ChangeCurrencyRequest> productToCurrencyChangeRequest in currencyChangeRequestsFromProducts)
        {
            if (currencyExchangeResults.TryGetValue(productToCurrencyChangeRequest.Value, out var val))
            {
                currenciesForProducts[productToCurrencyChangeRequest.Key] = val;
            }
        }

        return currenciesForProducts;
    }

    private static List<XmlProductImage> GetXmlProductImagesFromProductImages(
       List<ProductImageData>? productImages = null,
       List<ProductImageFileData>? productImageFileNameInfos = null,
       string? applicationRootPath = null)
    {
        List<XmlProductImage> output = new();

        if (productImages is null && productImageFileNameInfos is null) return output;

        if (productImageFileNameInfos is not null)
        {
            foreach (ProductImageFileData productImageFileNameInfo in productImageFileNameInfos)
            {
                if (productImageFileNameInfo.FileName is null) continue;

                XmlProductImage xmlImage = GetXmlImageFromProductImageFileName(
                    productImageFileNameInfo.FileName, applicationRootPath);

                output.Add(xmlImage);
            }
        }

        return output;
    }

    private static XmlProductImage GetXmlImageFromProductImageFileName(string fileName, string? applicationRootPath = null)
    {
        return new()
        {
            PictureUrl = CombinePathsWithSeparator(
                '/', applicationRootPath ?? string.Empty, ProductImageFileDataEndpoints.EndpointGroupRoute, fileName),
        };
    }

    private static List<XmlPromotion> GetXmlPromotionsFromPromotions(
        Product product,
        IEnumerable<Promotion>? promotions = null,
        ProductXmlOptions? productXmlOptions = null)
    {
        List<XmlPromotion> output = new();

        int? promotionPictureId = product.AlertPictureId;

        if (promotionPictureId is null || promotionPictureId <= 0)
        {
            promotionPictureId = product.PromotionPictureId;
        }

        bool isInfoPromotionActive = (product.AlertExpireDate is null || product.AlertExpireDate >= DateTime.Now);

        if (promotionPictureId > 0 && isInfoPromotionActive)
        {
            string? promotionPictureUrl = GetPromotionPictureUrl(product, productXmlOptions?.ApplicationRootPath);

            XmlPromotion xmlPromotion = new()
            {
                Type = "Info",
                ValidFrom = null,
                ValidTo = product.AlertExpireDate,
                Info = GetInfoPromotionDataStringFromPictureId(promotionPictureId),
                PictureUrl = promotionPictureUrl
            };

            output.Add(xmlPromotion);
        }

        if (promotions is null) return output;

        foreach (Promotion promotion in promotions)
        {
            XmlPromotion xmlPromotion = new()
            {
                Type = GetPromotionTypeAsXmlDisplayString(promotion.Type),
                ValidFrom = promotion.StartDate,
                ValidTo = promotion.ExpirationDate,
                PromotionUSD = promotion.DiscountUSD,
                PromotionEUR = promotion.DiscountEUR,
            };

            output.Add(xmlPromotion);
        }

        return output;
    }

    private static string? GetPromotionTypeAsXmlDisplayString(short? promotionType)
    {
        if (promotionType is null) return null;

        return promotionType switch
        {
            1 => _pidPromotionTypeInXml,
            2 => _ridPromotionTypeInXml,
            _ => null,
        };
    }

    private static string? GetInfoPromotionDataStringFromPictureId(int? promotionPictureId)
    {
        if (promotionPictureId is null) return null;

        bool exists = _promotionImageIdToInfoStringInXml.TryGetValue(promotionPictureId.Value, out string? infoString);

        return exists ? infoString : null;
    }


    private static string? GetPromotionPictureUrl(Product product, string? applicationRootPath)
    {
        if (product.AlertExpireDate is not null && product.AlertExpireDate < DateTime.Now) return null;

        int? promotionPictureId = product.AlertPictureId;

        if (promotionPictureId is null || promotionPictureId <= 0)
        {
            promotionPictureId = product.PromotionPictureId;
        }

        if (promotionPictureId is null || promotionPictureId <= 0) return null;

        string? promotionPictureSource = promotionPictureId switch
        {
            1 => "/img/ProductInfo/PL_1.gif",
            2 => "/img/ProductInfo/PL_2.gif",
            3 => "/img/ProductInfo/PL_3.gif",
            4 => "/img/ProductInfo/PL_4.gif",
            5 => "/img/ProductInfo/PL_5.gif",
            6 => "/img/ProductInfo/PL_6.gif",
            7 => "/img/ProductInfo/PL_7.gif",
            8 => "/img/ProductInfo/PL_8.gif",
            9 => "/img/ProductInfo/PL_9.gif",
            10 => "/img/ProductInfo/PL_10.gif",
            11 => "/img/ProductInfo/PL_11.gif",
            12 => "/img/ProductInfo/PL_12.gif",
            _ => null
        };

        if (promotionPictureSource is null) return null;

        return CombinePathsWithSeparator('/', applicationRootPath ?? string.Empty, promotionPictureSource);
    }

    private static string? GetPromotionGroupImagesPageUrl(int promotionGroupId, string? applicationRootPath)
    {
        if (string.IsNullOrEmpty(applicationRootPath)) return null;

        return CombinePathsWithSeparator('/', applicationRootPath, "promotionGroupImages", promotionGroupId.ToString());
    }

    private static List<XmlSearchStringPartInfo> GetSearchStringDataForXmlFromSearchStringData(List<SearchStringPartOriginData> searchStringOriginData)
    {
        List<XmlSearchStringPartInfo> output = new();

        foreach (SearchStringPartOriginData searchStringPartOriginData in searchStringOriginData)
        {
            string? description = searchStringPartOriginData.Characteristic.Meaning;

            XmlSearchStringPartInfo searchStringPartInfo = new()
            {
                Name = searchStringPartOriginData.SearchStringPart,
                Description = description,
            };

            output.Add(searchStringPartInfo);
        }

        return output;
    }
}