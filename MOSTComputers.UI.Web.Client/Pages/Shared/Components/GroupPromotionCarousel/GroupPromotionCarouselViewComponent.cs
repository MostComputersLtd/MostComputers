using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.UI.Web.Client.Services;
using static MOSTComputers.UI.Web.Client.Services.ActivePromotionGroupsService;

namespace MOSTComputers.UI.Web.Client.Pages.Shared.Components.GroupPromotionCarousel;

[ViewComponent(Name = ComponentPath)]
public class GroupPromotionCarouselViewComponent : ViewComponent
{
    internal const string ComponentPath = "GroupPromotionCarousel";
    internal const string CarouselItemPath = $"Components/{ComponentPath}/GroupPromotionsCarouselItem";
    internal const string CarouselIndicatorPath = $"Components/{ComponentPath}/GroupPromotionCarouselIndicator";

    private readonly ActivePromotionGroupsService _activePromotionGroupsService;

    public GroupPromotionCarouselViewComponent(
        ActivePromotionGroupsService activePromotionGroupsService)
    {
        _activePromotionGroupsService = activePromotionGroupsService;
    }

    public async Task<IViewComponentResult> InvokeAsync(string carouselId, int viewportItemsCount)
    {
        Dictionary<PromotionGroup, List<PromotionGroupImagesResult>> groupPromotionsSearched
            = await _activePromotionGroupsService.GetAllActivePromotionImagesAsync();

        IOrderedEnumerable<PromotionGroupImagesResult> groupPromotionsOrdered
            = groupPromotionsSearched
            .SelectMany(kvp => kvp.Value)
            .OrderBy(x => x.GroupPromotionContent.DisplayOrder ?? int.MaxValue);

        int index = 0;

        List<GroupPromotionsCarouselItemModel> popupItems = new();

        foreach (PromotionGroupImagesResult promotionGroupImagesResult in groupPromotionsOrdered)
        {
            int groupId = promotionGroupImagesResult.GroupPromotionContent.GroupId ?? -1;

            foreach (GroupPromotionImageFileData image in promotionGroupImagesResult.GroupPromotionImages)
            {
                popupItems.Add(new()
                {
                    ImageUrls = [$"api/promotionGroup/imageFiles/{image.Id}"],
                    CarouselId = carouselId,
                    Index = index,
                    PromotionImageFileId = image.Id,
                    PromotionGroupId = groupId,
                    IsActive = index < viewportItemsCount,
                });

                index++;
            }
        }

        DefaultModel model = new()
        {
            CarouselId = carouselId,
            Items = popupItems.Cast<object>().ToList(),
            ItemTemplatePartialPath = CarouselItemPath,
            ItemIndicatorTemplatePartialPath = CarouselIndicatorPath,
            DisplayedItemsCount = viewportItemsCount,
            ItemHopsPerMove = 1,
            TransitionMs = 1200,
            GoToTransitionMs = 400,
            DisplayArrowButtons = false,
            AllowAutoSlide = true,
            AutoSlideIntervalMs = 5000,
        };

        return View("Default", model);
    }
}