using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Groups;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.Promotions.GroupPromotionImages;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups.Contracts;
public interface IGroupPromotionImageFileDatasRepository
{
    Task<List<GroupPromotionImageFileData>> GetAllAsync();
    Task<List<GroupPromotionImageFileData>> GetAllInPromotionAsync(int promotionId);
    Task<GroupPromotionImageFileData?> GetByIdAsync(int id);
    Task<GroupPromotionImageFileData?> GetByImageIdAsync(int imageId);
    Task<OneOf<int, ImageFileAlreadyExistsResult, UnexpectedFailureResult>> InsertAsync(GroupPromotionImageFileDataCreateRequest createRequest);
    Task<OneOf<Success, NotFound>> UpdateAsync(GroupPromotionImageFileDataUpdateRequest updateRequest);
    Task<bool> DeleteAsync(int id);
    Task<bool> DeleteByImageIdAsync(int imageId);
    Task<List<IGrouping<int, GroupPromotionImageFileData>>> GetAllInPromotionsAsync(List<int> promotionIds);
}