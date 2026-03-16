using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;

namespace MOSTComputers.UI.Web.Client.Services;

public class ActivePromotionGroupsService
{
    public sealed class PromotionGroupImagesResult
    {
        public required GroupPromotionContent GroupPromotionContent { get; set; }
        public required List<GroupPromotionImageFileData> GroupPromotionImages { get; set; }
    }

    private readonly IPromotionGroupsRepository _promotionGroupsRepository;
    private readonly IGroupPromotionContentsRepository _groupPromotionContentsRepository;
    private readonly IGroupPromotionImageFileDataService _groupPromotionImageFileDataService;

    public ActivePromotionGroupsService(
        IPromotionGroupsRepository promotionGroupsRepository,
        IGroupPromotionContentsRepository groupPromotionContentsRepository,
        IGroupPromotionImageFileDataService groupPromotionImageFileDataService)
    {
        _promotionGroupsRepository = promotionGroupsRepository;
        _groupPromotionContentsRepository = groupPromotionContentsRepository;
        _groupPromotionImageFileDataService = groupPromotionImageFileDataService;
    }

    public async Task<Dictionary<PromotionGroup, List<PromotionGroupImagesResult>>> GetAllActivePromotionImagesAsync()
    {
        Dictionary<PromotionGroup, List<PromotionGroupImagesResult>> output = new();

        List<PromotionGroup> promotionGroups = await _promotionGroupsRepository.GetAllAsync();

        promotionGroups = promotionGroups.OrderBy(x => x.DisplayOrder ?? int.MaxValue)
            .ToList();

        List<int> promotionGroupIds = promotionGroups.Select(x => x.Id).ToList();

        List<IGrouping<int, GroupPromotionContent>> promotionGroupContents
            = await _groupPromotionContentsRepository.GetAllActiveInGroupsAsync(promotionGroupIds);

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
            = await _groupPromotionImageFileDataService.GetAllInPromotionsAsync(promotionIds);

        foreach (PromotionGroup promotionGroup in promotionGroups)
        {
            IGrouping<int, GroupPromotionContent>? promotionGroupContent
                = promotionGroupContents.FirstOrDefault(x => x.Key == promotionGroup.Id);

            if (promotionGroupContent is null) continue;

            output.Add(promotionGroup, new());

            List<PromotionGroupImagesResult> groupPromotionResults = output[promotionGroup];

            foreach (GroupPromotionContent groupPromotionContent in promotionGroupContent)
            {
                IEnumerable<GroupPromotionImageFileData>? imageFilesForPromotion = promotionImageFiles
                    .FirstOrDefault(x => x.Key == groupPromotionContent.Id);

                if (imageFilesForPromotion is null) continue;

                PromotionGroupImagesResult promotionGroupImagesResult = new()
                {
                    GroupPromotionContent = groupPromotionContent,
                    GroupPromotionImages = imageFilesForPromotion.ToList(),
                };

                groupPromotionResults.Add(promotionGroupImagesResult);
            }
        }

        return output;
    }
}