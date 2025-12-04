using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups.Contracts;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.PromotionGroupData;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.New.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;
using MOSTComputers.UI.Web.Blazor.Endpoints.Images;

using static MOSTComputers.Utils.Files.FilePathUtils;

namespace MOSTComputers.UI.Web.Blazor.Endpoints.Xml;

internal static class PromotionGroupXmlDataEndpoints
{
    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "promotionGroup/" + "xml";

    public static IEndpointConventionBuilder MapPromotionGroupXmlEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute);

        endpointGroup.MapGet("/all", GetXmlForAllPromotionGroupsAsync);
        endpointGroup.MapGet("/id={promotionGroupId:int}", GetXmlForAllPromotionGroupAsync);

        return endpointGroup;
    }

    private static async Task<IResult> GetXmlForAllPromotionGroupsAsync(
        HttpContext httpContext,
        [FromServices] IPromotionGroupsRepository promotionGroupsRepository,
        [FromServices] IGroupPromotionContentsRepository groupPromotionContentsRepository,
        [FromServices] IGroupPromotionImageFileDataService groupPromotionImageFileDataService,
        [FromServices] IGroupPromotionXmlService groupPromotionXmlService)
    {
        HttpRequest request = httpContext.Request;

        string baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";

        List<PromotionGroup> promotionGroups = await promotionGroupsRepository.GetAllAsync();

        IEnumerable<PromotionGroup> promotionGroupsOrdered = promotionGroups.OrderBy(x => x.DisplayOrder ?? int.MaxValue);

        GroupPromotionsXmlFullData xmlData = await GetXmlGroupPromotionsXmlDataForGroupsAsync(
            groupPromotionContentsRepository,
            groupPromotionImageFileDataService,
            promotionGroupsOrdered,
            baseUrl);

        httpContext.Response.ContentType = "application/xml";
        httpContext.Response.Headers.TryAdd("Content-Disposition", "inline; filename=data.xml");

        await groupPromotionXmlService.TrySerializeXmlAsync(httpContext.Response.Body, xmlData);

        return Results.Empty;
    }

    private static async Task<IResult> GetXmlForAllPromotionGroupAsync(
        [FromRoute] int promotionGroupId,
        HttpContext httpContext,
        [FromServices] IPromotionGroupsRepository promotionGroupsRepository,
        [FromServices] IGroupPromotionContentsRepository groupPromotionContentsRepository,
        [FromServices] IGroupPromotionImageFileDataService groupPromotionImageFileDataService,
        [FromServices] IGroupPromotionXmlService groupPromotionXmlService)
    {
        HttpRequest request = httpContext.Request;

        string baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";

        PromotionGroup? promotionGroup = await promotionGroupsRepository.GetByIdAsync(promotionGroupId);

        List<PromotionGroup> promotionGroupsInXml = promotionGroup is not null ? [promotionGroup] : [];

        GroupPromotionsXmlFullData xmlData = await GetXmlGroupPromotionsXmlDataForGroupsAsync(
            groupPromotionContentsRepository,
            groupPromotionImageFileDataService,
            promotionGroupsInXml,
            baseUrl);

        httpContext.Response.ContentType = "application/xml";
        httpContext.Response.Headers.TryAdd("Content-Disposition", "inline; filename=data.xml");

        await groupPromotionXmlService.TrySerializeXmlAsync(httpContext.Response.Body, xmlData);

        return Results.Empty;
    }

    private static async Task<GroupPromotionsXmlFullData> GetXmlGroupPromotionsXmlDataForGroupsAsync(
        IGroupPromotionContentsRepository groupPromotionContentsRepository,
        IGroupPromotionImageFileDataService groupPromotionImageFileDataService,
        IEnumerable<PromotionGroup> promotionGroups,
        string baseUrl)
    {
        List<int> groupIds = promotionGroups.Select(x => x.Id).ToList();

        List<IGrouping<int, GroupPromotionContent>> promotionGroupContents
            = await groupPromotionContentsRepository.GetAllActiveInGroupsAsync(groupIds);

        List<IGrouping<int, GroupPromotionContent>> activePromotionGroupContentsGrouped = promotionGroupContents
            .SelectMany(x => x)
            .Where(x => x.GroupId is not null
                && (x.StartDate is null || x.StartDate <= DateTime.Now)
                && (x.ExpirationDate is null || x.ExpirationDate >= DateTime.Now))
            .GroupBy(x => x.GroupId!.Value)
            .ToList();

        List<int> promotionIds = activePromotionGroupContentsGrouped
            .SelectMany(x => x)
            .Select(x => x.Id)
            .ToList();

        List<IGrouping<int, GroupPromotionImageFileData>> promotionImageFiles = await groupPromotionImageFileDataService.GetAllInPromotionsAsync(promotionIds);

        GroupPromotionsXmlFullData xmlData = new()
        {
            PromotionGroups = new(),
        };

        foreach (PromotionGroup promotionGroup in promotionGroups)
        {
            XmlPromotionGroup xmlPromotionGroup = new()
            {
                Name = promotionGroup.Name,
                Promotions = new(),
            };

            List<GroupPromotionContent>? promotionsInGroup = activePromotionGroupContentsGrouped
                .FirstOrDefault(x => x.Key == promotionGroup.Id)?
                .OrderBy(x => x.DisplayOrder ?? int.MaxValue)
                .ToList();

            if (promotionsInGroup is null || promotionsInGroup.Count <= 0)
            {
                xmlData.PromotionGroups.Add(xmlPromotionGroup);

                continue;
            }

            foreach (GroupPromotionContent groupPromotionContent in promotionsInGroup)
            {
                IEnumerable<GroupPromotionImageFileData>? imageFilesForPromotion = promotionImageFiles.FirstOrDefault(x => x.Key == groupPromotionContent.Id);

                if (imageFilesForPromotion is null) continue;

                foreach (GroupPromotionImageFileData imageFileForPromotion in imageFilesForPromotion)
                {
                    XmlGroupPromotion xmlGroupPromotion = new()
                    {
                        PromotionPictureUrl = CombinePathsWithSeparator('/', baseUrl, GroupPromotionImageFileEndpoints.EndpointGroupRoute, $"{imageFileForPromotion.Id}"),
                    };

                    xmlPromotionGroup.Promotions.Add(xmlGroupPromotion);
                }
            }

            xmlData.PromotionGroups.Add(xmlPromotionGroup);
        }

        return xmlData;
    }
}