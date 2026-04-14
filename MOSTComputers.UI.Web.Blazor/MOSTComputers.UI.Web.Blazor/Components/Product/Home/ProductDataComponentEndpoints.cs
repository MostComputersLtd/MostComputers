using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Services.Currencies.Contracts;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductProperties.Contacts;
using MOSTComputers.Services.ProductRegister.Services.Products.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Contracts;
using MOSTComputers.Services.SearchStringOrigin.Models;
using MOSTComputers.Services.SearchStringOrigin.Services.Contracts;
using MOSTComputers.UI.Web.Blazor.Endpoints;
using OneOf;
using ZiggyCreatures.Caching.Fusion;
using static MOSTComputers.UI.Web.Blazor.Components.Pages.Home2;

namespace MOSTComputers.UI.Web.Blazor.Components.Product.Home;

public static class ProductDataComponentEndpoints
{
    public sealed class ProductSearchData
    {
        public string? SearchData { get; set; }
        public int? CategoryId { get; set; }
        public int? ManufacturerId { get; set; }
        public ProductStatusSearchOptions ProductStatusSearchOptions { get; set; } = ProductStatusSearchOptions.AvailableAndCall;
        public PromotionSearchOptions? PromotionSearchOptions { get; set; } = null;
        public Currency? Currency { get; set; } = null;
        public int? MaxSearchResults { get; set; } = null;
    }
    private sealed class ManufacturerSearchResponse
    {
        public int? id { get; set; }
        public string? displayName { get; set; }
    }

    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "components/" + "home";

