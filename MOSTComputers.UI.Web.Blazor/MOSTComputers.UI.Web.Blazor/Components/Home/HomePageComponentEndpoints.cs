using Microsoft.AspNetCore.Components.Endpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using MOSTComputers.Models.Common;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.Promotions;
using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Services.Currencies.Contracts;
using MOSTComputers.Services.Currencies.Models;
using MOSTComputers.Services.PDF.Models.HomeOffers;
using MOSTComputers.Services.PDF.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductProperties.Contacts;
using MOSTComputers.Services.ProductRegister.Services.Products.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;
using MOSTComputers.Services.SearchStringOrigin.Models;
using MOSTComputers.Services.SearchStringOrigin.Services.Contracts;
using MOSTComputers.UI.Web.Blazor.Components.Primitives;
using MOSTComputers.UI.Web.Blazor.Endpoints;
using MOSTComputers.UI.Web.Blazor.Endpoints.Images;
using MOSTComputers.UI.Web.Blazor.Utils;
using OneOf;
using static MOSTComputers.UI.Web.Blazor.Components.Pages.Home;
using static MOSTComputers.Utils.Files.FilePathUtils;

namespace MOSTComputers.UI.Web.Blazor.Components.Home;

public static class HomePageComponentEndpoints
{
    public sealed class ProductSearchData
    {
        public string? SearchData { get; set; }
        public int? CategoryId { get; set; }
        public int? ManufacturerId { get; set; }
        public HomeProductStatusSearchOptions? ProductStatusSearchOptions { get; set; } = null;
        public PromotionSearchOptions? PromotionSearchOptions { get; set; } = null;
        public Currency? Currency { get; set; } = null;
        public int? MaxSearchResults { get; set; } = null;
    } 

    private sealed class ProductOrderSummariesRequest
    {
        public List<ProductOrderSummaryRequest>? ProductQuantities { get; set; }
        public Currency? Currency { get; set; } = null;
        public bool ShowEntireTable { get; set; } = false;
    }

    private sealed class ProductOrderSummaryRequest
    {
        public int Id { get; init; }
        public int Quantity { get; init; }
    }

    private sealed class ManufacturerSearchResponse
    {
        public int? id { get; set; }
        public string? displayName { get; set; }
    }

    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "components/" + "home";

    private const int _maxProductsSearchCount = 2000;
    private const int _maxProductOrderSummariesCount = 2000;

