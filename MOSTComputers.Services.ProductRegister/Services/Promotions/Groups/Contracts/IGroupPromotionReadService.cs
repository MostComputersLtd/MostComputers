using MOSTComputers.Models.Product.Models.Promotions.Groups;

namespace MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;
public interface IGroupPromotionReadService
{
    string? ChangeLegacyUrlsToNewOnes(string? htmlContent, IEnumerable<GroupPromotionImageFileData>? promotionImageFiles, Func<GroupPromotionImageFileData, string> getNewUrlFromFileData);
    Task<List<GroupPromotionContent>> GetAllActiveAndNotExpiredDuringGivenDateTimeAsync(DateTime dateTime);
    Task<List<GroupPromotionContent>> GetAllActiveAsync();
    Task<List<GroupPromotionContent>> GetAllActiveInGroupAsync(int groupId);
    Task<List<IGrouping<int, GroupPromotionContent>>> GetAllActiveInGroupsAsync(List<int> groupIds);
    Task<List<GroupPromotionContent>> GetAllAsync();
    Task<List<GroupPromotionContent>> GetAllInGroupAsync(int groupId);
    Task<List<IGrouping<int, GroupPromotionContent>>> GetAllInGroupsAsync(List<int> groupIds);
    Task<GroupPromotionContent?> GetByIdAsync(int id);
    Task<List<GroupPromotionContent>> GetByIdsAsync(IEnumerable<int> groupIds);
    DateTime GetMinStartDate();
}