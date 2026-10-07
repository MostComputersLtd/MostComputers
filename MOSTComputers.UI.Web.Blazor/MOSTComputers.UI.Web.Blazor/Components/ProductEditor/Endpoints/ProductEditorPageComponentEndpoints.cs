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
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductWorkStatuses;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductProperties.Contacts;
using MOSTComputers.Services.ProductRegister.Services.ProductStatus.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.PromotionFiles.Contracts;
using MOSTComputers.Services.SearchStringOrigin.Models;
using MOSTComputers.Services.SearchStringOrigin.Services.Contracts;
using MOSTComputers.UI.Web.Blazor.Components.Product;
using MOSTComputers.UI.Web.Blazor.Components.ProductEditor.SingleProductEditorPopup;
using MOSTComputers.UI.Web.Blazor.Endpoints;
using MOSTComputers.UI.Web.Blazor.Endpoints.Documents;
using MOSTComputers.UI.Web.Blazor.Endpoints.Images;
using OneOf;
using SixLabors.ImageSharp;
using static MOSTComputers.Services.ProductRegister.Utils.ImageUtils;
using static MOSTComputers.UI.Web.Blazor.Components.ProductEditor.SingleProductEditorPopup.EditorIdsGenerateUtils;
using static MOSTComputers.UI.Web.Blazor.Components.ProductEditor.SingleProductEditorPopup.SingleProductEditorPopup;
using static MOSTComputers.UI.Web.Blazor.Utils.AuthenticationUtils;
using static MOSTComputers.Utils.Files.ContentTypeUtils;
using static MOSTComputers.Utils.Files.FilePathUtils;

namespace MOSTComputers.UI.Web.Blazor.Components.ProductEditor.Endpoints;

internal static class ProductEditorPageComponentEndpoints
{
    private sealed class ProductDocumentPopupRequest
    {
        public string? FileUrl { get; set; }
    }

    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "components/" + "productEditor";

