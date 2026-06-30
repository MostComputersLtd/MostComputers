using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;
using MOSTComputers.UI.Web.Client.Endpoints.Images;

using static MOSTComputers.Utils.Files.FilePathUtils;

namespace MOSTComputers.UI.Web.Client.Endpoints.Html;

public static class GroupPromotionHtmlEndpoints
{
    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "groupPromotion/" + "html";

    public static IEndpointConventionBuilder MapGroupPromotionHtmlEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute);

        endpointGroup.MapGet("/{promotionId:int}", GetHtmlForGroupPromotionAsync);

        return endpointGroup;
    }

    private static async Task<IResult> GetHtmlForGroupPromotionAsync(
        HttpContext httpContext,
        [FromRoute] int promotionId,
        [FromServices] IGroupPromotionReadService groupPromotionReadService,
        [FromServices] IGroupPromotionImageFileDataService groupPromotionImageFileDataService)
    {
        HttpRequest request = httpContext.Request;

        GroupPromotionContent? groupPromotion = await groupPromotionReadService.GetByIdAsync(promotionId);

        if (groupPromotion == null || string.IsNullOrEmpty(groupPromotion.HtmlContent))
        {
            return Results.NotFound();
        }

        List<GroupPromotionImageFileData> groupPromotionImages = await groupPromotionImageFileDataService.GetAllInPromotionAsync(promotionId);

        string? modifiedPromotionHtml = groupPromotionReadService.ChangeLegacyUrlsToNewOnes(
            groupPromotion.HtmlContent,
            groupPromotionImages,
            promotionImageFile => CombinePathsWithSeparator('/', GroupPromotionImageFileEndpoints.EndpointGroupRoute, promotionImageFile.Id.ToString()));

        if (modifiedPromotionHtml == null)
        {
            return Results.NoContent();
        }

        httpContext.Response.ContentType = "application/html";
        httpContext.Response.Headers.TryAdd("Content-Disposition", "inline; filename=data.xml");

        return Results.Ok(modifiedPromotionHtml);
    }
}
