using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;
using System.Text;
using static MOSTComputers.Services.ProductRegister.Validation.CommonValueConstraints;

namespace MOSTComputers.Services.ProductRegister.Services.Promotions.Groups;

public sealed class GroupPromotionReadService : IGroupPromotionReadService
{
    private const string _legacyImageRepresentationInHtmlContent = "PromViewImage.aspx?ImageId=";

    private readonly IGroupPromotionContentsRepository _groupPromotionContentsRepository;

    public GroupPromotionReadService(IGroupPromotionContentsRepository groupPromotionContentsRepository)
    {
        _groupPromotionContentsRepository = groupPromotionContentsRepository;
    }

    public DateTime GetMinStartDate()
    {
        return GroupPromotionContentConstraints.MinInsertStartDate;
    }

    public Task<List<GroupPromotionContent>> GetAllAsync()
    {
        return _groupPromotionContentsRepository.GetAllAsync();
    }

    public Task<List<GroupPromotionContent>> GetAllActiveAsync()
    {
        return _groupPromotionContentsRepository.GetAllActiveAsync();
    }

    public Task<List<GroupPromotionContent>> GetAllActiveInGroupAsync(int groupId)
    {
        return _groupPromotionContentsRepository.GetAllActiveInGroupAsync(groupId);
    }

    public Task<List<IGrouping<int, GroupPromotionContent>>> GetAllActiveInGroupsAsync(List<int> groupIds)
    {
        return _groupPromotionContentsRepository.GetAllActiveInGroupsAsync(groupIds);
    }

    public Task<List<GroupPromotionContent>> GetAllActiveAndNotExpiredDuringGivenDateTimeAsync(DateTime dateTime)
    {
        return _groupPromotionContentsRepository.GetAllActiveAndNotExpiredDuringGivenDateTimeAsync(dateTime);
    }

    public Task<List<GroupPromotionContent>> GetAllInGroupAsync(int groupId)
    {
        return _groupPromotionContentsRepository.GetAllInGroupAsync(groupId);
    }

    public Task<List<IGrouping<int, GroupPromotionContent>>> GetAllInGroupsAsync(List<int> groupIds)
    {
        return _groupPromotionContentsRepository.GetAllInGroupsAsync(groupIds);
    }

    public Task<List<GroupPromotionContent>> GetByIdsAsync(IEnumerable<int> groupIds)
    {
        return _groupPromotionContentsRepository.GetByIdsAsync(groupIds);
    }

    public Task<GroupPromotionContent?> GetByIdAsync(int id)
    {
        return _groupPromotionContentsRepository.GetByIdAsync(id);
    }

    public string? ChangeLegacyUrlsToNewOnes(
        string? htmlContent,
        IEnumerable<GroupPromotionImageFileData>? promotionImageFiles,
        Func<GroupPromotionImageFileData, string> getNewUrlFromFileData)
    {
        if (string.IsNullOrWhiteSpace(htmlContent)
            || promotionImageFiles == null)
        {
            return htmlContent;
        }

        int indexToScanFrom = 0;

        StringBuilder stringBuilder = new();

        while (true)
        {
            int indexOfLegacyHtmlImageRepresentation = htmlContent.IndexOf(_legacyImageRepresentationInHtmlContent, indexToScanFrom);

            if (indexOfLegacyHtmlImageRepresentation < 0)
            {
                stringBuilder.Append(htmlContent[indexToScanFrom..]);

                break;
            }

            string contentBeforeImageRepresentation = htmlContent[indexToScanFrom..indexOfLegacyHtmlImageRepresentation];

            stringBuilder.Append(contentBeforeImageRepresentation);

            int currentImageIdCharacterIndex = indexOfLegacyHtmlImageRepresentation + _legacyImageRepresentationInHtmlContent.Length;

            char nextDigitInId;

            string imageIdAsString = string.Empty;

            while (currentImageIdCharacterIndex < htmlContent.Length)
            {
                nextDigitInId = htmlContent[currentImageIdCharacterIndex];

                if (!char.IsDigit(nextDigitInId)) break;

                imageIdAsString += nextDigitInId;

                currentImageIdCharacterIndex++;
            }

            int imageId = int.Parse(imageIdAsString);

            foreach (GroupPromotionImageFileData promotionImageFile in promotionImageFiles)
            {
                if (promotionImageFile.ImageId != imageId) continue;

                string newFileName = getNewUrlFromFileData(promotionImageFile);

                stringBuilder.Append(newFileName);

                break;
            }

            indexToScanFrom = currentImageIdCharacterIndex;
        }

        return stringBuilder.ToString();
    }
}