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
using MOSTComputers.UI.Web.Blazor.Components.ProductEditor.SingleProductEditorPopup;
using MOSTComputers.UI.Web.Blazor.Endpoints;
using MOSTComputers.UI.Web.Blazor.Endpoints.Documents;
using MOSTComputers.UI.Web.Blazor.Endpoints.Images;
using OneOf;
using static MOSTComputers.Services.ProductRegister.Utils.ImageUtils;
using static MOSTComputers.UI.Web.Blazor.Components.ProductEditor.SingleProductEditorPopup.EditorIdsGenerateUtils;
using static MOSTComputers.UI.Web.Blazor.Components.ProductEditor.SingleProductEditorPopup.SingleProductEditorPopup;
using static MOSTComputers.UI.Web.Blazor.Utils.AuthenticationUtils;

namespace MOSTComputers.UI.Web.Blazor.Components.ProductEditor.Endpoints;

internal static class SingleProductEditorComponentEndpoints
{
    private sealed class ProductDataPopupRequest
    {
        [JsonPropertyName("productProperties")]
        public List<ProductPropertyDisplayData>? ProductProperties { get; init; }

        [JsonPropertyName("productLinks")]
        public List<ProductPropertyDisplayData>? ProductLinks { get; init; }

        [JsonPropertyName("productImages")]
        public List<ProductImageListItemDisplayData>? ProductImages { get; init; }
    }

    private sealed class ProductLegacyDisplayPopupRequest
    {
        [JsonPropertyName("productProperties")]
        public List<ProductPropertyDisplayData>? ProductProperties { get; init; }

        [JsonPropertyName("productLinks")]
        public List<ProductPropertyDisplayData>? ProductLinks { get; init; }

        [JsonPropertyName("productImages")]
        public List<ProductImageListItemDisplayData>? ProductImages { get; init; }
    }

    private sealed class PromotionProductFilePopupRequest
    {
        [JsonPropertyName("promotionProductFiles")]
        public List<PromotionProductFileInfoDisplayData>? PromotionProductFiles { get; init; }
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

        endpointGroup.MapPut("/updateProductWorkStatus/{productId:int}/{productNewStatus:int}",
            UpdateProductWorkStatusAsync);

        SingleProductEditorDocumentComponentEndpoints.MapSingleProductEditorDocumentComponentEndpoints(endpointGroup);
        SingleProductEditorImageComponentEndpoints.MapSingleProductEditorImageComponentEndpoints(endpointGroup);
        SingleProductEditorPromotionFileComponentEndpoints.MapSingleProductEditorPromotionFileComponentEndpoints(endpointGroup);

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
        // await RevokeAllExistingBrowserFilePreviewUrls();

        // _changedProperties.Clear();
        // _changedImages.Clear();
        // _changedPromotionFiles.Clear();
        // _changedDocuments.Clear();

        MOSTComputers.Models.Product.Models.Product? product = await productService.GetByIdAsync(productId);

        if (product == null) return Results.NotFound();

        List<ProductImageData> productImages = await productImageService.GetAllInProductWithoutFileDataAsync(product.Id);

        List<ProductImageFileData> productImageFiles = await productImageFileService.GetAllInProductAsync(product.Id);

        List<ProductImageFileDisplayData> productImageFileDisplayDatas = [];

        foreach (ProductImageFileData productImageFile in productImageFiles)
        {
            ProductImageFileDisplayData productImageFileDisplayData = new()
            {
                ExistingFileInfoId = productImageFile.Id,
                ExistingImageId = productImageFile.ImageId,
                FileName = productImageFile.FileName,
                DisplayOrder = productImageFile.DisplayOrder,
                Active = productImageFile.Active,
            };

            productImageFileDisplayDatas.Add(productImageFileDisplayData);
        }

        List<PromotionProductFileInfo> promotionProductFileInfos = await promotionProductFileService.GetAllForProductAsync(productId);

        // ProductImages.Clear();

        List<ProductImageListItemDisplayData> productImageDatas = [];

        // List<PromotionProductFileEditorPopup.PromotionProductFileInfoDisplayData> promotionProductFileInfoDisplayDatas
        //     = PromotionProductFileEditorPopup.MapRangeToNewEditorItems(promotionProductFileInfos);

        List<PromotionProductFileInfoDisplayData> promotionProductFileInfoDisplayDatas = [];