    public static IEndpointConventionBuilder MapProductDataComponentEndpoints(
        this IEndpointRouteBuilder endpoints,
        RazorComponentsEndpointConventionBuilder razorEndpointBuilder)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute);

        endpointGroup.MapGet("/search/manufacturers/{categoryId:int?}", GetManufacturersForCategoryAsync);

        endpointGroup.MapGet("/productXmlDataPopup/{productId:int}", GetProductXmlPopupDataAsync);
        endpointGroup.MapGet("/productDataPopup/{productId:int}", GetProductDataPopupAsync);
        endpointGroup.MapGet("/productGroupPromotionPopup/{promotionGroupId:int}", GetProductGroupPromotionPopupAsync);
        endpointGroup.MapGet("/productSearchStringPopup/{productId:int}", GetProductSearchStringPartsPopupAsync);

        endpointGroup.MapPost("/search", GetProductTableForSearchedProductsAsync);
        endpointGroup.MapPost("/orderSummary", GetOrderSummaryForProductsAsync);

        ConfigureHomeOfferPageEndpoint(razorEndpointBuilder);

        endpointGroup.MapPost("/offer/pdf", GetPdfFromHomeOfferPageDataAsync);

        return endpointGroup;
    }

    private static async Task<IResult> GetProductTableForSearchedProductsAsync(
        HttpContext httpContext,
        [FromBody] ProductSearchData productSearchData,
        [FromServices] IProductService productService,
        [FromServices] IProductSearchService productSearchService,
        [FromServices] IProductImageFileService productImageFileService,
        [FromServices] IPromotionService promotionService,
        [FromServices] IManufacturerToPromotionGroupRelationService manufacturerToPromotionGroupRelationService,
        [FromServices] ICurrencyConversionService currencyConversionService)
    {
        httpContext.Response.ContentType = "application/html";
        httpContext.Response.Headers.TryAdd("Content-Disposition", "inline; filename=data.xml");

        List<ProductDisplayData> productDisplayDatas = await SearchProductsAsync(
            productService, productSearchService, productSearchData);

        Currency currency = productSearchData.Currency ?? Currency.EUR;

        Currency? secondaryCurrency = null;

        if (currency == Currency.EUR)
        {
            secondaryCurrency = Currency.BGN;
        }

        await AddCurrenciesToProductDataAsync(currencyConversionService, productDisplayDatas, currency, secondaryCurrency);

        await AddActivePromotionsToProductDataAsync(promotionService, productDisplayDatas);

        await AddImageUrlsToProductDataAsync(productImageFileService, productDisplayDatas);

        await AddManufacturerGroupPromotionIdToProductDataAsync(manufacturerToPromotionGroupRelationService, productDisplayDatas);

        return new RazorComponentResult<ProductDatasTable>(new
        {
            ProductDisplayDatas = productDisplayDatas,
            ShowImages = false
        });
    }

    private static async Task<List<ProductDisplayData>> SearchProductsAsync(
        IProductService productService,
        IProductSearchService productSearchService,
        ProductSearchData productSearchData)
    {
        int selectedResultCount = Math.Min(productSearchData.MaxSearchResults ?? int.MaxValue, _maxProductsSearchCount);
        int? finalResultCount = selectedResultCount;

        bool orderProductsFromSecondCategoryUp = productSearchData.CategoryId == null
            && productSearchData.ManufacturerId == null
            && string.IsNullOrWhiteSpace(productSearchData.SearchData);

        if (orderProductsFromSecondCategoryUp)
        {
            finalResultCount = null;
        }

        List<MOSTComputers.Models.Product.Models.Product> products = new();

        if (int.TryParse(productSearchData.SearchData, out int productId))
        {
            MOSTComputers.Models.Product.Models.Product? product = await productService.GetByIdAsync(productId);

            if (product is not null && product.PlShow != 2 && product.Status != ProductStatus.Unavailable)
            {
                products.Add(product);
            }
        }

        ProductStatusSearchOptions productStatusSearchOptions = productSearchData.ProductStatusSearchOptions switch
        {
            HomeProductStatusSearchOptions.Available => ProductStatusSearchOptions.Available,
            HomeProductStatusSearchOptions.Call => ProductStatusSearchOptions.Call,
            HomeProductStatusSearchOptions.AvailableAndCall => ProductStatusSearchOptions.AvailableAndCall,
            _ => ProductStatusSearchOptions.AvailableAndCall
        };

        if (products.Count <= 0)
        {
            ProductSearchRequest productSearchRequest = new()
            {
                OnlyVisibleByEndUsers = true,
                UserInputString = productSearchData.SearchData,
                CategoryId = productSearchData.CategoryId,
                ManufacturerId = productSearchData.ManufacturerId,
                ProductStatus = productStatusSearchOptions,
                PromotionSearchOptions = productSearchData.PromotionSearchOptions,
                MaxResultCount = finalResultCount,
            };

            List<MOSTComputers.Models.Product.Models.Product> searchedProducts
                = await productSearchService.SearchProductsAsync(productSearchRequest);

           products.AddRange(searchedProducts);
        }

        if (orderProductsFromSecondCategoryUp)
        {
            SortProductsFromSecondCategoryUp(products);
        }

        int productsToDisplayCount = Math.Min(selectedResultCount, products.Count);

        products = products[..productsToDisplayCount];

        List<ProductDisplayData> productDisplayDatas = [];

        foreach (MOSTComputers.Models.Product.Models.Product product in products)
        {
            ProductDisplayData productData = new()
            {
                Product = product,
            };

            productDisplayDatas.Add(productData);
        }

        return productDisplayDatas;
    }

    private static async Task<IResult> GetOrderSummaryForProductsAsync(
        HttpContext httpContext,
        [FromBody] ProductOrderSummariesRequest request,
        [FromServices] IProductService productService,
        [FromServices] IProductImageFileService productImageFileService,
        [FromServices] IPromotionService promotionService,
        [FromServices] IManufacturerToPromotionGroupRelationService manufacturerToPromotionGroupRelationService,
        [FromServices] ICurrencyConversionService currencyConversionService)
    {
        List<ProductOrderSummaryRequest>? productQuantities = request.ProductQuantities;

        if (productQuantities == null
            || productQuantities.Count > _maxProductOrderSummariesCount)
        {
            return Results.Problem(statusCode: StatusCodes.Status400BadRequest);
        }

        List<int> productIds = [];

        foreach (ProductOrderSummaryRequest productQuantity in productQuantities)
        {
            productIds.Add(productQuantity.Id);
        }

        List<MOSTComputers.Models.Product.Models.Product> products = await productService.GetByIdsAsync(productIds);

        List<ProductDisplayData> productDisplayDatas = [];

        List<MOSTComputers.Models.Product.Models.Product> unavailableProducts = [];

        foreach (MOSTComputers.Models.Product.Models.Product product in products)
        {
            if (product.PlShow == 2 || product.Status == ProductStatus.Unavailable)
            {
                unavailableProducts.Add(product);

                continue;
            }

            ProductDisplayData productData = new()
            {
                Product = product,
                IsExternalOrderSummaryItem = true,
            };

            productDisplayDatas.Add(productData);
        }

        Currency currency = request.Currency ?? Currency.EUR;

        Currency? secondaryCurrency = null;

        if (currency == Currency.EUR)
        {
            secondaryCurrency = Currency.BGN;
        }

        await AddCurrenciesToProductDataAsync(currencyConversionService, productDisplayDatas, currency, secondaryCurrency);

        await AddActivePromotionsToProductDataAsync(promotionService, productDisplayDatas);

        await AddImageUrlsToProductDataAsync(productImageFileService, productDisplayDatas);

        await AddManufacturerGroupPromotionIdToProductDataAsync(manufacturerToPromotionGroupRelationService, productDisplayDatas);

        if (request.ShowEntireTable)
        {
            return new RazorComponentResult<ProductDatasTable>(new
            {
                ProductDisplayDatas = productDisplayDatas,
                UnavailableProducts = unavailableProducts,
                ShowImages = false
            });
        }

        return new RazorComponentResult<ProductDatasTableItems>(new
        {
            ProductDisplayDatas = productDisplayDatas,
            UnavailableProducts = unavailableProducts,
            ShowImages = false
        });
    }

    private static async Task AddCurrenciesToProductDataAsync(
        ICurrencyConversionService currencyConversionService,
        List<ProductDisplayData> productDisplayDatas,
        Currency currency,
        Currency? secondaryCurrency = null)
    {
        Dictionary<ProductDisplayData, ChangeCurrencyRequest> currencyChangeRequests = productDisplayDatas
            .Where(productData => productData.Product.Price.HasValue && productData.Product.Currency.HasValue)
            .ToDictionary(product => product, productData =>
            {
                return new ChangeCurrencyRequest()
                {
                    Value = productData.Product.Price!.Value,
                    CurrentCurrency = productData.Product.Currency!.Value,
                    NewCurrency = currency
                };
            });

        Dictionary<ProductDisplayData, ChangeCurrencyRequest>? secondaryChangeRequests = null;

        if (secondaryCurrency is not null)
        {
            secondaryChangeRequests = productDisplayDatas
                .Where(productData => productData.Product.Price.HasValue && productData.Product.Currency.HasValue)
                .ToDictionary(product => product, productData =>
                {
                    return new ChangeCurrencyRequest()
                    {
                        Value = productData.Product.Price!.Value,
                        CurrentCurrency = productData.Product.Currency!.Value,
                        NewCurrency = secondaryCurrency.Value,
                    };
                });
        }

        Dictionary<ChangeCurrencyRequest, OneOf<decimal, OneOf.Types.NotFound>> currencyChangeResults
            = await currencyConversionService.ChangeCurrenciesAsync(currencyChangeRequests.Values.ToList());

        Dictionary<ChangeCurrencyRequest, OneOf<decimal, OneOf.Types.NotFound>>? secondaryCurrencyChangeResults = null;

        if (secondaryChangeRequests is not null)
        {
            secondaryCurrencyChangeResults = await currencyConversionService.ChangeCurrenciesAsync(secondaryChangeRequests.Values.ToList());
        }

        IEnumerable<int> productIds = productDisplayDatas.Select(x => x.Product.Id);

        foreach (ProductDisplayData productDisplayData in productDisplayDatas)
        {
            decimal? displayPrice = null;
            decimal? secondaryDisplayPrice = null;

            if (productDisplayData.Product.Price is not null && productDisplayData.Product.Currency is not null)
            {
                OneOf<decimal, OneOf.Types.NotFound>? getPriceResult = currencyChangeResults.TryGetValue(
                    currencyChangeRequests[productDisplayData], out OneOf<decimal, OneOf.Types.NotFound> result)
                        ? result
                        : null;

                if (getPriceResult is not null && getPriceResult.Value.IsT0)
                {
                    displayPrice = getPriceResult.Value.AsT0;
                }

                if (secondaryChangeRequests is not null && secondaryCurrencyChangeResults is not null)
                {
                    OneOf<decimal, OneOf.Types.NotFound>? getSecondaryPriceResult = secondaryCurrencyChangeResults.TryGetValue(
                        secondaryChangeRequests[productDisplayData], out OneOf<decimal, OneOf.Types.NotFound> secondaryResult)
                            ? secondaryResult
                            : null;

                    if (getSecondaryPriceResult is not null && getSecondaryPriceResult.Value.IsT0)
                    {
                        secondaryDisplayPrice = getSecondaryPriceResult.Value.AsT0;
                    }
                }
            }

            if (displayPrice is not null)
            {
                productDisplayData.DisplayPrice = new() { Price = displayPrice.Value, Currency = currency };
            }

            if (secondaryDisplayPrice is not null)
            {
                productDisplayData.SecondaryDisplayPrice = new() { Price = secondaryDisplayPrice.Value, Currency = secondaryCurrency!.Value };
            }
        }
    }

    private static async Task AddActivePromotionsToProductDataAsync(
        IPromotionService promotionService,
        List<ProductDisplayData> productDisplayDatas)
    {
        IEnumerable<int> productIds = productDisplayDatas.Select(x => x.Product.Id);

        List<IGrouping<int?, Promotion>> promotions = await promotionService.GetAllActiveForSelectionOfProductsAsync(productIds);

        foreach (ProductDisplayData productDisplayData in productDisplayDatas)
        {
            List<Promotion>? activePromotions = promotions
                .FirstOrDefault(x => x.Key == productDisplayData.Product.Id)?
                .Where(x =>
                {
                    if (x.Id != productDisplayData.Product.PromotionPid && x.Id != productDisplayData.Product.PromotionRid) return false;

                    return (x.StartDate is null || x.StartDate <= DateTime.Now)
                        && (x.ExpirationDate is null || x.ExpirationDate >= DateTime.Now);
                })
                .ToList();

            productDisplayData.Promotions = activePromotions;
        }
    }

    private static async Task AddImageUrlsToProductDataAsync(
        IProductImageFileService productImageFileService,
        List<ProductDisplayData> productDisplayDatas)
    {
        IEnumerable<int> productIds = productDisplayDatas.Select(x => x.Product.Id);

        List<IGrouping<int, ProductImageFileData>> imageFiles = await productImageFileService.GetAllInProductsAsync(productIds);

        foreach (IGrouping<int, ProductImageFileData> imageFilesForProduct in imageFiles)
        {
            ProductDisplayData productData = productDisplayDatas.First(x => x.Product.Id == imageFilesForProduct.Key);

            productData.ImageUrls ??= new();

            foreach (ProductImageFileData imageFile in imageFilesForProduct)
            {
                if (string.IsNullOrWhiteSpace(imageFile.FileName)) continue;

                string imageFileUrl = UrlUtils.JoinUrlPath(ProductImageFileDataEndpoints.EndpointGroupRoute, imageFile.FileName);

                productData.ImageUrls.Add('/' + imageFileUrl);
            }
        }
    }

    private static async Task AddManufacturerGroupPromotionIdToProductDataAsync(
        IManufacturerToPromotionGroupRelationService manufacturerToPromotionGroupRelationService,
        List<ProductDisplayData> productDisplayDatas)
    {
        IEnumerable<int> manufacturerIds = productDisplayDatas
            .Where(x => x.Product.ManufacturerId is not null)
            .Select(x => (int)x.Product.ManufacturerId!);

        List<ManufacturerToPromotionGroupRelation> relations = await manufacturerToPromotionGroupRelationService.GetAllAsync();

        IEnumerable<ManufacturerToPromotionGroupRelation> matchingRelations = relations
            .Where(x => manufacturerIds.Contains(x.ManufacturerId));

        foreach (ProductDisplayData productDisplayData in productDisplayDatas)
        {
            ManufacturerToPromotionGroupRelation? matchingRelation = matchingRelations.FirstOrDefault(
                x => x.ManufacturerId == productDisplayData.Product.ManufacturerId);

            if (matchingRelation is null) continue;

            productDisplayData.ManufacturerPromotionGroupId = matchingRelation.PromotionGroupId;
        }
    }

    private static async Task<IResult> GetManufacturersForCategoryAsync(
        [FromServices] IManufacturerService manufacturerService,
        [FromServices] IProductSearchService productSearchService,
        [FromRoute] int? categoryId)
    {
        List<Manufacturer> manufacturers;

        if (categoryId.HasValue)
        {
            ProductSearchRequest productSearchRequest = new()
            {
                OnlyVisibleByEndUsers = true,
                CategoryId = categoryId.Value,
                ProductStatus = ProductStatusSearchOptions.AvailableAndCall,
            };

            List<MOSTComputers.Models.Product.Models.Product> visibleProductsInCategory
                = await productSearchService.SearchProductsAsync(productSearchRequest);

            List<Manufacturer> manufacturersInProducts = new();

            foreach (MOSTComputers.Models.Product.Models.Product product in visibleProductsInCategory)
            {
                if (product.Manufacturer is null || manufacturersInProducts.Any(x => x.Id == product.Manufacturer.Id)) continue;

                manufacturersInProducts.Add(product.Manufacturer);
            }

            manufacturers = manufacturersInProducts;
        }
        else
        {
            manufacturers =  await manufacturerService.GetAllAsync();
        }

        IEnumerable<ManufacturerSearchResponse> manufacturerSearchResponses = manufacturers
            .Select(x => new ManufacturerSearchResponse()
            {
                id = x.Id,
                displayName = x.RealCompanyName,
            });

        JsonResult result = new(manufacturerSearchResponses)
        {
            StatusCode = 200
        };

        return Results.Json(manufacturerSearchResponses, statusCode: 200, contentType: "application/json");
    }

    private static async Task<IResult> GetProductXmlPopupDataAsync(
        [FromServices] IProductService productService,
        int productId)
    {
        MOSTComputers.Models.Product.Models.Product? product = await productService.GetByIdAsync(productId);

        if (product == null)
        {
            return Results.NotFound();
        }

        return new RazorComponentResult<ProductXmlDataPopup>(new
        {
            ProductXmlPopupData = new ProductXmlPopupData()
            {
                IsVisible = true,
                Product = product,
            }
        });
    }

    private static async Task<IResult> GetProductDataPopupAsync(
        HttpContext httpContext,
        [FromServices] IProductService productService,
        [FromServices] IProductCharacteristicService productCharacteristicService,
        [FromServices] IProductPropertyService productPropertyService,
        [FromServices] IProductImageFileService productImageFileService,
        [FromServices] IPromotionService promotionService,
        [FromServices] IGroupPromotionProductBindingsService groupPromotionProductBindingsService,
        [FromServices] IGroupPromotionService groupPromotionService,
        [FromServices] IGroupPromotionImageFileDataService groupPromotionImageFileDataService,
        [FromServices] ISearchStringOriginService searchStringOriginService,
        [FromServices] ICurrencyConversionService currencyConversionService,
        [FromServices] ICurrencyVATService currencyVATService,
        int productId)
    {
        MOSTComputers.Models.Product.Models.Product? product = await productService.GetByIdAsync(productId);

        if (product == null)
        {
            return Results.NotFound();
        } 

        List<Product.ProductData.ProductPropertyDisplayData> propertiesInData
            = await GetProductPropertiesFromProductDataAsync(productId, product.CategoryId, productCharacteristicService, productPropertyService);

        List<Product.ProductData.ProductImageDisplayData> productImagesInData = new();

        List<ProductImageFileData> productImageFileNameInfos = await productImageFileService.GetAllInProductAsync(product.Id);

        foreach (ProductImageFileData imageFileNameInfo in productImageFileNameInfos)
        {
            if (imageFileNameInfo.FileName is null) continue;

            Product.ProductData.ProductImageDisplayData productImageDisplayData = new()
            {
                ImageSrc = $"/api/images/imageFileData/{imageFileNameInfo.FileName}",
            };

            productImagesInData.Add(productImageDisplayData);
        }

        List<int> groupPromotionIds = await groupPromotionProductBindingsService.GetAllPromotionIdsBoundToProductAsync(productId);

        List<Product.ProductData.ProductRelatedGroupPromotionDisplayData> productRelatedGroupPromotions = new();

        if (groupPromotionIds.Count > 0)
        {
            List<GroupPromotionContent> groupPromotionContents = await groupPromotionService.GetByIdsAsync(groupPromotionIds);

            List<IGrouping<int, GroupPromotionImageFileData>> promotionImageFiles
                = await groupPromotionImageFileDataService.GetAllInPromotionsAsync(groupPromotionIds);

            for (int i = 0; i < groupPromotionContents.Count; i++)
            {
                GroupPromotionContent groupPromotionContent = groupPromotionContents[i];

                IGrouping<int, GroupPromotionImageFileData>? imageFilesForPromotion = promotionImageFiles.Find(x => x.Key == groupPromotionContent.Id);

                string? htmlContent = groupPromotionContent.HtmlContent;

                if (string.IsNullOrEmpty(htmlContent)) continue;

                string? modifiedPromotionHtmlContent = groupPromotionService.ChangeLegacyUrlsToNewOnes(
                    groupPromotionContent.HtmlContent,
                    imageFilesForPromotion,
                    promotionImageFile =>
                    {
                        string imageUrl = CombinePathsWithSeparator(
                        '/', GroupPromotionImageFileEndpoints.EndpointGroupRoute, promotionImageFile.Id.ToString());

                        return QueryHelpers.AddQueryString(imageUrl, "v", promotionImageFile.Version.ToString());
                    });

                if (string.IsNullOrEmpty(modifiedPromotionHtmlContent)) continue;

                Product.ProductData.ProductRelatedGroupPromotionDisplayData productPromotionDisplayData = new()
                {
                    HtmlContent = modifiedPromotionHtmlContent,
                };

                productRelatedGroupPromotions.Add(productPromotionDisplayData);
            }
        }

        List<MOSTComputers.Models.Product.Models.Promotions.Promotion> promotions
            = await promotionService.GetAllForProductAsync(product.Id);

        List<MOSTComputers.Models.Product.Models.Promotions.Promotion> productPromotions = promotions.Where(x =>
        {
            if (x.Id != product.PromotionPid && x.Id != product.PromotionRid) return false;

            return (x.StartDate is null || x.StartDate <= DateTime.Now)
                && (x.ExpirationDate is null || x.ExpirationDate >= DateTime.Now);
        })
            .ToList();

        List<SearchStringPartOriginData>? productSearchStringParts = await searchStringOriginService.GetSearchStringPartsAndDataAboutTheirOriginAsync(
            product.SearchString, product.CategoryId);

        Product.ProductData.ProductPriceData? productPriceData = await GetProductPriceDataAsync(
            currencyConversionService, currencyVATService, product);

        HttpRequest httpRequest = httpContext.Request;

        string baseUrl = $"{httpRequest.Scheme}://{httpRequest.Host}{httpRequest.PathBase}/";

        return new RazorComponentResult<ProductDataPopup>(new
        {
            ProductDataPopupData = new ProductDataPopupData()
            {
                IsVisible = true,
                Product = product,
                BaseProductPagePath = baseUrl,
                ProductDataPopupPriceData = productPriceData,
                ProductProperties = propertiesInData,
                ProductImages = productImagesInData,
                ProductRelatedGroupPromotions = productRelatedGroupPromotions,
                Promotions = productPromotions,
                ProductSearchStringParts = productSearchStringParts,
            }
        });
    }

    private static async Task<List<Product.ProductData.ProductPropertyDisplayData>> GetProductPropertiesFromProductDataAsync(
        int productId,
        int? productCategoryId,
        IProductCharacteristicService productCharacteristicService,
        IProductPropertyService productPropertyService)
    {
        List<int> relatedCategoryIds = [-1];

        if (productCategoryId is not null)
        {
            relatedCategoryIds.Add(productCategoryId.Value);
        }

        IEnumerable<ProductCharacteristicType> productCharacteristicTypes = [
            ProductCharacteristicType.ProductCharacteristic,
            ProductCharacteristicType.Link
        ];

        List<ProductCharacteristic> productCharacteristics = await productCharacteristicService.GetAllByCategoryIdsAndTypesAsync(
            relatedCategoryIds, productCharacteristicTypes, true);

        List<ProductProperty> productProperties = await productPropertyService.GetAllInProductAsync(productId);

        List<Product.ProductData.ProductPropertyDisplayData> propertiesInData = new();

        foreach (ProductProperty property in productProperties)
        {
            ProductCharacteristic? productCharacteristic = productCharacteristics
                .FirstOrDefault(characteristic => characteristic.Id == property.ProductCharacteristicId);

            if (productCharacteristic == null) continue;

            Product.ProductData.ProductPropertyDisplayData mappedProperty = new()
            {
                ProductCharacteristic = new()
                {
                    Id = productCharacteristic.Id,
                    CategoryId = productCharacteristic.CategoryId,
                    Active = productCharacteristic.Active,
                    KWPrCh = productCharacteristic.KWPrCh
                },
                Name = property.Characteristic,
                Value = property.Value,
                DisplayOrder = property.DisplayOrder
            };

            propertiesInData.Add(mappedProperty);
        }

        return propertiesInData;
    }

    private static async Task<Product.ProductData.ProductPriceData?> GetProductPriceDataAsync(
        ICurrencyConversionService currencyConversionService,
        ICurrencyVATService currencyVATService,
        MOSTComputers.Models.Product.Models.Product product)
    {
        Product.ProductData.DisplayPrice? displayPrice = null;
        Product.ProductData.DisplayPrice? secondaryDisplayPrice = null;

       if (product.Price is not null && product.Currency is not null)
        {
            OneOf<decimal, OneOf.Types.NotFound> getPriceInEurResult = await currencyConversionService.ChangeCurrencyAsync(
                product.Price.Value, product.Currency.Value, Currency.EUR);

            OneOf<decimal, OneOf.Types.NotFound> getPriceInBgnResult = await currencyConversionService.ChangeCurrencyAsync(
                product.Price.Value, product.Currency.Value, Currency.BGN);

            if (getPriceInEurResult.IsT0)
            {
                decimal priceInEur = currencyVATService.AddVATToPriceUsingDefaultRate(getPriceInEurResult.AsT0, 1);

                displayPrice = new()
                {
                    Amount = priceInEur,
                    Currency = Currency.EUR,
                };
            }

            if (getPriceInEurResult.IsT0)
            {
                decimal priceInBgn = currencyVATService.AddVATToPriceUsingDefaultRate(getPriceInBgnResult.AsT0, 1);

                secondaryDisplayPrice = new()
                {
                    Amount = priceInBgn,
                    Currency = Currency.BGN,
                };
            }
        }

        if (displayPrice is null) return null;

        return new()
        {
            DisplayPrice = displayPrice.Value,
            SecondaryDisplayPrice = secondaryDisplayPrice,
        };
    }

    private static async Task<IResult> GetProductGroupPromotionPopupAsync(
        [FromServices] IProductService productService,
        [FromServices] IGroupPromotionService groupPromotionService,
        [FromRoute] int promotionGroupId,
        [FromQuery] int productId)
    {
        MOSTComputers.Models.Product.Models.Product? product = await productService.GetByIdAsync(productId);

        if (product == null)
        {
            return Results.NotFound();
        }

        List<GroupPromotionContent> promotions = await groupPromotionService.GetAllActiveInGroupAsync(promotionGroupId);

        DateTime now = DateTime.Now;

        bool anyNotExpiredPromotions = promotions.Any(x =>
        {
            return (x.StartDate == null || x.StartDate <= now)
                && (x.ExpirationDate == null || x.ExpirationDate >= DateTime.Now);
        });

        return new RazorComponentResult<PromotionGroupImagesPopup>(new
        {
            PromotionGroupPromotionPopupData = new PromotionGroupImagesPopupData()
            {
                IsVisible = true,
                Product = product,
                PromotionGroupIdToDisplay = anyNotExpiredPromotions ? promotionGroupId : null,
            }
        });
    }

    private static async Task<IResult> GetProductSearchStringPartsPopupAsync(
        [FromServices] IProductService productService,
        [FromServices] ISearchStringOriginService searchStringOriginService,
        [FromRoute] int productId)
    {
        MOSTComputers.Models.Product.Models.Product? product = await productService.GetByIdAsync(productId);

        if (product == null)
        {
            return Results.NotFound();
        }

        List<SearchStringPartOriginData>? searchStringParts
            = await searchStringOriginService.GetSearchStringPartsAndDataAboutTheirOriginAsync(
            product.SearchString, product.CategoryId);

        return new RazorComponentResult<ProductSearchStringPartsPopup>(new
        {
            ProductSearchStringPartsPopupData = new ProductSearchStringPartsPopupData()
            {
                IsVisible = true,
                Product = product,
                SearchStringOriginDataList = searchStringParts ?? new()
            },
            PopupMarginOptions = new Popup.MarginOptions() { Left = "15vw", Top = "15vh", Right = "15vw", Bottom = "15vh" }
        });
    }

    public static RazorComponentsEndpointConventionBuilder ConfigureHomeOfferPageEndpoint(
        this RazorComponentsEndpointConventionBuilder builder)
    {
        builder.Add(endpointBuilder =>
        {
            bool isHomeOfferPage = false;

            foreach (object metadata in endpointBuilder.Metadata)
            {
                if (metadata is ComponentTypeMetadata componentTypeMetadata
                    && componentTypeMetadata.Type == typeof(HomeOffer))
                {
                    isHomeOfferPage = true;
                    break;
                }
            }

            if (!isHomeOfferPage)
            {
                return;
            }

            RequestDelegate originalRequestDelegate = endpointBuilder.RequestDelegate!;

            endpointBuilder.RequestDelegate = async context =>
            {
                if (HttpMethods.IsPost(context.Request.Method))
                {
                    IFormCollection form = await context.Request.ReadFormAsync();

                    context.Items[HomeOffer.OfferItemsKey] = form["payload"].ToString();

                    string originalMethod = context.Request.Method;

                    context.Request.Method = HttpMethods.Get;

                    try
                    {
                        await originalRequestDelegate(context);
                    }
                    finally
                    {
                        context.Request.Method = originalMethod;
                    }
                }
                else
                {
                    await originalRequestDelegate(context);
                }
            };
        });

        return builder;
    }

    private sealed class HomeOfferPdfRequest
    {
        public int? OfferNumber { get; init; }
        public string? AdditionalInfo { get; init; }
        public List<HomeOfferPdfRequestItem> Items { get; init; } = [];
    }

    private sealed class HomeOfferPdfRequestItem
    {
        public required int Id { get; init; }
        public required int Quantity { get; init; }
        public required decimal PricePerItemEur { get; init; }
    }

    private static async Task<IResult> GetPdfFromHomeOfferPageDataAsync(
        [FromBody] HomeOfferPdfRequest request,
        [FromServices] IProductService productService,
        [FromServices] ICurrencyVATPercentageProvider currencyVATPercentageProvider,
        [FromServices] ICurrencyVATService currencyVATService,
        [FromServices] IPdfHomeOfferFileGeneratorService pdfHomeOfferFileGeneratorService)
    {
        List<int> productIds = [];

        foreach (HomeOfferPdfRequestItem item in request.Items)
        {
            productIds.Add(item.Id);
        }

        List<MOSTComputers.Models.Product.Models.Product> products = await productService.GetByIdsAsync(productIds); 

        List<HomeOfferItemData> homeOfferItems = [];

        decimal varPercentageFraction = currencyVATPercentageProvider.GetDefaultVATPercentage();

        foreach (MOSTComputers.Models.Product.Models.Product product in products)
        {
            HomeOfferPdfRequestItem requestItem = request.Items.First(x => x.Id == product.Id);

            decimal netPrice = requestItem.PricePerItemEur * requestItem.Quantity;
            decimal vatPrice = currencyVATService.CalculateVAT(requestItem.PricePerItemEur, requestItem.Quantity, varPercentageFraction);
            decimal totalPrice = netPrice + vatPrice;

            HomeOfferItemData homeOfferItemData = new()
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Quantity = requestItem.Quantity,
                PricePerUnit = requestItem.PricePerItemEur,
                LineNetPrice = netPrice,
                LineVatPrice = vatPrice,
                LineTotalPrice = totalPrice,
            };

            homeOfferItems.Add(homeOfferItemData);
        }

        HomeOfferData homeOfferData = new()
        {
            OfferNumber = request.OfferNumber ?? 1,
            AdditionalInfo = request.AdditionalInfo,
            Date = DateTime.Now,
            Items = homeOfferItems,
        };

        Stream fileStream = await pdfHomeOfferFileGeneratorService.CreateHomeOfferPdfAndGetStreamAsync(homeOfferData);

        return Results.File(fileStream, "application/pdf");
    }

    private static void SortProductsFromSecondCategoryUp(List<MOSTComputers.Models.Product.Models.Product> products)
    {
        products.Sort(CompareProductsFromSecondCategoryUp);
    }

    private static int CompareProductsFromSecondCategoryUp(
        MOSTComputers.Models.Product.Models.Product product1,
        MOSTComputers.Models.Product.Models.Product product2)
    {
        int categorySortIndex1 = product1.CategoryId > 1 ? product1.CategoryId.Value : int.MaxValue;
        int categorySortIndex2 = product2.CategoryId > 1 ? product2.CategoryId.Value : int.MaxValue;

        int categoryComparison = categorySortIndex1 - categorySortIndex2;

        if (categoryComparison != 0) return categoryComparison;

        int displayOrder1 = product1.DisplayOrder ?? int.MaxValue;
        int displayOrder2 = product2.DisplayOrder ?? int.MaxValue;

        int displayOrderComparison = displayOrder1 - displayOrder2;

        if (displayOrderComparison != 0) return displayOrderComparison;

        return product1.Id.CompareTo(product2.Id);
    }
}
