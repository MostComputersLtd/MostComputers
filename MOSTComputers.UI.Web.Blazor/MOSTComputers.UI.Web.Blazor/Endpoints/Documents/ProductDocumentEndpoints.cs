using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using static MOSTComputers.UI.Web.Blazor.Utils.AuthenticationUtils;

namespace MOSTComputers.UI.Web.Blazor.Endpoints.Documents;

public static class ProductDocumentEndpoints
{
    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "documents/" + "product/";

    public static IEndpointConventionBuilder MapProductDocumentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute);

        endpointGroup.MapGet("{documentId:int}", GetByIdAsync)
            .RequireAuthorization(Policies.Cookie)
            .RequireAuthorization(policy => policy.RequireRole("Admin", "ProductEditor"))
            // .WithMetadata(new IncludeInOpenApiSpecMetadata())
            .WithTags("Product Document")
            .WithName("GetProductDocument")
            .WithSummary("Returns a product document")
            .WithDescription("Returns the document data for the specified product document.")
            .AddOpenApiOperationTransformer((operation, context, cancellationToken) =>
            {
                OpenApiResponses currentResponses = operation.Responses ?? new();

                operation.Responses = new()
                {
                    ["200"] = new OpenApiResponse()
                    {
                        Description = "The PDF for the specified document.",
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

    [ProducesResponseType(400, Description = "The specified product document could not be retrieved.")]
    public static async Task<IResult> GetByIdAsync(
        [FromRoute(Name = "documentId")]
        [Description("The ID of the product document.")]
        int documentId,
        HttpContext httpContext,
        [FromServices] IProductDocumentFileService productDocumentFileService)
    {
        Stream? fileStream = await productDocumentFileService.GetFileStreamByIdAsync(documentId);

        if (fileStream == null)
        {
            return Results.BadRequest();
        }

        await fileStream.CopyToAsync(httpContext.Response.Body);

        return Results.Empty;
    }
}