    public static IEndpointConventionBuilder MapProductDataComponentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute);

        endpointGroup.MapGet("/search/manufacturers/{categoryId:int?}", GetManufacturersForCategoryAsync);

        endpointGroup.MapGet("/productXmlDataPopup/{productId:int}", GetProductXmlPopupDataAsync);
        endpointGroup.MapGet("/productDataPopup/{productId:int}", GetProductDataPopupAsync);
        endpointGroup.MapGet("/productGroupPromotionPopup/{promotionGroupId:int}", GetProductGroupPromotionPopupAsync);
        endpointGroup.MapGet("/productSearchStringPopup/{productId:int}", GetProductSearchStringPartsPopupAsync);

        endpointGroup.MapPost("/search", GetProductTableForSearchedProductsAsync);

        return endpointGroup;
    }

    private static async Task<IResult> GetProductTableForSearchedProductsAsync(
        HttpContext httpContext,
        [FromBody] ProductSearchData productSearchData,
        [FromServices] IProductService productService,
        [FromServices] IProductSearchService productSearchService)
    {
        httpContext.Response.ContentType = "application/html";
        httpContext.Response.Headers.TryAdd("Content-Disposition", "inline; filename=data.xml");

        List<ProductDisplayData> productDisplayDatas = await SearchProductsAsync(
            productService, productSearchService, productSearchData);

        return new RazorComponentResult<ProductDatasTable>(new
        {
            ProductDisplayDatas = productDisplayDatas,
            productSearchData.Currency,
            SecondaryCurrency = (Currency?)(productSearchData.Currency == Currency.EUR ? Currency.BGN : null),
            ShowImages = false
        });
    }

    private static async Task<List<ProductDisplayData>> SearchProductsAsync(
        IProductService productService,
        IProductSearchService productSearchService,
        ProductSearchData productSearchData)
    {
        int selectedResultCount = Math.Min(productSearchData.MaxSearchResults ?? int.MaxValue, 300);
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

        if (products.Count <= 0)
        {
            ProductSearchRequest productSearchRequest = new()
            {
                OnlyVisibleByEndUsers = true,
                UserInputString = productSearchData.SearchData,
                CategoryId = productSearchData.CategoryId,
                ManufacturerId = productSearchData.ManufacturerId,
                ProductStatus = productSearchData.ProductStatusSearchOptions,
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

        List<ProductDisplayData> productDisplayDatas = products.Select(product =>
        {
            return new ProductDisplayData()
            {
                Product = product,
            };
        })
            .ToList();

        return productDisplayDatas;
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
        [FromServices] IProductService productService,
        [FromServices] IProductCharacteristicService ProductCharacteristicService,
        [FromServices] IProductPropertyService ProductPropertyService,
        [FromServices] IProductImageService ProductImageService,
        [FromServices] IPromotionService PromotionService,
        [FromServices] ISearchStringOriginService SearchStringOriginService,
        [FromServices] ICurrencyConversionService currencyConversionService,
        [FromServices] ICurrencyVATService currencyVATService,
        int productId)
    {
        MOSTComputers.Models.Product.Models.Product? product = await productService.GetByIdAsync(productId);
        
        if (product == null)
        {
            return Results.NotFound();
        }

        List<int> relatedCategoryIds = [-1];

        if (product.CategoryId is not null)
        {
            relatedCategoryIds.Add(product.CategoryId.Value);
        }

        IEnumerable<ProductCharacteristicType> productCharacteristicTypes = [
            ProductCharacteristicType.ProductCharacteristic,
            ProductCharacteristicType.Link
        ];

        List<ProductCharacteristic> productCharacteristics = await ProductCharacteristicService.GetAllByCategoryIdsAndTypesAsync(
            relatedCategoryIds, productCharacteristicTypes, true);

        List<ProductProperty> productProperties = await ProductPropertyService.GetAllInProductAsync(product.Id);

        List<Client.Product.ProductData.ProductPropertyDisplayData> propertiesInData
            = GetProductPropertiesFromProductData(productProperties, productCharacteristics);

        List<ProductImageData> productImages = await ProductImageService.GetAllInProductWithoutFileDataAsync(product.Id);

        if (productImages.Count == 0)
        {
            // var firstImage = await ProductImageService.GetByProductIdWithoutFileDataInFirstImagesAsync(product.Id);
        }

        List<Client.Product.ProductData.ProductImageDisplayData> productImagesInData = new();

        foreach (ProductImageData productImageData in productImages)
        {
            Client.Product.ProductData.ProductImageDisplayData imageDisplayData = new()
            {
                ImageSrc = $"/api/images/originalImageData/{productImageData.Id}"
            };

            productImagesInData.Add(imageDisplayData);
        }

        List<MOSTComputers.Models.Product.Models.Promotions.Promotion> promotions
            = await PromotionService.GetAllForProductAsync(product.Id);

        List<MOSTComputers.Models.Product.Models.Promotions.Promotion> productPromotions = promotions.Where(x =>
        {
            if (x.Id != product.PromotionPid && x.Id != product.PromotionRid) return false;

            return (x.StartDate is null || x.StartDate <= DateTime.Now) && (x.ExpirationDate is null || x.ExpirationDate >= DateTime.Now);
        })
            .ToList();

        List<SearchStringPartOriginData>? productSearchStringParts = await SearchStringOriginService.GetSearchStringPartsAndDataAboutTheirOriginAsync(
            product.SearchString, product.CategoryId);

        Client.Product.ProductData.ProductPriceData? productPriceData = await GetProductPriceDataAsync(
            currencyConversionService, currencyVATService, product);

        return new RazorComponentResult<ProductDataPopup>(new
        {
            ProductDataPopupData = new ProductDataPopupData()
            {
                IsVisible = true,
                Product = product,
                ProductDataPopupPriceData = productPriceData,
                ProductProperties = propertiesInData,
                ProductImages = productImagesInData,
                Promotions = productPromotions,
                ProductSearchStringParts = productSearchStringParts,
            }
        });
    }

    private static List<Client.Product.ProductData.ProductPropertyDisplayData> GetProductPropertiesFromProductData(
        List<ProductProperty> productProperties,
        List<ProductCharacteristic> productCharacteristics)
    {
        List<Client.Product.ProductData.ProductPropertyDisplayData> propertiesInData = new();

        foreach (ProductProperty property in productProperties)
        {
            ProductCharacteristic productCharacteristic = productCharacteristics
                .First(characteristic => characteristic.Id == property.ProductCharacteristicId);

            Client.Product.ProductData.ProductPropertyDisplayData mappedProperty = new()
            {
                ProductCharacteristic = new()
                {
                    Id = productCharacteristic!.Id,
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

    private static async Task<Client.Product.ProductData.ProductPriceData?> GetProductPriceDataAsync(
        ICurrencyConversionService currencyConversionService,
        ICurrencyVATService currencyVATService,
        MOSTComputers.Models.Product.Models.Product product)
    {
        Client.Product.ProductData.DisplayPrice? displayPrice = null;
        Client.Product.ProductData.DisplayPrice? secondaryDisplayPrice = null;

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
        [FromRoute] int promotionGroupId,
        [FromQuery] int productId)
    {
        MOSTComputers.Models.Product.Models.Product? product = await productService.GetByIdAsync(productId);

        if (product == null)
        {
            return Results.NotFound();
        }

        return new RazorComponentResult<PromotionGroupImagesPopup>(new
        {
            PromotionGroupPromotionPopupData = new PromotionGroupImagesPopupData()
            {
                IsVisible = true,
                Product = product,
                ManufacturerPromotionGroupId = promotionGroupId,
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
            }
        });
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
