using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;
using MOSTComputers.UI.Web.Blazor.Endpoints;
using static MOSTComputers.UI.Web.Blazor.Components.Product.Promotion.Groups.PromotionGroupImagesViewer;

namespace MOSTComputers.UI.Web.Blazor.Components.Product.Promotion.Groups;

public static class PromotionGroupComponentEndpoints
{
    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "components/" + "promotion/" + "groups";

    public static IEndpointConventionBuilder MapPromotionGroupComponentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute);

        endpointGroup.MapGet("/{promotionGroupId}", GetPromotionImagesComponentForGroupAsync);

        return endpointGroup;
    }

    private static async Task<IResult> GetPromotionImagesComponentForGroupAsync(
        [FromServices] IGroupPromotionContentsRepository groupPromotionContentsRepository,
        [FromServices] IGroupPromotionImageFileDataService groupPromotionImageFileDataService,
        [FromRoute] int promotionGroupId,
        [FromQuery] int? alwaysPresentDefaultGroupId = null)
    {
        if (promotionGroupId <= 0) return Results.BadRequest();

        List<int> promotionGroupIds = [ promotionGroupId ];

        if (alwaysPresentDefaultGroupId != null)
        {
            if (alwaysPresentDefaultGroupId <= 0) return Results.BadRequest();

            promotionGroupIds.Add(alwaysPresentDefaultGroupId.Value);
        }

        List<GroupPromotionImageDisplayData> imageFiles = await GetPromotionGroupImagesAsync(
            groupPromotionContentsRepository,
            groupPromotionImageFileDataService,
            promotionGroupIds);

        return new RazorComponentResult<PromotionGroupImages>(new
        {
            ImageFiles = imageFiles,
        });
    }

    private static async Task<List<GroupPromotionImageDisplayData>> GetPromotionGroupImagesAsync(
        IGroupPromotionContentsRepository groupPromotionContentsRepository,
        IGroupPromotionImageFileDataService groupPromotionImageFileDataService,
        List<int> promotionGroupIds)
    {
        List<IGrouping<int, GroupPromotionContent>>? promotionGroupContents
            = await groupPromotionContentsRepository.GetAllActiveInGroupsAsync(promotionGroupIds);

        if (promotionGroupContents is null || promotionGroupContents.Count <= 0) return new();

        promotionGroupContents = promotionGroupContents
            .SelectMany(x => x)
            .Where(x => (x.StartDate is null || x.StartDate <= DateTime.Now)
                && (x.ExpirationDate is null || x.ExpirationDate >= DateTime.Now))
            .GroupBy(x => x.GroupId!.Value)
            .ToList();

        List<int> promotionIds = promotionGroupContents
            .SelectMany(x => x)
            .Select(x => x.Id)
            .ToList();

        List<IGrouping<int, GroupPromotionImageFileData>> promotionImageFiles
            = await groupPromotionImageFileDataService.GetAllInPromotionsAsync(promotionIds);

        if (promotionImageFiles is null || promotionImageFiles.Count <= 0) return new();

        List<GroupPromotionImageDisplayData> imagesForGroup = new();

        foreach (IGrouping<int, GroupPromotionContent> kvp in promotionGroupContents)
        {
            foreach (GroupPromotionContent groupPromotionContent in kvp)
            {
                IEnumerable<GroupPromotionImageFileData>? imageFilesForPromotion
                    = promotionImageFiles.FirstOrDefault(x => x.Key == groupPromotionContent.Id);

                if (imageFilesForPromotion is null) continue;

                foreach (GroupPromotionImageFileData imageFileForPromotion in imageFilesForPromotion)
                {
                    GroupPromotionImageDisplayData xmlGroupPromotion = new()
                    {
                        Id = imageFileForPromotion.Id,
                        FileName = imageFileForPromotion.FileName,
                    };

                    imagesForGroup.Add(xmlGroupPromotion);
                }
            }
        }

        return imagesForGroup;
    }
}
