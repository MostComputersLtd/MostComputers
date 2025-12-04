using OneOf;
using OneOf.Types;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Models.Product.Models.Promotions.Files;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Files.PromotionFiles;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Files.Contracts;
public interface IPromotionFileInfoRepository
{
    Task<List<PromotionFileInfo>> GetAllAsync();
    Task<List<PromotionFileInfo>> GetAllByActivityAsync(bool active = true);
    Task<List<PromotionFileInfo>> GetByIdsAsync(IEnumerable<int> ids);
    Task<PromotionFileInfo?> GetByIdAsync(int id);
    Task<OneOf<int, UnexpectedFailureResult>> InsertAsync(PromotionFileInfoCreateRequest createRequest);
    Task<OneOf<Success, UnexpectedFailureResult>> UpdateAsync(PromotionFileInfoUpdateRequest updateRequest);
    Task<bool> DeleteAsync(int id);
}