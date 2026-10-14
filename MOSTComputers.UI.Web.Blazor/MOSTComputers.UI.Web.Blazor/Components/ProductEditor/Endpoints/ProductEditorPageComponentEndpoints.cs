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
    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "components/" + "productEditor";

    public static IEndpointConventionBuilder MapProductEditorPageComponentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute)
            .RequireAuthorization(Policies.Cookie)
            .RequireAuthorization(policy => policy.RequireRole("ProductEditor"));

        endpointGroup.MapGet("/productXmlDataPopup/{productId:int}", GetProductXmlPopupDataAsync);
        endpointGroup.MapGet("/productPromotionPopup/{productId:int}/{promotionId:int}", GetProductPromotionPopupDataAsync);
        endpointGroup.MapGet("/productInfoPromotionPopup/{productId:int}", GetProductInfoPromotionPopupDataAsync);
        endpointGroup.MapGet("/productImagesPopup/{productId:int}", GetProductImagesPopupDataAsync);
        endpointGroup.MapGet("/productImageFilesPopup/{productId:int}", GetProductImageFilesPopupDataAsync);
        endpointGroup.MapGet("/productPropertiesPopup/{productId:int}", GetProductPropertiesPopupDataAsync);
        endpointGroup.MapGet("/productSearchStringPopup/{productId:int}", GetProductSearchStringPopupDataAsync);

        SingleProductEditorComponentEndpoints.MapSingleProductEditorComponentEndpoints(endpointGroup);

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
}
