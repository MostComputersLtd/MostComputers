using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Services.ProductRegister.Services.Contracts;

namespace MOSTComputers.UI.Web.Blazor.Endpoints.Documents;

public static class ProductDocumentEndpoints
{
    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "documents/" + "product/";

    public static IEndpointConventionBuilder MapProductDocumentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute);

        endpointGroup.MapGet("{documentId:int}", GetByIdAsync);

        return endpointGroup;
    }

    public static async Task<IResult> GetByIdAsync(
        [FromRoute] int documentId,
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