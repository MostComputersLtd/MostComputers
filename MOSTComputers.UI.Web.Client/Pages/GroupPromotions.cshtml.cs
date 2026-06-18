using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;
using MOSTComputers.UI.Web.Client.Endpoints.Images;

using static MOSTComputers.Utils.Files.FilePathUtils;

namespace MOSTComputers.UI.Web.Client.Pages;

public class GroupPromotionsModel : PageModel
{
    public sealed class GroupPromotionWithImageFileData
    {
        public required GroupPromotionContent GroupPromotionContent { get; set; }
        public string? ModifiedPromotionHtml { get; set; }
    }

    private readonly IPromotionGroupService _promotionGroupService;
    private readonly IGroupPromotionReadService _groupPromotionReadService;
    private readonly IGroupPromotionImageFileDataService _groupPromotionImageFileDataService;

    public GroupPromotionsModel(
        IPromotionGroupService promotionGroupService,
        IGroupPromotionReadService groupPromotionReadService,
        IGroupPromotionImageFileDataService groupPromotionImageFileDataService)
    {
        _promotionGroupService = promotionGroupService;
        _groupPromotionReadService = groupPromotionReadService;
        _groupPromotionImageFileDataService = groupPromotionImageFileDataService;
    }

    public Dictionary<PromotionGroup, List<GroupPromotionWithImageFileData>> GroupPromotions { get; set; } = new();
    public List<GroupPromotionWithImageFileData> DefaultGroupPromotions { get; set; } = new();

    public int? DisplayedGroupId { get; set; }
    public int? FocusedPromotionId { get; set; }

    public async Task OnGetAsync([FromQuery] int? displayedGroupId = null, [FromQuery] int? focusedPromotionId = null)
    {
        List<PromotionGroup> promotionGroups = await _promotionGroupService.GetAllAsync();

        List<GroupPromotionContent> groupPromotionContents
            = await _groupPromotionReadService.GetAllActiveAndNotExpiredDuringGivenDateTimeAsync(DateTime.Now);

        List<int> promotionIds = groupPromotionContents.Select(x => x.Id)
            .ToList();

        List<IGrouping<int, GroupPromotionImageFileData>> promotionImageFiles
            = await _groupPromotionImageFileDataService.GetAllInPromotionsAsync(promotionIds);

        List<IGrouping<int?, GroupPromotionContent>> promotionContentsGrouped = groupPromotionContents
            .GroupBy(x => x.GroupId)
            .ToList();

        foreach (PromotionGroup promotionGroup in promotionGroups)
        {
            IEnumerable<GroupPromotionContent>? promotionGroupContent
                = promotionContentsGrouped.FirstOrDefault(x => x.Key == promotionGroup.Id)?
                .OrderBy(x => x.DisplayOrder ?? int.MaxValue);

            if (promotionGroupContent is null) continue;

            GroupPromotions.Add(promotionGroup, new());

            List<GroupPromotionWithImageFileData> groupPromotionDatas = GroupPromotions[promotionGroup];

            foreach (GroupPromotionContent groupPromotionContent in promotionGroupContent)
            {
                IEnumerable<GroupPromotionImageFileData>? imageFilesForPromotion = promotionImageFiles
                    .FirstOrDefault(x => x.Key == groupPromotionContent.Id);

                string? modifiedPromotionHtml = _groupPromotionReadService.ChangeLegacyUrlsToNewOnes(
                    groupPromotionContent.HtmlContent,
                    imageFilesForPromotion,
					promotionImageFile => CombinePathsWithSeparator('/', GroupPromotionImageFileEndpoints.EndpointGroupRoute, promotionImageFile.Id.ToString())
                );

                if (imageFilesForPromotion == null) continue;

                GroupPromotionWithImageFileData promotionData = new()
                {
                    GroupPromotionContent = groupPromotionContent,
                    ModifiedPromotionHtml = modifiedPromotionHtml,
                };

                groupPromotionDatas.Add(promotionData);
            }
        }

        IEnumerable<GroupPromotionContent> defaultGroupPromotions = groupPromotionContents
            .Where(x => x.MemberOfDefaultGroup == true)
            .OrderBy(x => x.DefaultGroupPriority);

        foreach (GroupPromotionContent defaultGroupPromotion in defaultGroupPromotions)
        {
            IEnumerable<GroupPromotionImageFileData>? imageFilesForPromotion = promotionImageFiles
                .FirstOrDefault(x => x.Key == defaultGroupPromotion.Id);

			string? modifiedPromotionHtml = _groupPromotionReadService.ChangeLegacyUrlsToNewOnes(
				defaultGroupPromotion.HtmlContent,
				imageFilesForPromotion,
				promotionImageFile => CombinePathsWithSeparator('/', GroupPromotionImageFileEndpoints.EndpointGroupRoute, promotionImageFile.Id.ToString())
			);

            GroupPromotionWithImageFileData promotionData = new()
			{
				GroupPromotionContent = defaultGroupPromotion,
				ModifiedPromotionHtml = modifiedPromotionHtml,
			};

            DefaultGroupPromotions.Add(promotionData);

            if (imageFilesForPromotion == null) continue;
        }

        DisplayedGroupId = displayedGroupId;
        FocusedPromotionId = focusedPromotionId;
    } 
}