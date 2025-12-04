using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using static MOSTComputers.Utils.Files.ContentTypeUtils;

namespace MOSTComputers.UI.Web.Blazor.Endpoints.Images;

public static class ProductImageDataEndpoints
{
    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "images/" + "originalImageData";

    public static IEndpointConventionBuilder MapProductImageDataEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute);

        endpointGroup.MapGet("/{imageId}", GetProductImageAsync);

        return endpointGroup;
    }

    private static async Task<IResult> GetProductImageAsync(
        [FromRoute] int imageId,
        [FromServices] IProductImageService productImageService)
    {
        ProductImage? productImage = await productImageService.GetByIdInAllImagesAsync(imageId);

        if (productImage is null)
        {
            return Results.NotFound(imageId);
        }

        string? contentType = productImage.ImageContentType;

        bool isKnownContentType = IsKnownContentType(contentType);

        if (!isKnownContentType)
        {
            contentType = "application/octet-stream";
        }

        byte[] imageData = productImage.ImageData ?? Array.Empty<byte>();

        return Results.File(imageData, contentType!);
    }
}