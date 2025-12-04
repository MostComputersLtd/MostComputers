using MOSTComputers.Models.Product.Models.Promotions.Groups;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups.Contracts;
public interface IGroupPromotionContentsRepository
{
    Task<List<GroupPromotionContent>> GetAllActiveAsync();
    Task<List<GroupPromotionContent>> GetAllActiveInGroupAsync(int groupId);
    Task<List<IGrouping<int, GroupPromotionContent>>> GetAllActiveInGroupsAsync(List<int> groupIds);
    Task<List<GroupPromotionContent>> GetAllAsync();
    Task<List<GroupPromotionContent>> GetAllInGroupAsync(int groupId);
    Task<List<IGrouping<int, GroupPromotionContent>>> GetAllInGroupsAsync(List<int> groupIds);
    Task<GroupPromotionContent?> GetByIdAsync(int id);
}