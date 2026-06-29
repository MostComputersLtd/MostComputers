using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;

public interface IGroupPromotionProductBindingsService
{
    Task<Dictionary<int, List<int>>> GetAllAsync();
    Task<List<int>> GetAllProductIdsBoundToPromotionAsync(int promotionId);
    Task<List<int>> GetAllPromotionIdsBoundToProductAsync(int productId);
    Task<Dictionary<int, List<int>>> GetAllPromotionIdsBoundToProductsAsync(IEnumerable<int> productIds);
    Task<OneOf<Success, NotFound>> UpsertAllAsync(int promotionId, List<int>? relatedProductIds);
}