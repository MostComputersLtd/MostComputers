using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;

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
        HttpContext httpContext,
        [FromRoute] int imageId,
        [FromServices] IGroupPromotionImageFileService groupPromotionImageFileService)
    {
        GroupPromotionImageFileData? promotionFile = await groupPromotionImageFileService.GetByIdAsync(imageId); 

        if (promotionFile is null)
        {
            return Results.NotFound(imageId);
        }

        EntityTagHeaderValue eTag = new($"\"promotion-group-image-file-version-{promotionFile.Version}\"", false);

        httpContext.Response.Headers.ETag = eTag.ToString();
        httpContext.Response.Headers.CacheControl = "no-cache";

        if (httpContext.Request.Headers.IfNoneMatch.Count > 0)
        {
            foreach (StringValues headerValue in httpContext.Request.Headers.IfNoneMatch)
            {
                IList<EntityTagHeaderValue> requestEtags = EntityTagHeaderValue.ParseList(headerValue);

                foreach (EntityTagHeaderValue requestEtag in requestEtags)
                {
                    if (requestEtag.Compare(eTag, useStrongComparison: false))
                    {
                        return Results.StatusCode(StatusCodes.Status304NotModified);
                    }
                }
            }
        }

        Stream? fileStream = groupPromotionImageFileService.GetFileStream(promotionFile);

        if (fileStream == null) return Results.Problem(statusCode: StatusCodes.Status500InternalServerError);

        return Results.File(fileStream, promotionFile.ContentType);
    }
}
