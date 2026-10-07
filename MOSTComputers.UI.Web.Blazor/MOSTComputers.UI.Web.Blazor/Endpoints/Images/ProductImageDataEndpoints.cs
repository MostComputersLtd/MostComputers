using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using static MOSTComputers.UI.Web.Blazor.Utils.AuthenticationUtils;
using static MOSTComputers.Utils.Files.ContentTypeUtils;

namespace MOSTComputers.UI.Web.Blazor.Endpoints.Images;

public static class ProductImageDataEndpoints
{
    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "images/" + "originalImageData";

    public static IEndpointConventionBuilder MapProductImageDataEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute);

        endpointGroup.MapGet("/{imageId}", GetProductImageAsync)
            .RequireAuthorization(Policies.Cookie)
            .WithMetadata(new IncludeInOpenApiSpecMetadata())
            .WithTags("Product Image Data")
            .WithName("GetProductImage")
            .WithSummary("Returns a product image")
            .WithDescription("Returns the image data for the specified product image.")
            .AddOpenApiOperationTransformer((operation, context, cancellationToken) =>
            {
                OpenApiResponses currentResponses = operation.Responses ?? new();

                operation.Responses = new()
                {
                    ["200"] = new OpenApiResponse()
                    {
                        Description = "The image data for the specified product image.",
                        Content = new Dictionary<string, OpenApiMediaType>()
                        {
                            ["image/*"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                {
                                    Type = JsonSchemaType.String,
                                    Format = "binary"
                                }
                            }
                        }
                    }
                };

                foreach (KeyValuePair<string, IOpenApiResponse> kvp in currentResponses)
                {
                    operation.Responses.Add(kvp.Key, kvp.Value);
                }

                return Task.CompletedTask;
            });

        return endpointGroup;
    }

    [ProducesResponseType(404, Description = "The specified product image does not exist.")]
    private static async Task<IResult> GetProductImageAsync(
        [FromRoute(Name = "imageId")]
        [Description("The ID of the product image.")]
        int imageId,
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

        return Results.File(imageData, contentType);
    }
}
