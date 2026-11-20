using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Models.Common;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Promotions;
using MOSTComputers.Models.Product.Models.Promotions.Files;
using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.Currencies.Contracts;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Html.New;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Html.New.Contracts;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductWorkStatuses;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductProperties.Contacts;
using MOSTComputers.Services.ProductRegister.Services.ProductStatus.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.PromotionFiles.Contracts;
using MOSTComputers.Services.SearchStringOrigin.Models;
using MOSTComputers.Services.SearchStringOrigin.Services.Contracts;
using MOSTComputers.UI.Web.Blazor.Components.Product;
using MOSTComputers.UI.Web.Blazor.Components.Primitives;
using MOSTComputers.UI.Web.Blazor.Components.ProductEditor.SingleProductEditorPopup;
using MOSTComputers.UI.Web.Blazor.Endpoints;
using MOSTComputers.UI.Web.Blazor.Endpoints.Documents;
using MOSTComputers.UI.Web.Blazor.Endpoints.Images;
using OneOf;
using static MOSTComputers.Services.ProductRegister.Utils.ImageUtils;
using static MOSTComputers.UI.Web.Blazor.Components.ProductEditor.SingleProductEditorPopup.EditorIdsGenerateUtils;
using static MOSTComputers.UI.Web.Blazor.Components.ProductEditor.SingleProductEditorPopup.SingleProductEditorPopup;
using static MOSTComputers.UI.Web.Blazor.Utils.AuthenticationUtils;
using static MOSTComputers.UI.Web.Blazor.Utils.UrlUtils;

namespace MOSTComputers.UI.Web.Blazor.Components.ProductEditor.Endpoints;

internal static class SingleProductEditorComponentEndpoints
{
    private sealed class ProductDataPopupRequest
    {
        [JsonPropertyName("productProperties")]
        public List<ProductDataPopupPropertyRequest>? ProductProperties { get; init; }

        [JsonPropertyName("productImageUrls")]
        public List<string>? ProductImageUrls { get; init; }
    }

    private sealed class ProductDataPopupPropertyRequest
    {
        [JsonPropertyName("productCharacteristicId")] public required int ProductCharacteristicId { get; set; }
        [JsonPropertyName("value")] public string? Value { get; set; }
    }

    private sealed class ProductLegacyDisplayPopupRequest
    {
        [JsonPropertyName("productProperties")]
        public List<ProductLegacyPopupPropertyRequest>? ProductProperties { get; init; }

        [JsonPropertyName("productImages")]
        public List<ProductLegacyDisplayPopupImageRequest>? ProductImages { get; init; }
    } 

    private sealed class ProductLegacyPopupPropertyRequest
    {
        [JsonPropertyName("productCharacteristicId")] public required int ProductCharacteristicId { get; set; }
        [JsonPropertyName("value")] public string? Value { get; set; }
    }

    private sealed class ProductLegacyDisplayPopupImageRequest
    {
        [JsonPropertyName("imageUrl")] public required string ImageUrl { get; set; }
        [JsonPropertyName("displayOrder")] public int? DisplayOrder { get; set; }
        [JsonPropertyName("active")] public bool? Active { get; set; }
    }

    internal const string EndpointGroupUniqueRoute = "single/";

    private static readonly JsonSerializerOptions _jsonDataOptions = new()
    {
        Encoder = JavaScriptEncoder.Default,
    };

