using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.Promotions.GroupPromotionImages;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups.Contracts;
public interface IGroupPromotionImagesRepository
{
    Task<List<GroupPromotionImage>> GetAllInPromotionAsync(int groupPromotionId);
    Task<List<GroupPromotionImageWithoutFile>> GetAllInPromotionWithoutFilesAsync(int groupPromotionId);
    Task<List<GroupPromotionImageWithoutFile>> GetAllWithoutFilesAsync();
    Task<GroupPromotionImage?> GetByIdAsync(int id);
    Task<List<GroupPromotionImage>> GetByIdsAsync(IEnumerable<int> ids);
    Task<List<GroupPromotionImageWithoutFile>> GetByIdsWithoutFilesAsync(IEnumerable<int> ids);
    Task<GroupPromotionImageWithoutFile?> GetByIdWithoutFileAsync(int id);
}