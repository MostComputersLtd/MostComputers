using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.UI.Web.Client.Services;
using System.Collections.Generic;
using static MOSTComputers.UI.Web.Client.Services.ActivePromotionGroupsService;

namespace MOSTComputers.UI.Web.Client.Pages;

public class GroupPromotionsModel : PageModel
{
    private readonly ActivePromotionGroupsService _activePromotionGroupsService;

#pragma warning disable 8618 // Disabled nullability warnings as all affected variables are always not-null when we return
    public GroupPromotionsModel(ActivePromotionGroupsService activePromotionGroupsService)
#pragma warning restore 8618
    {
        _activePromotionGroupsService = activePromotionGroupsService;
    }

    public Dictionary<PromotionGroup, List<GroupPromotionImageFileData>> GroupPromotions { get; set; }
    public int? DisplayedGroupId { get; set; }
    public int? FocusedImageId { get; set; }

    public async Task OnGetAsync([FromQuery] int? displayedGroupId = null, [FromQuery] int? focusedImageId = null)
    {
        Dictionary<PromotionGroup, List<PromotionGroupImagesResult>> groupPromotionsSearched
            = await _activePromotionGroupsService.GetAllActivePromotionImagesAsync();

        IOrderedEnumerable<KeyValuePair<PromotionGroup, List<PromotionGroupImagesResult>>> groupPromotionsOrdered
            = groupPromotionsSearched.OrderBy(x => x.Key.DisplayOrder ?? int.MaxValue);

        Dictionary<PromotionGroup, List<GroupPromotionImageFileData>> groupPromotions = new();

        foreach (KeyValuePair<PromotionGroup, List<PromotionGroupImagesResult>> kvp in groupPromotionsOrdered)
        {
            groupPromotions.Add(kvp.Key, new());

            List<GroupPromotionImageFileData> groupPromotionImageFileDatas = groupPromotions[kvp.Key];

            IOrderedEnumerable<PromotionGroupImagesResult> orderedResults
                = kvp.Value.OrderBy(x => x.GroupPromotionContent.DisplayOrder ?? int.MaxValue);

            foreach (PromotionGroupImagesResult imageResult in orderedResults)
            {
                groupPromotionImageFileDatas.AddRange(imageResult.GroupPromotionImages);
            }
        }

        GroupPromotions = groupPromotions;

        DisplayedGroupId = displayedGroupId;
        FocusedImageId = focusedImageId;
    }
}