        foreach (PromotionProductFileInfo promotionProductFileInfo in promotionProductFileInfos)
        {
            PromotionProductFileInfoDisplayData promotionFileDisplayData = new()
            {
                EditorId = GeneratePromotionFileId(promotionProductFileInfo.Id),
                Id = promotionProductFileInfo.Id,
                FileInfoId = promotionProductFileInfo.PromotionFileInfoId,
                ProductImageId = promotionProductFileInfo.ProductImageId,
                FileInfoName = promotionProductFileInfo.PromotionFileInfo.Name,
                FileInfoFileName = promotionProductFileInfo.PromotionFileInfo.FileName,
                ValidFrom = promotionProductFileInfo.ValidFrom,
                ValidTo = promotionProductFileInfo.ValidTo,
                Active = promotionProductFileInfo.Active,
            };
            
            promotionProductFileInfoDisplayDatas.Add(promotionFileDisplayData);
        }

        for (int i = 0; i < productImages.Count; i++)
        {
            ProductImageData productImage = productImages[i];

            ProductImageFileDisplayData? relatedImageFile = productImageFileDisplayDatas
                .FirstOrDefault(x => x.ExistingImageId == productImage.Id);

            if (relatedImageFile is not null)
            {
                productImageFileDisplayDatas.Remove(relatedImageFile);
            }

            PromotionProductFileInfoDisplayData? relatedPromotionFile
                = promotionProductFileInfoDisplayDatas.FirstOrDefault(x => x.ProductImageId == productImage.Id);

            ProductImageDisplayData productImageDisplayData = new()
            {
                ExistingImageId = productImage.Id,
                ContentType = productImage.ImageContentType,
                DateModified = productImage.DateModified,
            };

            string editorId = GenerateImageId(
                productImage.Id,
                relatedImageFile?.ExistingFileInfoId,
                relatedPromotionFile?.Id);

            string? imageFileUrl = GetImageFileUrlFromListItem(productImageDisplayData, relatedImageFile, relatedPromotionFile);

            ProductImageListItemDisplayData productImageListItemDisplayData = new()
            {
                EditorId = editorId,
                Index = i,
                ImageFileUrl = imageFileUrl!,
                RelatedImage = productImageDisplayData,
                RelatedImageFile = relatedImageFile,
                RelatedPromotionFileEditorId = relatedPromotionFile?.EditorId,
            };

            productImageDatas.Add(productImageListItemDisplayData);
        }

        foreach (ProductImageFileDisplayData productImageFile in productImageFileDisplayDatas)
        {
            string editorId = GenerateImageId(
                null, productImageFile?.ExistingFileInfoId, null);

            string? imageFileUrl = GetImageFileUrlFromListItem(
                null,
                productImageFile,
                null);

            ProductImageListItemDisplayData productImageListItemDisplayData = new()
            {
                EditorId = editorId,
                Index = productImageDatas.Count,
                ImageFileUrl = imageFileUrl!,
                RelatedImage = null,
                RelatedImageFile = productImageFile,
                RelatedPromotionFileEditorId = null,
            };

            productImageDatas.Add(productImageListItemDisplayData);
        }

        productImageDatas = OrderImageRelatedItems(productImageDatas,
            imageListItem => imageListItem.RelatedImageFile?.DisplayOrder,
            imageListItem => imageListItem.RelatedImage?.ExistingImageId);

        List<ProductCharacteristic> relatedProductCharacteristics = await GetRelatedProductCharacteristicsAsync(
            productCharacteristicService, product.CategoryId);

        List<ProductPropertyDisplayData> propertyEditorDataList = await GetProductPropertyEditorDataForProductAsync(
            product.Id, relatedProductCharacteristics, productPropertyService);

        List<ProductPropertyDisplayData> productProperties = propertyEditorDataList
            .Where(x => x.ProductCharacteristic?.KWPrCh == ProductCharacteristicType.ProductCharacteristic)
            .ToList();

        List<ProductPropertyDisplayData> productLinks = propertyEditorDataList
            .Where(x => x.ProductCharacteristic?.KWPrCh == ProductCharacteristicType.Link)
            .ToList();

        List<Promotion> productPromotions = await promotionService.GetAllForProductAsync(product.Id);

        List<SearchStringPartOriginData> searchStringParts = await searchStringOriginService.GetSearchStringPartsAndDataAboutTheirOriginAsync(
            product.SearchString, product.CategoryId)
            ?? new();

        List<ProductDocument> productDocuments = await productDocumentFileService.GetAllForProductAsync(product.Id);

        List<ProductDocumentDisplayData> productDocumentDatas = [];

        for (int i = 0; i < productDocuments.Count; i++)
        {
            ProductDocument productDocument = productDocuments[i];

            ProductDocumentDisplayData productDocumentDisplayData = new()
            {
                EditorId = GenerateDocumentId(productDocument.Id),
                Index = i,
                ExistingDocument = productDocument,
                BrowserFilePath = $"{ProductDocumentEndpoints.EndpointGroupRoute}/{productDocument.Id}",
                CurrentDescription = productDocument.Description ?? string.Empty,
                FileExtension = ".pdf",
            };

            productDocumentDatas.Add(productDocumentDisplayData);
        }

