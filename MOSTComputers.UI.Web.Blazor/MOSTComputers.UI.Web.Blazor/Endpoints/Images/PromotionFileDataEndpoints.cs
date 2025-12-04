using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Services.PromotionFileManagement.Services.Contracts;
using static MOSTComputers.Utils.Files.ContentTypeUtils;

namespace MOSTComputers.UI.Web.Blazor.Endpoints.Images;

public static class PromotionFileDataEndpoints
{
    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "images/" + "promotionFileData";

    public static IEndpointConventionBuilder MapPromotionFileDataEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute);

        endpointGroup.MapGet("/{fullFileName}", GetPromotionFile);

        return endpointGroup;
    }

    private static IResult GetPromotionFile(
        [FromRoute] string fullFileName,
        [FromServices] IPromotionFileManagementService promotionFileManagementService)
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

        Stream? fileStream = promotionFileManagementService.GetFileStream(fullFileName);

        if (fileStream is null)
        {
            return Results.NotFound($"The file was not found: {fullFileName}.");
        }

        return Results.File(fileStream, contentType!);
    }
}