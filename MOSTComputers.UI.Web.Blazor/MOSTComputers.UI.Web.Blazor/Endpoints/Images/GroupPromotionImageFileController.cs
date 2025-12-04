using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Services.ProductRegister.Models.Responses;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;

using static MOSTComputers.Utils.Files.ContentTypeUtils;

namespace MOSTComputers.UI.Web.Blazor.Endpoints.Images;

internal static class GroupPromotionImageFileEndpoints
{
    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "promotionGroup/" + "imageFiles";

    public static IEndpointConventionBuilder MapGroupPromotionImageFileDataEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute);

        endpointGroup.MapGet("/{imageId}", GetGroupPromotionImageFileAsync);

        return endpointGroup;
    }

    private static async Task<IResult> GetGroupPromotionImageFileAsync(
        [FromRoute] int imageId,
        [FromServices] IGroupPromotionImageFileService groupPromotionFileManagementService)
    {
        GroupPromotionImageFile? promotionFile = await groupPromotionFileManagementService.GetByIdWithFileAsync(imageId);

        if (promotionFile is null || promotionFile.FileDataStream is null)
        {
            return Results.NotFound(imageId);
        }

        string? contentType = GetContentTypeFromExtension(promotionFile.FileName);

        if (string.IsNullOrWhiteSpace(contentType))
        {
            contentType = "application/octet-stream";
        }

        return Results.File(promotionFile.FileDataStream, contentType!);
    }
}