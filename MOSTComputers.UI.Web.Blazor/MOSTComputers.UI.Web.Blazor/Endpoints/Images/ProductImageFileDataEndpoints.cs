using Microsoft.AspNetCore.Mvc;
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

        endpointGroup.MapGet("/{fullFileName}", GetProductImageFile);

        return endpointGroup;
    }

    private static IResult GetProductImageFile(
        [FromRoute] string fullFileName,
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