    public static IEndpointConventionBuilder MapSingleProductEditorComponentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupUniqueRoute);

        endpointGroup.MapGet("/{productId:int}", GetSingleProductEditorPopupAsync);

        endpointGroup.MapPost("/productDataPopup/{productId:int}", GetProductDataPopupAsync);
        endpointGroup.MapPost("/productLegacyDisplayPopup/{productId:int}", GetProductLegacyDisplayPopupAsync);
        endpointGroup.MapGet("/unsavedChangesPopup", GetUnsavedChangesPopup);

        endpointGroup.MapPut("/updateProductWorkStatus/{productId:int}/{productNewStatus:int}",
            UpdateProductWorkStatusAsync);

        SingleProductEditorDocumentComponentEndpoints.MapSingleProductEditorDocumentComponentEndpoints(endpointGroup);
        SingleProductEditorImageComponentEndpoints.MapSingleProductEditorImageComponentEndpoints(endpointGroup);
        SingleProductEditorPromotionFileComponentEndpoints.MapSingleProductEditorPromotionFileComponentEndpoints(endpointGroup);
        SingleProductEditorSaveEndpoints.MapSingleProductEditorSaveEndpoint(endpointGroup);

        return endpointGroup;
    }

    private static async Task<IResult> GetSingleProductEditorPopupAsync(
        [FromRoute(Name = "productId")] int productId,
        [FromServices] IProductService productService,
        [FromServices] IProductPropertyService productPropertyService,
        [FromServices] IProductCharacteristicService productCharacteristicService,
        [FromServices] IProductImageService productImageService,
        [FromServices] IProductImageFileService productImageFileService,
        [FromServices] IProductWorkStatusesService productWorkStatusesService,
        [FromServices] IPromotionService promotionService,
        [FromServices] IPromotionProductFileService promotionProductFileService,
        [FromServices] IProductDocumentFileService productDocumentFileService,
        [FromServices] ISearchStringOriginService searchStringOriginService)
    {
        MOSTComputers.Models.Product.Models.Product? product = await productService.GetByIdAsync(productId);

        if (product == null) return Results.NotFound();

        return await GetSingleProductEditorPopupForProductAsync(
            product,
            productPropertyService,
            productCharacteristicService,
            productImageService,
            productImageFileService,
            productWorkStatusesService,
            promotionService,
            promotionProductFileService,
            productDocumentFileService,
            searchStringOriginService);
    }

    public static async Task<RazorComponentResult<SingleProductEditorPopup.SingleProductEditorPopup>> GetSingleProductEditorPopupForProductAsync(
        MOSTComputers.Models.Product.Models.Product product,
        IProductPropertyService productPropertyService, 
        IProductCharacteristicService productCharacteristicService,
        IProductImageService productImageService,
        IProductImageFileService productImageFileService,
        IProductWorkStatusesService productWorkStatusesService,
        IPromotionService promotionService,
        IPromotionProductFileService promotionProductFileService,
        IProductDocumentFileService productDocumentFileService,
        ISearchStringOriginService searchStringOriginService)
    {
        List<ProductImageData> productImages = await productImageService.GetAllInProductWithoutFileDataAsync(product.Id);

        List<ProductImageData> productImagesCopy = new(productImages);

        List<ProductImageFileData> productImageFiles = await productImageFileService.GetAllInProductAsync(product.Id);

        List<ProductImageFileData> productImageFilesCopy = new(productImageFiles);

        List<PromotionProductFileInfo> promotionProductFileInfos = await promotionProductFileService.GetAllForProductAsync(product.Id);

        List<PromotionFileEditorPopup.PromotionProductFileEditorJsonData> promotionProductFileEditorJsonDatas = [];
        List<PromotionFileEditorPopup.PromotionProductFileRecoveryJsonData> promotionProductFileRecoveryJsonDatas = [];

        foreach (PromotionProductFileInfo promotionProductFileInfo in promotionProductFileInfos)
        {
            string editorId = GeneratePromotionFileId(promotionProductFileInfo.Id);

            PromotionFileEditorPopup.PromotionProductFileEditorJsonData promotionFileEditorJsonData = new()
            {
                EditorId = editorId,
                Id = promotionProductFileInfo.Id,
                PromotionFileId = promotionProductFileInfo.PromotionFileInfoId,
                ValidFrom = promotionProductFileInfo.ValidFrom,
                ValidTo = promotionProductFileInfo.ValidTo,
                Active = promotionProductFileInfo.Active,
            };

            PromotionFileEditorPopup.PromotionProductFileRecoveryJsonData promotionFileRecoveryJsonData = new()
            {
                EditorId = editorId,
                Id = promotionProductFileInfo.Id,
                PromotionFileId = promotionProductFileInfo.PromotionFileInfoId,
                ValidFrom = promotionProductFileInfo.ValidFrom,
                ValidTo = promotionProductFileInfo.ValidTo,
                Active = promotionProductFileInfo.Active,
            };

            promotionProductFileEditorJsonDatas.Add(promotionFileEditorJsonData);
            promotionProductFileRecoveryJsonDatas.Add(promotionFileRecoveryJsonData);
        }

        List<ProductImageListItemBase> productImageDatas = [];
        List<ProductImageRecoveryJsonDataBase> productImageRecoveryJsonDatas = [];

        Dictionary<string, int?> productImageDataDisplayOrders = [];
        Dictionary<string, int?> productImageDataImageIds = [];

        for (int i = 0; i < promotionProductFileInfos.Count; i++)
        {
            PromotionProductFileInfo promotionProductFile = promotionProductFileInfos[i];

            if (promotionProductFile.ProductImageId == null) continue;

            PromotionFileEditorPopup.PromotionProductFileEditorJsonData? promotionProductFileData
                = promotionProductFileEditorJsonDatas.First(x => x.Id == promotionProductFile.Id);

            ProductImageData relatedImage = productImagesCopy.First(x => x.Id == promotionProductFile.ProductImageId);
            ProductImageFileData relatedImageFile = productImageFilesCopy.First(x => x.ImageId == promotionProductFile.ProductImageId);

            string editorId = GenerateImageId(relatedImage.Id, relatedImageFile.Id, promotionProductFile.Id);

            PromotionFileImageListItem.PromotionFileImageItemDisplayData promotionFileImageData = new()
            {
                EditorId = editorId,
                Index = i,
                PromotionFileData = new()
                {
                    Name = promotionProductFile.PromotionFileInfo.Name,
                    FileName = promotionProductFile.PromotionFileInfo.FileName,
                },
                ImageData = new()
                {
                    Active = relatedImageFile.Active,
                },
                RelatedPromotionFileEditorId = promotionProductFileData.EditorId,
            };

            PromotionFileImageListItem.PromotionFileImageItemRecoveryJsonData promotionFileImageRecoveryJsonData = new()
            {
                EditorId = editorId,
                ImageFileData = new()
                {
                    Active = relatedImageFile.Active,
                },
                RelatedPromotionFileEditorId = promotionProductFileData.EditorId,
            };

            productImageDatas.Add(promotionFileImageData);
            productImageRecoveryJsonDatas.Add(promotionFileImageRecoveryJsonData);

            productImageDataDisplayOrders.Add(editorId, relatedImageFile.DisplayOrder);
            productImageDataImageIds.Add(editorId, relatedImage.Id);

            productImagesCopy.Remove(relatedImage);
            productImageFilesCopy.Remove(relatedImageFile);
        }

        foreach (ProductImageData productImage in productImagesCopy)
        {
            ProductImageFileData? relatedImageFile = productImageFilesCopy.Find(x => x.ImageId == productImage.Id);

            ProductImageListItem.ProductImageItemFileDisplayData? imageFileData = null;
            ProductImageListItem.ProductImageItemFileRecoveryJsonData? imageFileRecoveryJsonData = null;

            if (relatedImageFile != null)
            {
                imageFileData = new()
                {
                    Id = relatedImageFile.Id,
                    FileName = relatedImageFile.FileName!,
                    DisplayOrder = relatedImageFile.DisplayOrder,
                    Active = relatedImageFile.Active,
                };

                imageFileRecoveryJsonData = new()
                {
                    Active = relatedImageFile.Active,
                };

                productImageFilesCopy.Remove(relatedImageFile);
            }

            string editorId = GenerateImageId(productImage.Id, relatedImageFile?.Id, null);

            ProductImageListItem.ProductImageListItemData imageData = new()
            {
                EditorId = editorId,
                Index = productImageDatas.Count,
                Id = productImage.Id,
                DateModified = productImage.DateModified,
                RelatedImageFile = imageFileData,
            };

            ProductImageListItem.ProductImageItemRecoveryJsonData imageRecoveryJsonData = new()
            {
                EditorId = editorId,
                Id = productImage.Id,
                RelatedImageFile = imageFileRecoveryJsonData,
            };

            productImageDatas.Add(imageData);
            productImageRecoveryJsonDatas.Add(imageRecoveryJsonData);

            productImageDataDisplayOrders.Add(editorId, imageFileData?.DisplayOrder);
            productImageDataImageIds.Add(editorId, productImage.Id);
        }

        foreach (ProductImageFileData productImageFile in productImageFilesCopy)
        {
            string editorId = GenerateImageId(null, productImageFile.Id, null);

            ProductImageFileListItem.ProductImageFileItemData imageFileData = new()
            {
                EditorId = editorId,
                Index = productImageDatas.Count,
                Id = productImageFile.Id,
                FileName = productImageFile.FileName!,
                Active = productImageFile.Active,
                DisplayOrder = productImageFile.DisplayOrder,
            };

            ProductImageFileListItem.ProductImageFileItemRecoveryJsonData imageFileRecoveryJsonData = new()
            {
                EditorId = editorId,
                Id = productImageFile.Id,
                Active = productImageFile.Active,
            };

            productImageDatas.Add(imageFileData);
            productImageRecoveryJsonDatas.Add(imageFileRecoveryJsonData);

            productImageDataDisplayOrders.Add(editorId, imageFileData.DisplayOrder);
            productImageDataImageIds.Add(editorId, null);
        }

        productImageDatas = OrderImageRelatedItems(
            productImageDatas,
            imageItem => productImageDataDisplayOrders[imageItem.EditorId],
            imageItem => productImageDataImageIds[imageItem.EditorId]);

        productImageRecoveryJsonDatas = OrderImageRelatedItems(
            productImageRecoveryJsonDatas,
            imageRecoveryData => productImageDataDisplayOrders[imageRecoveryData.EditorId],
            imageRecoveryData => productImageDataImageIds[imageRecoveryData.EditorId]);

        List<ProductCharacteristic> relatedProductCharacteristics = await GetRelatedProductCharacteristicsAsync(
            productCharacteristicService, product.CategoryId);

        IOrderedEnumerable<ProductCharacteristic> orderedCharacteristics
            = relatedProductCharacteristics.OrderBy(x => x.DisplayOrder ?? 0);

        List<ProductProperty> activeProperties = await productPropertyService.GetAllInProductAsync(product.Id);

        List<ProductPropertyListItem.ProductPropertyDisplayData> productProperties = [];
        List<ProductPropertyListItem.ProductPropertyDisplayData> productLinks = [];

        List<ProductPropertyListItem.ProductPropertyRecoveryJsonData> jsonProductProperties = [];
        List<ProductPropertyListItem.ProductPropertyRecoveryJsonData> jsonProductLinks = [];

        foreach (ProductCharacteristic productCharacteristic in orderedCharacteristics)
        {
            ProductProperty? property = activeProperties.Find(
                x => x.ProductCharacteristicId == productCharacteristic.Id);

            bool doesPropertyExist = property != null;

            string editorId = GeneratePropertyId(
                productCharacteristic.Id, productCharacteristic.KWPrCh == ProductCharacteristicType.Link);

            ProductPropertyListItem.ProductPropertyDisplayData propertyData = new()
            {
                EditorId = editorId,
                IsActive = doesPropertyExist,
                ProductCharacteristic = productCharacteristic,
                Value = property?.Value,
            };

            ProductPropertyListItem.ProductPropertyRecoveryJsonData jsonPropertyData = new()
            {
                EditorId = editorId,
                IsActive = doesPropertyExist,
                ProductCharacteristicId = productCharacteristic.Id,
                Value = property?.Value,
            };

            if (productCharacteristic.KWPrCh == ProductCharacteristicType.ProductCharacteristic)
            {
                productProperties.Add(propertyData);
                jsonProductProperties.Add(jsonPropertyData);

                continue;
            }

            productLinks.Add(propertyData);
            jsonProductLinks.Add(jsonPropertyData);
        }

        List<Promotion> promotions = await promotionService.GetAllForProductAsync(product.Id);

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

        List<SearchStringPartOriginData> searchStringParts = await searchStringOriginService.GetSearchStringPartsAndDataAboutTheirOriginAsync(
            product.SearchString, product.CategoryId)
            ?? new();

        List<ProductDocument> productDocuments = await productDocumentFileService.GetAllForProductAsync(product.Id);

        List<ProductDocumentListItem.ProductDocumentDisplayData> productDocumentDatas = [];
        List<ProductDocumentListItem.ProductDocumentRecoveryJsonData> productDocumentRecoveryJsonDatas = [];

        for (int i = 0; i < productDocuments.Count; i++)
        {
            ProductDocument productDocument = productDocuments[i];

            string editorId = GenerateDocumentId(productDocument.Id);

            string documentBrowserUrl = '/' + JoinUrlPath(ProductDocumentEndpoints.EndpointGroupRoute, productDocument.Id.ToString());

            ProductDocumentListItem.ProductDocumentDisplayData productDocumentDisplayData = new()
            {
                EditorId = editorId,
                ExistingDocument = new()
                {
                    Id = productDocument.Id,
                    FileName = productDocument.FileName,
                },
                BrowserFilePath = documentBrowserUrl,
                CurrentDescription = productDocument.Description ?? string.Empty,
            };

            ProductDocumentListItem.ProductDocumentRecoveryJsonData productDocumentRecoveryJsonData = new()
            {
                EditorId = editorId,
                Id = productDocument.Id,
                Description = productDocument.Description ?? string.Empty,
            };

            productDocumentDatas.Add(productDocumentDisplayData);
            productDocumentRecoveryJsonDatas.Add(productDocumentRecoveryJsonData);
        }

        ProductWorkStatuses? productStatuses = await productWorkStatusesService.GetByProductIdAsync(product.Id);

        var parameters = new
        {
            productProperties = jsonProductProperties,
            productLinks = jsonProductLinks,
            productImages = productImageRecoveryJsonDatas,
            promotionProductFiles = promotionProductFileRecoveryJsonDatas,
            promotionProductFilesForEditor = promotionProductFileEditorJsonDatas,
            productDocuments = productDocumentRecoveryJsonDatas,
        };

        string parametersJsonData = JsonSerializer.Serialize(parameters, _jsonDataOptions);

        var output = new
        {
            IsVisible = true,
            PopupMarginOptions = new Popup.MarginOptions() { Left = "1vw", Right = "1vw", Top = "1vh", Bottom = "1vh" },
            Product = product,
            ProductStatuses = productStatuses,
            ProductProperties = productProperties,
            ProductLinks = productLinks,
            ProductImages = productImageDatas,
            SearchStringParts = searchStringParts,
            ProductDocuments = productDocumentDatas,
            PidPromotion = pidPromotion,
            RidPromotion = ridPromotion,
            JsonData = parametersJsonData,
        };

        return new RazorComponentResult<SingleProductEditorPopup.SingleProductEditorPopup>(output);
    }

    private static async Task<IResult> GetProductDataPopupAsync(
        HttpContext httpContext,
        [FromRoute(Name = "productId")] int productId,
        [FromBody] ProductDataPopupRequest request,
        [FromServices] IProductService productService,
        [FromServices] IPromotionService promotionService,
        [FromServices] IProductCharacteristicService productCharacteristicService,
        [FromServices] IProductPropertyService productPropertyService,
        [FromServices] IProductImageFileService productImageFileService,
        [FromServices] ISearchStringOriginService searchStringOriginService,
        [FromServices] IGroupPromotionService groupPromotionService,
        [FromServices] IGroupPromotionProductBindingsService groupPromotionProductBindingsService,
        [FromServices] IGroupPromotionImageFileDataService groupPromotionImageFileDataService,
        [FromServices] IPromotionFileService promotionFileService,
        [FromServices] ICurrencyConversionService currencyConversionService,
        [FromServices] ICurrencyVATService currencyVATService)
    {
        MOSTComputers.Models.Product.Models.Product? product = await productService.GetByIdAsync(productId);

        if (product == null) return Results.NotFound();

        List<ProductCharacteristic> characteristics
            = await GetRelatedProductCharacteristicsAsync(productCharacteristicService, product.CategoryId);

        List<ProductProperty> properties = await productPropertyService.GetAllInProductAsync(productId);

        List<ProductData.ProductPropertyDisplayData> propertyData = [];

        if (request.ProductProperties != null)
        {
            foreach (ProductDataPopupPropertyRequest productProperty in request.ProductProperties)
            {
                ProductCharacteristic? characteristic = characteristics.FirstOrDefault(
                    x => x.Id == productProperty.ProductCharacteristicId);

                if (characteristic == null) return Results.Problem(statusCode: StatusCodes.Status400BadRequest);

                ProductProperty? existingProperty = properties.FirstOrDefault(
                    x => x.ProductCharacteristicId == productProperty.ProductCharacteristicId);

                ProductData.ProductPropertyDisplayData displayData = new()
                {
                    ProductCharacteristic = new()
                    {
                        Id = characteristic.Id,
                        CategoryId = characteristic.CategoryId,
                        Active = characteristic.Active,
                        KWPrCh = characteristic.KWPrCh
                    },
                    Name = characteristic.Name,
                    Value = productProperty.Value,
                    DisplayOrder = existingProperty?.DisplayOrder ?? characteristic.DisplayOrder
                };

                propertyData.Add(displayData);
            }
        } 

        List<ProductData.ProductImageDisplayData> productImages = [];

        if (request.ProductImageUrls != null)
        {
            foreach (string imageUrl in request.ProductImageUrls)
            {
                ProductData.ProductImageDisplayData imageData = new()
                {
                    ImageSrc = imageUrl,
                };

                productImages.Add(imageData);
            }
        }

        ProductData.ProductPriceData? productPriceData = await GetProductPriceDataForPopupAsync(
            product.Currency, product.Price, currencyConversionService, currencyVATService);

        List<Promotion> promotions = [];

        List<Promotion> productPromotions = await promotionService.GetAllActiveForProductAsync(productId);

        foreach (Promotion promotion in productPromotions)
        {
            if (promotion.Id != product.PromotionPid && promotion.Id != product.PromotionRid) continue;

            if ((promotion.StartDate is null || promotion.StartDate <= DateTime.Now)
                && (promotion.ExpirationDate is null || promotion.ExpirationDate >= DateTime.Now))
            {
                promotions.Add(promotion);
            }
        }

        List<SearchStringPartOriginData>? searchStringParts = await searchStringOriginService.GetSearchStringPartsAndDataAboutTheirOriginAsync(
            product.SearchString, product.CategoryId);

        List<ProductData.ProductRelatedGroupPromotionDisplayData> productRelatedGroupPromotions = await GetRelatedGroupPromotionsAsync(
            productId, groupPromotionService, groupPromotionProductBindingsService, groupPromotionImageFileDataService);

        HttpRequest httpRequest = httpContext.Request;

        string baseUrl = $"{httpRequest.Scheme}://{httpRequest.Host}{httpRequest.PathBase}/";

        return new RazorComponentResult<Home.ProductDataPopup>(new
        {
            ProductDataPopupData = new Pages.Home.ProductDataPopupData()
            {
                IsVisible = true,
                Product = product,
                BaseProductPagePath = baseUrl,
                ProductDataPopupPriceData = productPriceData,
                ProductProperties = propertyData,
                ProductImages = productImages,
                ProductRelatedGroupPromotions = productRelatedGroupPromotions,
                Promotions = productPromotions,
                ProductSearchStringParts = searchStringParts,
            }
        });
    }

    private static async Task<List<ProductCharacteristic>> GetRelatedProductCharacteristicsAsync(
        IProductCharacteristicService productCharacteristicService,
        int? categoryId)
    {
        List<int> relatedCategoryIds = [-1];

        if (categoryId is not null)
        {
            relatedCategoryIds.Add(categoryId.Value);
        }

        List<ProductCharacteristicType> productCharacteristicTypes = [ProductCharacteristicType.ProductCharacteristic, ProductCharacteristicType.Link];

        List<ProductCharacteristic> relatedProductCharacteristics = await productCharacteristicService.GetAllByCategoryIdsAndTypesAsync(
            relatedCategoryIds, productCharacteristicTypes, true);

        return relatedProductCharacteristics;
    }

    private static async Task<ProductData.ProductPriceData?> GetProductPriceDataForPopupAsync(
        Currency? currency,
        decimal? price,
        ICurrencyConversionService currencyConversionService,
        ICurrencyVATService currencyVATService)
    {
        if (price is null || currency is null) return null;

        ProductData.DisplayPrice? displayPrice = null;
        ProductData.DisplayPrice? secondaryDisplayPrice = null;

        OneOf<decimal, OneOf.Types.NotFound> getPriceInEurResult = await currencyConversionService.ChangeCurrencyAsync(
            price.Value, currency.Value, Currency.EUR);

        OneOf<decimal, OneOf.Types.NotFound> getPriceInBgnResult = await currencyConversionService.ChangeCurrencyAsync(
            price.Value, currency.Value, Currency.BGN);

        if (getPriceInEurResult.IsT0)
        {
            decimal priceInEur = currencyVATService.AddVATToPriceUsingDefaultRate(getPriceInEurResult.AsT0, 1);

            displayPrice = new()
            {
                Amount = priceInEur,
                Currency = Currency.EUR,
            };
        }

        if (getPriceInBgnResult.IsT0)
        {
            decimal priceInBgn = currencyVATService.AddVATToPriceUsingDefaultRate(getPriceInBgnResult.AsT0, 1);

            secondaryDisplayPrice = new()
            {
                Amount = priceInBgn,
                Currency = Currency.BGN,
            };
        }

        if (displayPrice is null) return null;

        return new()
        {
            DisplayPrice = displayPrice.Value,
            SecondaryDisplayPrice = secondaryDisplayPrice,
        };
    }

    private static async Task<List<ProductData.ProductRelatedGroupPromotionDisplayData>> GetRelatedGroupPromotionsAsync(
        int productId,
        IGroupPromotionService groupPromotionService,
        IGroupPromotionProductBindingsService groupPromotionProductBindingsService,
        IGroupPromotionImageFileDataService groupPromotionImageFileDataService)
    {
        List<int> groupPromotionIds = await groupPromotionProductBindingsService.GetAllPromotionIdsBoundToProductAsync(productId);

        List<ProductData.ProductRelatedGroupPromotionDisplayData> productRelatedGroupPromotions = [];

        if (groupPromotionIds.Count <= 0)
        {
            return productRelatedGroupPromotions;
        }

        List<GroupPromotionContent> groupPromotionContents = await groupPromotionService.GetByIdsAsync(groupPromotionIds);

        List<IGrouping<int, GroupPromotionImageFileData>> promotionImageFiles
            = await groupPromotionImageFileDataService.GetAllInPromotionsAsync(groupPromotionIds);

        for (int i = 0; i < groupPromotionContents.Count; i++)
        {
            GroupPromotionContent groupPromotionContent = groupPromotionContents[i];

            IGrouping<int, GroupPromotionImageFileData>? imageFilesForPromotion
                = promotionImageFiles.Find(x => x.Key == groupPromotionContent.Id);

            string? htmlContent = groupPromotionContent.HtmlContent;

            if (string.IsNullOrEmpty(htmlContent)) continue;

            string? modifiedPromotionHtmlContent = groupPromotionService.ChangeLegacyUrlsToNewOnes(
                groupPromotionContent.HtmlContent,
                imageFilesForPromotion,
                promotionImageFile => $"{GroupPromotionImageFileEndpoints.EndpointGroupRoute}/{promotionImageFile.Id}"
            );

            if (string.IsNullOrEmpty(modifiedPromotionHtmlContent)) continue;

            ProductData.ProductRelatedGroupPromotionDisplayData productPromotionDisplayData = new()
            {
                HtmlContent = modifiedPromotionHtmlContent,
            };

            productRelatedGroupPromotions.Add(productPromotionDisplayData);
        }

        return productRelatedGroupPromotions;
    }

    private static async Task<IResult> GetProductLegacyDisplayPopupAsync(
        [FromRoute(Name = "productId")] int productId,
        [FromBody] ProductLegacyDisplayPopupRequest request,
        [FromServices] IProductService productService,
        [FromServices] IProductHtmlService productHtmlService,
        [FromServices] IProductCharacteristicService productCharacteristicService,
        [FromServices] IProductPropertyService productPropertyService)
    {
        MOSTComputers.Models.Product.Models.Product? product = await productService.GetByIdAsync(productId);

        if (product == null) return Results.NotFound();

        List<ProductCharacteristic> characteristics
            = await GetRelatedProductCharacteristicsAsync(productCharacteristicService, product.CategoryId);

        List<ProductProperty> properties = await productPropertyService.GetAllInProductAsync(productId);

        List<ProductData.ProductPropertyDisplayData> propertyData = [];

        List<HtmlProductProperty> htmlProductProperties = new();

        if (request.ProductProperties != null)
        {
            foreach (ProductLegacyPopupPropertyRequest productProperty in request.ProductProperties)
            {
                ProductCharacteristic? characteristic = characteristics.FirstOrDefault(
                    x => x.Id == productProperty.ProductCharacteristicId);

                if (characteristic == null) return Results.Problem(statusCode: StatusCodes.Status400BadRequest);

                ProductProperty? existingProperty = properties.FirstOrDefault(
                    x => x.ProductCharacteristicId == productProperty.ProductCharacteristicId);

                int? order = existingProperty?.DisplayOrder ?? characteristic.DisplayOrder;

                HtmlProductProperty htmlProductProperty = new()
                {
                    Name = characteristic.Name,
                    Value = productProperty.Value,
                    Order = order.ToString(),
                };

                htmlProductProperties.Add(htmlProductProperty);
            }
        }

        HtmlProduct htmlProduct = new()
        {
            Id = product.Id,
            Name = product.Name,
            Properties = htmlProductProperties,
        };

        HtmlProductsData htmlProductsData = new()
        {
            Products = [htmlProduct],
        };

        OneOf<string, InvalidXmlResult> getProductHtmlResult = productHtmlService.TryGetHtmlFromProducts(htmlProductsData);

        List<ProductHtmlAndImages.ProductImageDisplayData> productImageDisplayDatas = [];

        if (request.ProductImages != null)
        {
            foreach (ProductLegacyDisplayPopupImageRequest imageRequest in request.ProductImages)
            {
                ProductHtmlAndImages.ProductImageDisplayData imageData = new()
                {
                    ImageSrc = imageRequest.ImageUrl,
                    DisplayOrder = imageRequest.DisplayOrder,
                    Active = imageRequest.Active,
                };

                productImageDisplayDatas.Add(imageData);
            }
        }

        string? productHtml = getProductHtmlResult.Match<string?>(
            html => html,
            invalidXmlResult => null);

        return new RazorComponentResult<ProductLegacyDisplayPopup>(new
        {
            PopupData = new ProductLegacyDisplayPopup.ProductLegacyDisplayPopupData()
            {
                IsVisible = true,
                Product = product,
                ProductImages = productImageDisplayDatas,
                ProductHtml = productHtml,
            }
        });
    }

    private static RazorComponentResult<UnsavedChangesPopup> GetUnsavedChangesPopup()
    {
        return new RazorComponentResult<UnsavedChangesPopup>();
    }

    private static async Task<IResult> UpdateProductWorkStatusAsync(
        HttpContext httpContext,
        [FromServices] IProductService productService,
        [FromServices] IProductWorkStatusesService productWorkStatusesService,
        [FromRoute(Name = "productId")] int productId,
        [FromRoute(Name = "productNewStatus")] int productNewStatus)
    {
        if (!Enum.IsDefined(typeof(ProductNewStatus), productNewStatus))
        {
            return Results.BadRequest("Invalid product new status");
        }

        string currentUserName = httpContext.User.Identity!.Name!;

        ProductNewStatus productStatusParsed = (ProductNewStatus)productNewStatus;

        MOSTComputers.Models.Product.Models.Product? product = await productService.GetByIdAsync(productId);

        if (product == null)
        {
            return Results.NotFound();
        } 

        ProductWorkStatuses? existingProductWorkStatuses = await productWorkStatusesService.GetByProductIdAsync(productId);

        ServiceProductWorkStatusesUpsertRequest productWorkStatusesUpsertRequest = new()
        {
            ProductId = productId,
            ProductNewStatus = productStatusParsed,
            ProductXmlStatus = existingProductWorkStatuses?.ProductXmlStatus ?? ProductXmlStatus.NotReady,
            ReadyForImageInsert = existingProductWorkStatuses?.ReadyForImageInsert ?? false,
            UpsertUserName = currentUserName,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> updateStatusesResult
            = await productWorkStatusesService.UpsertByProductIdAsync(productWorkStatusesUpsertRequest);

        return updateStatusesResult.Match(
            statusId => Results.Json(new
            {
                productNewStatus = productNewStatus
            }, statusCode: 200),
            validationResult => Results.Json(validationResult, statusCode: 400),
            unexpectedFailureResult => Results.InternalServerError("An error has occured."));
    }
}
