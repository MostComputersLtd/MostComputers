using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Groups;
using OneOf;
using OneOf.Types;

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
    Task<OneOf<int, UnexpectedFailureResult>> InsertAsync(GroupPromotionContentCreateRequest createRequest);
    Task<OneOf<Success, NotFound>> UpdateAsync(GroupPromotionContentUpdateRequest updateRequest);
}