        ProductWorkStatuses? productStatuses = await productWorkStatusesService.GetByProductIdAsync(product.Id);

        var parameters = new
        {
            product = product,
            productStatuses = productStatuses,
            productProperties = productProperties,
            productLinks = productLinks,
            productImages = productImages,
            promotionProductFiles = promotionProductFileInfoDisplayDatas,
            searchStringParts = searchStringParts,
            productDocuments = productDocuments,
            productPromotions = productPromotions,
        };

        string parametersJsonData = JsonSerializer.Serialize(parameters, _jsonDataOptions);

        var output = new
        {
            Product = product,
            ProductStatuses = productStatuses,
            ProductProperties = productProperties,
            ProductLinks = productLinks,
            ProductImages = productImages,
            PromotionProductFiles = promotionProductFileInfoDisplayDatas,
            SearchStringParts = searchStringParts,
            ProductDocuments = productDocuments,
            ProductPromotions = productPromotions,
            JsonData = parametersJsonData,
        };

        return new RazorComponentResult<SingleProductEditorPopup.SingleProductEditorPopup>(output);
    }

    private static async Task<List<ProductPropertyDisplayData>> GetProductPropertyEditorDataForProductAsync(
        int productId,
        List<ProductCharacteristic> productCharacteristics,
        [FromServices] IProductPropertyService ProductPropertyService)
    {
        List<ProductProperty> productProperties = await ProductPropertyService.GetAllInProductAsync(productId);

        List<ProductPropertyDisplayData> properties = [];

        List<ProductPropertyDisplayData> propertyEditorDataList = [];

        for (int i = 0; i < productCharacteristics.Count; i++)
        {
            ProductCharacteristic productCharacteristic = productCharacteristics[i];

            ProductProperty? property = productProperties.Find(
                x => x.ProductCharacteristicId == productCharacteristic.Id);

            bool doesPropertyExist = property != null;

            string editorId = GeneratePropertyId(
                productCharacteristic.Id, productCharacteristic.KWPrCh == ProductCharacteristicType.Link);

            ProductPropertyDisplayData propertyData = new()
            {
                EditorId = editorId,
                IsActive = doesPropertyExist,
                ProductCharacteristic = productCharacteristic,
                DisplayOrder = property?.DisplayOrder,
                Value = property?.Value,
            };

            properties.Add(propertyData);
        }

        properties.Sort((property1, property2) =>
        {
            return (property1.ProductCharacteristic?.DisplayOrder ?? 0) - (property2.ProductCharacteristic?.DisplayOrder ?? 0);
        });

        return properties;
    }

    private static async Task<List<ProductCharacteristic>> GetRelatedProductCharacteristicsAsync(
        IProductCharacteristicService ProductCharacteristicService,
        int? categoryId)
    {
        List<int> relatedCategoryIds = [-1];

        if (categoryId is not null)
        {
            relatedCategoryIds.Add(categoryId.Value);
        }

        List<ProductCharacteristicType> productCharacteristicTypes = [ProductCharacteristicType.ProductCharacteristic, ProductCharacteristicType.Link];

        List<ProductCharacteristic> relatedProductCharacteristics = await ProductCharacteristicService.GetAllByCategoryIdsAndTypesAsync(
            relatedCategoryIds, productCharacteristicTypes, true);

        return relatedProductCharacteristics;
    }

    private static string? GetImageFileUrlFromListItem(
        ProductImageDisplayData? relatedImage,
        ProductImageFileDisplayData? relatedImageFile,
        PromotionProductFileInfoDisplayData? relatedPromotionFile)
    {
        string? imageDataRoute = null;

        if (relatedImage?.ExistingImageId is not null)
        {
            imageDataRoute = $"{ProductImageDataEndpoints.EndpointGroupRoute}/{relatedImage?.ExistingImageId}";
        }
        else if (relatedImageFile?.FileName is not null)
        {
            imageDataRoute = $"{ProductImageFileDataEndpoints.EndpointGroupRoute}/{relatedImageFile.FileName}";
        }
        else if (relatedPromotionFile?.FileInfoFileName is not null)
        {
            imageDataRoute = $"{PromotionFileDataEndpoints.EndpointGroupRoute}/{relatedPromotionFile.FileInfoFileName}";
        }

        return imageDataRoute;
    } 

