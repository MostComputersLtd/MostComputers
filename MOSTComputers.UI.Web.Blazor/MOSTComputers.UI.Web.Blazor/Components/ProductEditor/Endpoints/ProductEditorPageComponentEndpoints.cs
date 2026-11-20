using System.Text.Encodings.Web;
using System.Text.Json;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Promotions;
using MOSTComputers.Models.Product.Models.Promotions.Files;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductWorkStatuses;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductProperties.Contacts;
using MOSTComputers.Services.ProductRegister.Services.Products.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductStatus.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.PromotionFiles.Contracts;
using MOSTComputers.Services.SearchStringOrigin.Models;
using MOSTComputers.Services.SearchStringOrigin.Services.Contracts;
using MOSTComputers.UI.Web.Blazor.Components.Primitives;
using MOSTComputers.UI.Web.Blazor.Components.Product;
using MOSTComputers.UI.Web.Blazor.Components.ProductEditor.SingleProductEditorPopup;
using MOSTComputers.UI.Web.Blazor.Endpoints;
using MOSTComputers.UI.Web.Blazor.Endpoints.Documents;
using MOSTComputers.UI.Web.Blazor.Endpoints.Images;
using MOSTComputers.UI.Web.Blazor.Services.ProductEditor;
using MOSTComputers.UI.Web.Blazor.Services.ProductEditor.Contracts;
using OneOf;
using SixLabors.ImageSharp;
using static MOSTComputers.Services.ProductRegister.Utils.ImageUtils;
using static MOSTComputers.UI.Web.Blazor.Components.Pages.ProductEditor2;
using static MOSTComputers.UI.Web.Blazor.Components.ProductEditor.SingleProductEditorPopup.EditorIdsGenerateUtils;
using static MOSTComputers.UI.Web.Blazor.Components.ProductEditor.SingleProductEditorPopup.SingleProductEditorPopup;
using static MOSTComputers.UI.Web.Blazor.Utils.AuthenticationUtils;
using static MOSTComputers.Utils.Files.ContentTypeUtils;
using static MOSTComputers.Utils.Files.FilePathUtils;

namespace MOSTComputers.UI.Web.Blazor.Components.ProductEditor.Endpoints;

internal static class ProductEditorPageComponentEndpoints
{
    private sealed class SearchProductsRequest
    {
        public int? ProductNewStatus { get; init; }
        public string? UserInput { get; init; }
        public int? CategoryId { get; init; }
        public int? ManufacturerId { get; init; }
        public int? ProductStatus { get; init; }
        public int? PromotionStatus { get; init; }
        public int? ResultsCount { get; init; }
    }

    private enum ProductSearchResultsOrdering
    {
        Default = 0,
        ByProductId = 1,
        DefaultWithCategory = 2,
    }

    private sealed class ProductSearchResult
    {
        public required List<MOSTComputers.Models.Product.Models.Product> Products { get; init; }
        public required ProductSearchResultsOrdering ProductOrdering { get; init; }
    }

    private sealed class ManufacturerSelectOption
    {
        public required int ManufacturerId { get; set; }
        public string? ManufacturerName { get; set; }
    }

    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "components/" + "productEditor";

    private const int _minProductSearchResultCount = 50;
    private const int _maxProductSearchResultCount = 2000;

    private const int _lastAddedProductsSearchCount = 200;

