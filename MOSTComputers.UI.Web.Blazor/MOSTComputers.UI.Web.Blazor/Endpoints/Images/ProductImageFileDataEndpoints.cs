using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.ProductImageFileManagement.Services.Contracts;
using OneOf;
using static MOSTComputers.Utils.Files.ContentTypeUtils;

namespace MOSTComputers.UI.Web.Blazor.Endpoints.Images;

public static class ProductImageFileDataEndpoints
{
    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "images/" + "imageFileData";

    public static IEndpointConventionBuilder MapProductImageFileDataEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute);

        endpointGroup.MapGet("/{fullFileName}", GetProductImageFile)
            .AllowAnonymous()
            .WithMetadata(new IncludeInOpenApiSpecMetadata())
            .WithTags("Product Image File Data")
            .WithName("GetProductImageFile")
            .WithSummary("Returns a product image file")
            .WithDescription("Returns the image file identified by the specified file name.")
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
                            },
                            ["application/octet-stream"] = new OpenApiMediaType
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

    [ProducesResponseType(400, Description = "The file name is null or empty.")]
    [ProducesResponseType(404, Description = "The specified product image file does not exist.")]
    private static IResult GetProductImageFile(
        [FromRoute(Name = "fullFileName")]
        [Description("The full file name of the product image.")]
        string fullFileName,
        [FromServices] IProductImageFileManagementService productImageFileManagementService)
    {
        if (string.IsNullOrWhiteSpace(fullFileName))
        {
            return Results.BadRequest("The file name cannot be null or empty.");
        }

        fullFileName = Path.GetFileName(fullFileName);

        string? contentType = GetContentTypeFromExtension(fullFileName);

        if (string.IsNullOrWhiteSpace(contentType))
        {
            contentType = "application/octet-stream";
        }

        OneOf<Stream, FileDoesntExistResult> getImageFileDataResult
            = productImageFileManagementService.GetImageStream(fullFileName);

        return getImageFileDataResult.Match(
            fileStream =>
            {
                return Results.File(fileStream, contentType!);
            },
            fileDoesntExistResult => Results.NotFound(fileDoesntExistResult.FileName));
    }
}