    private static async Task<IResult> GetProductDataPopupAsync(
        HttpContext httpContext,
        [FromRoute(Name = "productId")] int productId,
        [FromBody] ProductDataPopupRequest request,
        [FromServices] IProductService productService,
        [FromServices] IPromotionService promotionService,
        [FromServices] ISearchStringOriginService searchStringOriginService,
        [FromServices] IGroupPromotionService groupPromotionService,
        [FromServices] IGroupPromotionProductBindingsService groupPromotionProductBindingsService,
        [FromServices] IGroupPromotionImageFileDataService groupPromotionImageFileDataService,
        [FromServices] ICurrencyConversionService currencyConversionService,
        [FromServices] ICurrencyVATService currencyVATService)
    {
        MOSTComputers.Models.Product.Models.Product? product = await productService.GetByIdAsync(productId);

        if (product == null) return Results.NotFound();

        List<ProductPropertyDisplayData> propertiesAndLinks = new();

        if (request.ProductProperties != null)
        {
            propertiesAndLinks.AddRange(request.ProductProperties.Where(x => x.IsActive));
        }

        if (request.ProductLinks != null)
        {
            propertiesAndLinks.AddRange(request.ProductLinks.Where(x => x.IsActive));
        }

        List<ProductData.ProductPropertyDisplayData> propertyData = [];

        foreach (ProductPropertyDisplayData productProperty in propertiesAndLinks)
        {
            if (productProperty.ProductCharacteristic is null
                || !productProperty.IsActive)
            {
                continue;
            }

            ProductData.ProductPropertyDisplayData displayData = new()
            {
                ProductCharacteristic = new()
                {
                    Id = productProperty.ProductCharacteristic.Id,
                    CategoryId = productProperty.ProductCharacteristic.CategoryId,
                    Active = productProperty.ProductCharacteristic.Active,
                    KWPrCh = productProperty.ProductCharacteristic.KWPrCh
                },
                Name = productProperty.ProductCharacteristic.Name,
                Value = productProperty.Value,
                DisplayOrder = productProperty.DisplayOrder
            };

            propertyData.Add(displayData);
        }

        List<ProductData.ProductImageDisplayData> productImages = [];

        if (request.ProductImages != null)
        {
            foreach (ProductImageListItemDisplayData imageListItem in request.ProductImages)
            {
                ProductData.ProductImageDisplayData imageData = new()
                {
                    ImageSrc = imageListItem.ImageFileUrl,
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
        [FromServices] IProductHtmlService productHtmlService)
    {
        MOSTComputers.Models.Product.Models.Product? product = await productService.GetByIdAsync(productId);

        if (product == null) return Results.NotFound();

        List<ProductPropertyDisplayData> propertiesAndLinks = [];

        if (request.ProductProperties != null)
        {
            propertiesAndLinks.AddRange(request.ProductProperties.Where(x => x.IsActive));
        }

        if (request.ProductLinks != null)
        {
            propertiesAndLinks.AddRange(request.ProductLinks.Where(x => x.IsActive));
        }

        OneOf<string, InvalidXmlResult> getProductHtmlResult = GetProductHtmlFromCurrentEditorData(
            product, propertiesAndLinks, productHtmlService);

        List<ProductHtmlAndImages.ProductImageDisplayData> productImageDisplayDatas = [];

        if (request.ProductImages != null)
        {
            foreach (ProductImageListItemDisplayData imageListItem in request.ProductImages)
            {
                ProductHtmlAndImages.ProductImageDisplayData imageData = new()
                {
                    ImageSrc = imageListItem.ImageFileUrl,
                    DisplayOrder = imageListItem.RelatedImageFile?.DisplayOrder,
                    Active = imageListItem.RelatedImageFile?.Active,
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

    private static OneOf<string, InvalidXmlResult> GetProductHtmlFromCurrentEditorData(
        MOSTComputers.Models.Product.Models.Product product,
        List<ProductPropertyDisplayData> includedProperties,
        IProductHtmlService productHtmlService)
    {
        List<HtmlProductProperty> htmlProductProperties = new();

        foreach (ProductPropertyDisplayData productProperty in includedProperties.OrderBy(x => x.DisplayOrder ?? int.MaxValue))
        {
            HtmlProductProperty htmlProductProperty = new()
            {
                Name = productProperty.ProductCharacteristic?.Name,
                Value = productProperty.Value,
                Order = productProperty.DisplayOrder?.ToString(),
            };

            htmlProductProperties.Add(htmlProductProperty);
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

        return productHtmlService.TryGetHtmlFromProducts(htmlProductsData);
    }

    private static async Task<IResult> ShowPromotionProductFilePopupAsync(
        [FromRoute] int productId,
        [FromBody] PromotionProductFilePopupRequest request)
    {

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

        string? currentUserName = httpContext.User.Identity?.Name;

        if (currentUserName is null)
        {
            return Results.Forbid();
        }

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