    public static IEndpointConventionBuilder MapProductEditorPageComponentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute)
            .RequireAuthorization(Policies.Cookie)
            .RequireAuthorization(policy => policy.RequireRole("ProductEditor"))
            .DisableCookieRedirect();

        endpointGroup.MapGet("/manufacturersInCategory", GetManufacturersThatHaveActiveProductsInACategoryAsync);
        endpointGroup.MapGet("/item/{productId:int}", GetProductEditorItemAsync);

        endpointGroup.MapGet("/_sync", StartApplicationSyncAsync);

        endpointGroup.MapGet("/productXmlDataPopup/{productId:int}", GetProductXmlPopupDataAsync);
        endpointGroup.MapGet("/productPromotionPopup/{productId:int}/{promotionId:int}", GetProductPromotionPopupDataAsync);
        endpointGroup.MapGet("/productInfoPromotionPopup/{productId:int}", GetProductInfoPromotionPopupDataAsync);
        endpointGroup.MapGet("/productImagesPopup/{productId:int}", GetProductImagesPopupDataAsync);
        endpointGroup.MapGet("/productImageFilesPopup/{productId:int}", GetProductImageFilesPopupDataAsync);
        endpointGroup.MapGet("/productPropertiesPopup/{productId:int}", GetProductPropertiesPopupDataAsync);
        endpointGroup.MapGet("/productSearchStringPopup/{productId:int}", GetProductSearchStringPopupDataAsync);
        endpointGroup.MapGet("promotionFilePopup/{productId:int}", GetProductPromotionFilePopupDataAsync);
        endpointGroup.MapGet("/singlePromotionFilePopup/{promotionProductFileId:int}", GetSingleProductPromotionFilePopupDataAsync);

        endpointGroup.MapPost("/search", GetSearchedProductsAsync);

        SingleProductEditorComponentEndpoints.MapSingleProductEditorComponentEndpoints(endpointGroup);

        return endpointGroup;
    }

    private static async Task StartApplicationSyncAsync(
        HttpContext httpContext,
        [FromServices] ApplicationDataSyncService applicationDataSyncService)
    {
        string userName = httpContext.User.Identity!.Name!;

        await applicationDataSyncService.SyncApplicationAsync(userName!);
    }

    private static async Task<IResult> GetProductEditorItemAsync(
        [FromRoute(Name = "productId")] int productId,
        IProductService productService,
        IProductEditorDataService productEditorDataService)
    {
        MOSTComputers.Models.Product.Models.Product? product = await productService.GetByIdAsync(productId);

        if (product == null) return Results.Problem(statusCode: StatusCodes.Status400BadRequest);

        Pages.ProductEditor.ProductEditorProductData productEditorData
            = await productEditorDataService.GetProductEditorProductDataAsync(product);

        return new RazorComponentResult<ProductEditorTableItem>(new
        {
            ItemData = TEMP__Map(productEditorData),
        });
    }

    private static async Task<IResult> GetSearchedProductsAsync(
        [FromBody] SearchProductsRequest request,
        IProductSearchService productSearchService,
        IProductEditorDataService productEditorDataService)
    {
        ProductSearchResult searchResult
            = await SearchProductsAsync(request, productSearchService);

        List<Pages.ProductEditor.ProductEditorProductData> oldProductDatas
            = await productEditorDataService.GetProductEditorProductDatasAsync(searchResult.Products);

        List<ProductEditorProductData> productDatas = new();

        foreach (Pages.ProductEditor.ProductEditorProductData productData in oldProductDatas)
        {
            productDatas.Add(TEMP__Map(productData));
        }

        var jsonParameters = new {
            ordering = (int)searchResult.ProductOrdering,
        };

        string jsonData = JsonSerializer.Serialize(jsonParameters);

        return new RazorComponentResult<ProductEditorTableItems>(new
        {
            ProductDatas = productDatas,
            JsonData = jsonData,
        });
    }

    private static async Task<ProductSearchResult> SearchProductsAsync(
        SearchProductsRequest request,
        IProductSearchService productSearchService)
    {
        ProductStatusSearchOptions? productStatus = null;

        if (request.ProductStatus.HasValue
            && Enum.IsDefined(typeof(ProductStatusSearchOptions), request.ProductStatus.Value))
        {
            productStatus = (ProductStatusSearchOptions)request.ProductStatus.Value;
        }

        ProductEditorProductNewStatusSearchOptions? productNewStatus = null;

        if (request.ProductNewStatus.HasValue
            && Enum.IsDefined(typeof(ProductEditorProductNewStatusSearchOptions), request.ProductNewStatus.Value))
        {
            productNewStatus = (ProductEditorProductNewStatusSearchOptions)request.ProductNewStatus.Value;
        }

        PromotionSearchOptions? promotionSearchOptions = null;

        if (request.PromotionStatus.HasValue
            && Enum.IsDefined(typeof(PromotionSearchOptions), request.PromotionStatus.Value))
        {
            promotionSearchOptions = (PromotionSearchOptions)request.PromotionStatus.Value;
        }

        int? manufacturerId = request.ManufacturerId;

        if (manufacturerId == -1)
        {
            manufacturerId = null;
        }

        bool orderProductsByLastAddedDate = productNewStatus == ProductEditorProductNewStatusSearchOptions.LastAdded
            || productNewStatus == ProductEditorProductNewStatusSearchOptions.LastAddedNew;

        bool orderProductsFromSecondCategoryUp = request.CategoryId == null && string.IsNullOrWhiteSpace(request.UserInput);

        int selectedResultCount = request.ResultsCount ?? _minProductSearchResultCount;

        selectedResultCount = Math.Min(selectedResultCount, _maxProductSearchResultCount);

        int? searchMaxResultCount = selectedResultCount;

        if (orderProductsByLastAddedDate || orderProductsFromSecondCategoryUp)
        {
            searchMaxResultCount = null;
        }

        List<ProductNewStatusSearchOptions>? productNewStatusSearchOptions = productNewStatus switch
        {
            ProductEditorProductNewStatusSearchOptions.New => [ProductNewStatusSearchOptions.New],
            ProductEditorProductNewStatusSearchOptions.WorkInProgress => [ProductNewStatusSearchOptions.WorkInProgress],
            ProductEditorProductNewStatusSearchOptions.NewAndWorkInProgress =>
                [ProductNewStatusSearchOptions.New, ProductNewStatusSearchOptions.WorkInProgress],
            ProductEditorProductNewStatusSearchOptions.ReadyForUse => [ProductNewStatusSearchOptions.ReadyForUse],
            ProductEditorProductNewStatusSearchOptions.Postponed1 => [ProductNewStatusSearchOptions.Postponed1],
            ProductEditorProductNewStatusSearchOptions.Postponed2 => [ProductNewStatusSearchOptions.Postponed2],
            ProductEditorProductNewStatusSearchOptions.LastAddedNew => [ProductNewStatusSearchOptions.New],
            ProductEditorProductNewStatusSearchOptions.None => [ProductNewStatusSearchOptions.None],
            _ => null
        };

        ProductSearchRequest productSearchRequest = new()
        {
            OnlyVisibleByEndUsers = false,
            UserInputString = request.UserInput,
            CategoryId = request.CategoryId,
            ManufacturerId = manufacturerId,
            ProductNewStatuses = productNewStatusSearchOptions,
            ProductStatus = productStatus,
            PromotionSearchOptions = promotionSearchOptions,
            MaxResultCount = searchMaxResultCount,
        };

        List<MOSTComputers.Models.Product.Models.Product> products = await productSearchService.SearchProductsAsync(productSearchRequest);

        ProductSearchResultsOrdering ordering = ProductSearchResultsOrdering.Default;

        if (orderProductsByLastAddedDate)
        {
            products = products.OrderByDescending(x => x.Id).ToList();
            ordering = ProductSearchResultsOrdering.ByProductId;
        }
        else if (orderProductsFromSecondCategoryUp)
        {
            SortProductsFromSecondCategoryUp(products);
            ordering = ProductSearchResultsOrdering.DefaultWithCategory;
        }
        else
        {
            products = products
                .OrderBy(x => x.DisplayOrder ?? int.MaxValue)
                .ThenBy(x => x.Id)
                .ToList();
        }

        if (request.ResultsCount != null
            && request.ResultsCount > 0
            && request.ResultsCount <= products.Count)
        {
            products = products[..request.ResultsCount.Value];
        }

        return new()
        {
            Products = products,
            ProductOrdering = ordering,
        };
    }

    private static void SortProductsFromSecondCategoryUp(List<MOSTComputers.Models.Product.Models.Product> products)
    {
        products.Sort(CompareProductsFromSecondCategoryUp);
    }

    private static int CompareProductsFromSecondCategoryUp(
        MOSTComputers.Models.Product.Models.Product product1, MOSTComputers.Models.Product.Models.Product product2)
    {
        int categorySortIndex1 = (product1.CategoryId > 1) ? product1.CategoryId.Value : int.MaxValue;
        int categorySortIndex2 = (product2.CategoryId > 1) ? product2.CategoryId.Value : int.MaxValue;

        int categoryComparison = categorySortIndex1 - categorySortIndex2;

        if (categoryComparison != 0) return categoryComparison;

        int displayOrder1 = product1.DisplayOrder ?? int.MaxValue;
        int displayOrder2 = product2.DisplayOrder ?? int.MaxValue;

        int displayOrderComparison = displayOrder1 - displayOrder2;

        if (displayOrderComparison != 0) return displayOrderComparison;

        return product1.Id.CompareTo(product2.Id);
    }

    private static async Task<IResult> GetManufacturersThatHaveActiveProductsInACategoryAsync(
        [FromQuery(Name = "categoryId")] int? categoryId,
        [FromServices] IProductService productService)
    {
        List<MOSTComputers.Models.Product.Models.Product> productsInCategory;

        if (categoryId == null)
        {
            productsInCategory = await productService.GetAllAsync();
        }
        else
        {
            productsInCategory = await productService.GetAllInCategoryAsync(categoryId.Value);
        }

        Dictionary<int, Manufacturer> manufacturersDict = new();

        foreach (MOSTComputers.Models.Product.Models.Product product in productsInCategory)
        {
            Manufacturer? manufacturer = product.Manufacturer;

            if (manufacturer == null) continue; 

            manufacturersDict.TryAdd(manufacturer.Id, manufacturer);
        }

        List<ManufacturerSelectOption> selectOptions = new();

        IOrderedEnumerable<Manufacturer> manufacturers = manufacturersDict.Values
            .OrderBy(x => x.DisplayOrder);

        foreach (Manufacturer manufacturer in manufacturers)
        {
            ManufacturerSelectOption selectOption = new()
            {
                ManufacturerId = manufacturer.Id,
                ManufacturerName = manufacturer.RealCompanyName,
            };

            selectOptions.Add(selectOption);
        }

        return Results.Json(selectOptions);
    }

    private static async Task<IResult> GetProductXmlPopupDataAsync(
        [FromServices] IProductService productService,
        [FromRoute(Name = "productId")] int productId)
    {
        MOSTComputers.Models.Product.Models.Product? product = await productService.GetByIdAsync(productId);

        if (product == null)
        {
            return Results.NotFound();
        }

        return new RazorComponentResult<ProductEditorProductXmlDataPopup>(new
        {
            PopupData = new ProductEditorProductXmlDataPopup.ProductEditorProductXmlPopupData()
            {
                IsVisible = true,
                Product = product,
            }
        });
    }

    private static async Task<IResult> GetProductPromotionPopupDataAsync(
        [FromServices] IProductService productService,
        [FromServices] IPromotionService promotionService,
        [FromRoute(Name = "productId")] int productId,
        [FromRoute(Name = "promotionId")] int promotionId)
    {
        MOSTComputers.Models.Product.Models.Product? product = await productService.GetByIdAsync(productId);

        if (product == null)
        {
            return Results.NotFound();
        }

        List<Promotion> promotions = await promotionService.GetAllActiveForProductAsync(productId);

        Promotion? promotion = promotions.FirstOrDefault(x => x.Id == promotionId);

        return new RazorComponentResult<ProductEditorPromotionPopup>(new
        {
            PopupData = new ProductEditorPromotionPopup.ProductEditorProductPromotionPopupData()
            {
                IsVisible = true,
                Product = product,
                Promotion = promotion,
            }
        });
    }

    private static async Task<IResult> GetProductInfoPromotionPopupDataAsync(
        [FromServices] IProductService productService,
        [FromRoute(Name = "productId")] int productId)
    {
        MOSTComputers.Models.Product.Models.Product? product = await productService.GetByIdAsync(productId);

        if (product == null)
        {
            return Results.NotFound();
        }

        return new RazorComponentResult<ProductEditorInfoPromotionPopup>(new
        {
            PopupData = new ProductEditorInfoPromotionPopup.ProductEditorInfoPromotionPopupData()
            {
                IsVisible = true,
                Product = product,
            }
        });
    }

    private static async Task<IResult> GetProductImagesPopupDataAsync(
        [FromServices] IProductService productService,
        [FromServices] IProductImageService productImageService,
        [FromRoute(Name = "productId")] int productId)
    {
        MOSTComputers.Models.Product.Models.Product? product = await productService.GetByIdAsync(productId);

        if (product == null)
        {
            return Results.NotFound();
        }

        List<ProductImageData> images = await productImageService.GetAllInProductWithoutFileDataAsync(productId);

        return new RazorComponentResult<ProductEditorImagesPopup>(new
        {
            PopupData = new ProductEditorImagesPopup.ProductEditorImagesPopupData()
            {
                IsVisible = true,
                Product = product,
                ImageDatas = images,
            }
        });
    }

    private static async Task<IResult> GetProductImageFilesPopupDataAsync(
        [FromServices] IProductService productService,
        [FromServices] IProductImageFileService productImageFileService,
        [FromRoute(Name = "productId")] int productId)
    {
        MOSTComputers.Models.Product.Models.Product? product = await productService.GetByIdAsync(productId);

        if (product == null)
        {
            return Results.NotFound();
        }

        List<ProductImageFileData> imageFiles = await productImageFileService.GetAllInProductAsync(productId);

        return new RazorComponentResult<ProductEditorImageFilesPopup>(new
        {
            PopupData = new ProductEditorImageFilesPopup.ProductEditorImageFilesPopupData()
            {
                IsVisible = true,
                Product = product,
                FileNameData = imageFiles,
            }
        });
    }

    private static async Task<IResult> GetProductPropertiesPopupDataAsync(
        [FromServices] IProductService productService,
        [FromServices] IProductPropertyService productPropertyService,
        [FromRoute(Name = "productId")] int productId)
    {
        MOSTComputers.Models.Product.Models.Product? product = await productService.GetByIdAsync(productId);

        if (product == null)
        {
            return Results.NotFound();
        }

        List<ProductProperty> properties = await productPropertyService.GetAllInProductAsync(productId);

        return new RazorComponentResult<ProductEditorPropertiesPopup>(new
        {
            PopupData = new ProductEditorPropertiesPopup.ProductEditorPropertiesPopupData()
            {
                IsVisible = true,
                Product = product,
                Properties = properties,
            }
        });
    }

    private static async Task<IResult> GetProductSearchStringPopupDataAsync(
        [FromServices] IProductService productService,
        [FromServices] ISearchStringOriginService searchStringOriginService,
        [FromRoute(Name = "productId")] int productId)
    {
        MOSTComputers.Models.Product.Models.Product? product = await productService.GetByIdAsync(productId);

        if (product == null)
        {
            return Results.NotFound();
        }

        List<SearchStringPartOriginData>? searchStringParts;

        if (string.IsNullOrWhiteSpace(product.SearchString)
            || product.CategoryId == null)
        {
            searchStringParts = [];
        }
        else
        {
            searchStringParts = await searchStringOriginService.GetSearchStringPartsAndDataAboutTheirOriginAsync(
            product.SearchString, product.CategoryId.Value);

            searchStringParts ??= [];
        }
        

        return new RazorComponentResult<ProductEditorSearchStringPopup>(new
        {
            PopupData = new ProductEditorSearchStringPopup.ProductEditorSearchStringPopupData()
            {
                IsVisible = true,
                Product = product,
                SearchStringParts = searchStringParts,
            }
        });
    }

    private static async Task<IResult> GetProductPromotionFilePopupDataAsync(
        [FromRoute(Name = "productId")] int productId,
        IProductService productService,
        IPromotionProductFileService promotionProductFileService,
        IPromotionService promotionService)
    {
        MOSTComputers.Models.Product.Models.Product? product = await productService.GetByIdAsync(productId);

        if (product == null)
        {
            return Results.Problem(statusCode: StatusCodes.Status404NotFound);
        }

        List<PromotionProductFileInfo> promotionProductFiles = await promotionProductFileService.GetAllForProductAsync(productId);

        List<PromotionFileEditorPopup.PromotionProductFileInfoReadonlyDisplayData> promotionFileData = [];

        foreach (PromotionProductFileInfo promotionProductFile in promotionProductFiles)
        {
            PromotionFileEditorPopup.PromotionProductFileInfoReadonlyDisplayData editorData = new()
            {
                Id = promotionProductFile.Id,
                ValidFrom = promotionProductFile.ValidFrom,
                ValidTo = promotionProductFile.ValidTo,
                Active = promotionProductFile.Active,
                ProductImageId = promotionProductFile?.ProductImageId,
                PromotionFileInfo = promotionProductFile?.PromotionFileInfo,
            };

            promotionFileData.Add(editorData);
        }

        OneOf<List<PromotionFileEditorPopup.PromotionProductFileInfoReadonlyDisplayData>,
            List<PromotionFileEditorPopup.PromotionProductFileInfoDisplayData>> promotionFileDataWrapped
                = OneOf<List<PromotionFileEditorPopup.PromotionProductFileInfoReadonlyDisplayData>,
                List<PromotionFileEditorPopup.PromotionProductFileInfoDisplayData>>.FromT0(promotionFileData);

        List<Promotion> promotions = await promotionService.GetAllForProductAsync(productId);

        Promotion? pidPromotion = null;
        Promotion? ridPromotion = null;

        DateTime now = DateTime.Now;

        foreach (Promotion promotion in promotions)
        {
            if (promotion.StartDate > now || promotion.ExpirationDate < now)
            {
                continue;
            }

            if (promotion.Id == product.PromotionPid)
            {
                pidPromotion = promotion;
            }
            else if (promotion.Id == product.PromotionRid)
            {
                ridPromotion = promotion;
            }
        }

        return new RazorComponentResult<PromotionFileEditorPopup>(new
        {
            IsVisible = true,
            Product = product,
            PromotionProductFiles = promotionFileDataWrapped,
            PidPromotion = pidPromotion,
            RidPromotion = ridPromotion,
            PopupMarginOptions = new Popup.MarginOptions() { Left = "10vw", Top = "10vh", Right = "10vw", Bottom = "10vh" },
        });
    }

    private static async Task<IResult> GetSingleProductPromotionFilePopupDataAsync(
        [FromRoute(Name = "promotionProductFileId")] int promotionProductFileId,
        IPromotionProductFileService promotionProductFileService)
    {
        PromotionProductFileInfo? promotionProductFile = await promotionProductFileService.GetByIdAsync(promotionProductFileId);

        if (promotionProductFile == null)
        {
            return Results.Problem(statusCode: StatusCodes.Status404NotFound);
        }

        SinglePromotionFilePopup.PromotionProductFileInfoReadonlyData popupData = new()
        {
            Id = promotionProductFile.Id,
            PromotionFile = promotionProductFile.PromotionFileInfo,
            ValidFrom = promotionProductFile.ValidFrom,
            ValidTo = promotionProductFile.ValidTo,
            Active = promotionProductFile.Active,
            CreateUserName = promotionProductFile.CreateUserName,
            CreateDate = promotionProductFile.CreateDate,
            LastUpdateUserName = promotionProductFile.LastUpdateUserName,
            LastUpdateDate = promotionProductFile.LastUpdateDate,
            IsInImage = promotionProductFile.ProductImageId != null,
        };

        OneOf<SinglePromotionFilePopup.PromotionProductFileInfoReadonlyData,
            SinglePromotionFilePopup.PromotionProductFileInfoEditData> popupDataWrapped
                = OneOf<SinglePromotionFilePopup.PromotionProductFileInfoReadonlyData,
            SinglePromotionFilePopup.PromotionProductFileInfoEditData>.FromT0(popupData);

        return new RazorComponentResult<SinglePromotionFilePopup>(new
        {
            IsVisible = true,
            PopupData = popupDataWrapped,
            PopupMarginOptions = new Popup.MarginOptions() { Left = "15vw", Right = "15vw", Top = "15vh", Bottom = "15vh" },
        });
    }
}