    private static readonly JsonSerializerOptions _jsonDataOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.Default,
    };

    private const string _filePreviewUrlFormFieldName = "previewUrl";
    private const string _fileIndexFormFieldName = "index";
    private const string _productImageFileWidthFormFieldName = "width";
    private const string _productImageFileHeightFormFieldName = "height";

    private const int _productImageFileUploadSize = 20 * 1024 * 1024;

    private static readonly string[] _validIFrameUriSchemes = ["http", "https", "blob"];

    public static IEndpointConventionBuilder MapProductEditorPageComponentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute)
            .RequireAuthorization(Policies.Cookie)
            .RequireAuthorization(policy => policy.RequireRole("ProductEditor"));

        endpointGroup.MapPost("/productXmlDataPopup/{productId:int})", GetProductXmlPopupDataAsync);
        endpointGroup.MapPost("/productPromotionPopup/{productId:int}/{promotionId:int}", GetProductPromotionPopupDataAsync);
        endpointGroup.MapPost("/productInfoPromotionPopup/{productId:int}", GetProductInfoPromotionPopupDataAsync);
        endpointGroup.MapPost("/productImagesPopup/{productId:int}", GetProductImagesPopupDataAsync);
        endpointGroup.MapPost("/productImageFilesPopup/{productId:int}", GetProductImageFilesPopupDataAsync);
        endpointGroup.MapPost("/productPropertiesPopup/{productId:int}", GetProductPropertiesPopupDataAsync);
        endpointGroup.MapPost("/productSearchStringPopup/{productId:int}", GetProductSearchStringPopupDataAsync);

        endpointGroup.MapPost("/single/{productId:int}", GetSingleProductEditorPopupAsync);
        endpointGroup.MapGet("/single/productDocumentPopup/{productId:int}",
            GetProductDocumentPopupDataAsync);

        endpointGroup.MapPost("/single/addImage", (Delegate)GetProductImageFromExternalFileAsync);

        endpointGroup.MapPut("/single/updateProductWorkStatus/{productId:int}/{productNewStatus:int}",
            UpdateProductWorkStatusAsync);

        return endpointGroup;
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

    private static async Task<IResult> GetSingleProductEditorPopupAsync(
        [FromRoute(Name = "productId")] int productId,
        [FromServices] IProductService productService,
        [FromServices] IProductPropertyService ProductPropertyService,
        [FromServices] IProductCharacteristicService ProductCharacteristicService,
        [FromServices] IProductImageService ProductImageService,
        [FromServices] IProductImageFileService ProductImageFileService,
        [FromServices] IProductWorkStatusesService ProductWorkStatusesService,
        [FromServices] IPromotionService PromotionService,
        [FromServices] IPromotionProductFileService PromotionProductFileService,
        [FromServices] IProductDocumentFileService ProductDocumentFileService,
        [FromServices] ISearchStringOriginService SearchStringOriginService)
    {
        // await RevokeAllExistingBrowserFilePreviewUrls();

        // _changedProperties.Clear();
        // _changedImages.Clear();
        // _changedPromotionFiles.Clear();
        // _changedDocuments.Clear();

        MOSTComputers.Models.Product.Models.Product? product = await productService.GetByIdAsync(productId);

        if (product == null) return Results.NotFound();

        List<ProductImageData> productImages = await ProductImageService.GetAllInProductWithoutFileDataAsync(product.Id);

        List<ProductImageFileData> productImageFiles = await ProductImageFileService.GetAllInProductAsync(product.Id);

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

        List<PromotionProductFileInfo> promotionProductFileInfos = await PromotionProductFileService.GetAllForProductAsync(productId);

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

            ProductImageListItemDisplayData productImageListItemDisplayData = new()
            {
                EditorId = editorId,
                Index = i,
                RelatedImage = productImageDisplayData,
                RelatedImageFile = relatedImageFile,
                RelatedPromotionFile = relatedPromotionFile,
            };

            productImageDatas.Add(productImageListItemDisplayData);
        }

        foreach (ProductImageFileDisplayData productImageFile in productImageFileDisplayDatas)
        {
            string editorId = GenerateImageId(
                null, productImageFile?.ExistingFileInfoId, null);

            ProductImageListItemDisplayData productImageListItemDisplayData = new()
            {
                EditorId = editorId,
                Index = productImageDatas.Count,
                RelatedImage = null,
                RelatedImageFile = productImageFile,
                RelatedPromotionFile = null
            };

            productImageDatas.Add(productImageListItemDisplayData);
        }

        productImageDatas = OrderImageRelatedItems(productImageDatas,
            imageListItem => imageListItem.RelatedImageFile?.DisplayOrder,
            imageListItem => imageListItem.RelatedImage?.ExistingImageId);

        List<ProductCharacteristic> relatedProductCharacteristics = await GetRelatedProductCharacteristicsAsync(
            ProductCharacteristicService, product.CategoryId);

        List<ProductPropertyDisplayData> propertyEditorDataList = await GetProductPropertyEditorDataForProductAsync(
            product.Id, relatedProductCharacteristics, ProductPropertyService);

        List<ProductPropertyDisplayData> productProperties = propertyEditorDataList
            .Where(x => x.ProductCharacteristic?.KWPrCh == ProductCharacteristicType.ProductCharacteristic)
            .ToList();

        List<ProductPropertyDisplayData> productLinks = propertyEditorDataList
            .Where(x => x.ProductCharacteristic?.KWPrCh == ProductCharacteristicType.Link)
            .ToList();

        List<Promotion> productPromotions = await PromotionService.GetAllForProductAsync(product.Id);

        List<SearchStringPartOriginData> searchStringParts = await SearchStringOriginService.GetSearchStringPartsAndDataAboutTheirOriginAsync(
            product.SearchString, product.CategoryId)
            ?? new();

        List<ProductDocument> productDocuments = await ProductDocumentFileService.GetAllForProductAsync(product.Id);

        List<ProductDocumentDisplayData> productDocumentDatas = [];

        foreach (ProductDocument productDocument in productDocuments)
        {
            Stream? fileStream = ProductDocumentFileService.GetFileStreamByFileName(productDocument.FileName);

            byte[]? fileData = null;

            if (fileStream is not null)
            {
                fileData = new byte[fileStream.Length];

                await fileStream.ReadAsync(fileData);
            }

            ProductDocumentDisplayData productDocumentDisplayData = new()
            {
                EditorId = GenerateDocumentId(productDocument.Id),
                ExistingDocument = productDocument,
                CurrentDescription = productDocument.Description ?? string.Empty,
                FileExtension = ".pdf",
            };

            productDocumentDatas.Add(productDocumentDisplayData);
        }

        ProductWorkStatuses? productStatuses = await ProductWorkStatusesService.GetByProductIdAsync(product.Id);

        var parameters = new
        {
            product = product,
            productStatuses = productStatuses,
            productProperties = productProperties,
            productLinks = productLinks,
            productImages = productImages,
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

    private static async Task<IResult> GetProductImageFromExternalFileAsync(HttpContext httpContext)
    {
        IFormCollection form = httpContext.Request.Form;

        StringValues previewUrlValue = form[_filePreviewUrlFormFieldName];

        if (previewUrlValue.Count == 0)
        {
            return Results.BadRequest("A preview url is required");
        }

        string previewUrl = previewUrlValue[0]!;

        StringValues indexValue = form[_fileIndexFormFieldName];

        if (indexValue.Count == 0
            || !int.TryParse(indexValue[0], out int index))
        {
            return Results.BadRequest("An index is required");
        }

        StringValues imageWidthValue = form[_productImageFileWidthFormFieldName];

        if (imageWidthValue.Count == 0
            || !int.TryParse(imageWidthValue[0], out int imageWidth))
        {
            return Results.BadRequest("An image width is required");
        }

        StringValues imageHeightValue = form[_productImageFileHeightFormFieldName];

        if (imageHeightValue.Count == 0
            || !int.TryParse(imageHeightValue[0], out int imageHeight))
        {
            return Results.BadRequest("An image height is required");
        }

        IFormFileCollection? selectedFiles = form.Files;

        if (selectedFiles == null || selectedFiles.Count == 0)
        {
            return Results.BadRequest("A file is required");
        }

        IFormFile? file = selectedFiles[0];

        if (file is null || file.Length == 0)
        {
            return Results.BadRequest("A file is required");
        }

        if (!IsImageContentType(file.ContentType))
        {
            return Results.BadRequest("File is not an image file");
        }

        ProductImageFileDisplayData productImageFileDisplayData = new()
        {
            ExistingFileInfoId = null,
            ExistingImageId = null,
            FileName = null,
            DisplayOrder = index,
            Active = true
        };

        ProductImageDisplayData productImageDisplayData = new()
        {
            ExistingImageId = null,
            ContentType = file.ContentType,
            DateModified = null,
        };

        ProductImageListItemDisplayData productImageListItemDisplayData = new()
        {
            EditorId = $"from-browser-file-{Guid.NewGuid()}",
            Index = index,
            ImageFileUrl = previewUrl,
            UploadedImageFileData = new()
            {
                FileName = file.Name,
                ContentType = file.ContentType,
                FileSize = file.Length,
                ImageSize = new()
                {
                    WidthPixels = imageWidth,
                    HeightPixels = imageHeight,
                },
                PreviewUrl = previewUrl,
            },

            RelatedImage = productImageDisplayData,
            RelatedImageFile = productImageFileDisplayData,
            RelatedPromotionFile = null
        };

        var parameters = new
        {
            ItemData = productImageListItemDisplayData,
        };

        string parametersJsonData = JsonSerializer.Serialize(parameters, _jsonDataOptions);

        var output = new
        {
            ItemData = productImageListItemDisplayData,
            JsonData = parametersJsonData,
        };

        return new RazorComponentResult<ProductImageListItem>(output);
    }

    private static string? GetImageDataRouteFromListItem(
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

    private static async Task<IResult> GetProductDataPopup(
        [FromRoute(Name = "productId")] int productId)
    {
        List<ProductData.ProductPropertyDisplayData> productProperties = [];

        List<ProductProperty> ProductProperties = await productPropertyService.GetAllInProductAsync(productId);

        List<SingleProductEditorPopup.SingleProductEditorPopup.ProductPropertyDisplayData> allProductProperties = new(ProductProperties);

        foreach (SingleProductEditorPopup.SingleProductEditorPopup.ProductPropertyDisplayData productProperty in allProductProperties)
        {
            if (productProperty.ProductCharacteristic is null
                || !productProperty.IsActive)
            {
                continue;
            }

            ProductData.ProductPropertyDisplayData displayData = new ()
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

            productProperties.Add(displayData);
        }

        List<ProductData.ProductImageDisplayData> productImages = ProductImages.Select(imageListItem =>
        {
            return new ProductData.ProductImageDisplayData()
            {
                ImageSrc = GetImageDataRouteFromListItem(
                    imageListItem.RelatedImage,
                    imageListItem.RelatedImageFile,
                    imageListItem.RelatedPromotionFile)!,
            };
        })
        .ToList();

        ProductPriceData? productPriceData = await GetProductPriceDataForPopupAsync();

        List<Promotion> promotions = ProductPromotions.Where(x =>
        {
            if (x.Id != Product.PromotionPid && x.Id != Product.PromotionRid) return false;

            return (x.StartDate is null || x.StartDate <= DateTime.Now) && (x.ExpirationDate is null || x.ExpirationDate >= DateTime.Now);
        })
            .ToList();

        _productDataPopupData = new()
        {
            Product = Product,
            ProductDataPopupPriceData = productPriceData,
            ProductProperties = productProperties,
            ProductImages = productImages,
            ProductSearchStringParts = SearchStringParts,
            Promotions = promotions,
        };

        IsProductDataPopupVisible = true;
    }

    private static async Task<IResult> GetProductDocumentPopupDataAsync(
        [FromServices] IProductService productService,
        [FromBody] ProductDocumentPopupRequest productDocumentPopupRequest,
        [FromRoute(Name = "productId")] int productId)
    {
        MOSTComputers.Models.Product.Models.Product? product = await productService.GetByIdAsync(productId);

        if (product == null)
        {
            return Results.NotFound();
        }

        if (productDocumentPopupRequest == null
            || string.IsNullOrWhiteSpace(productDocumentPopupRequest.FileUrl)
            || !Uri.TryCreate(productDocumentPopupRequest.FileUrl, UriKind.RelativeOrAbsolute, out Uri? fileUri))
        {
            return Results.BadRequest("Missing or invalid image url");
        }

        if (fileUri.IsAbsoluteUri
            && !_validIFrameUriSchemes.Contains(fileUri.Scheme))
        {
            return Results.BadRequest("Missing or invalid image url");
        }

        return new RazorComponentResult<ProductDocumentPopup>(new
        {
            PopupData = new ProductDocumentPopup.ProductDocumentPopupData()
            {
                IsVislble = true,
                Product = product,
                DocumentPath = productDocumentPopupRequest.FileUrl,
            }
        });
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
