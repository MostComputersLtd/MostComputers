using MOSTComputers.Models.Product.Models.Promotions.Files;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Files.PromotionProductFiles;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.Promotions.PromotionProductFileInfos;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Files.Contracts;
public interface IPromotionProductFileInfoRepository
{
    Task<List<IGrouping<int, PromotionProductFileInfo>>> GetAllForProductsAsync(IEnumerable<int> productIds);
    Task<List<PromotionProductFileInfoForProductCountData>> GetCountOfAllForProductsAsync(IEnumerable<int> productIds);
    Task<List<PromotionProductFileInfo>> GetAllForProductAsync(int productId);
    Task<int> GetCountOfAllForProductAsync(int productId);
    Task<bool> DoesExistForPromotionFileAsync(int promotionFileId);
    Task<PromotionProductFileInfo?> GetByIdAsync(int id);
    Task<OneOf<int, UnexpectedFailureResult>> InsertAsync(PromotionProductFileInfoCreateRequest createRequest);
    Task<OneOf<Success, NotFound>> UpdateAsync(PromotionProductFileInfoUpdateRequest updateRequest);
    Task<bool> DeleteAsync(int id);
}