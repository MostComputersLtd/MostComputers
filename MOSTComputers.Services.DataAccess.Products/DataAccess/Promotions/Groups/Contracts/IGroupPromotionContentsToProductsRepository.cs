
namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups.Contracts;

public interface IGroupPromotionContentsToProductsRepository
{
    Task<Dictionary<int, List<int>>> GetAllAsync();
    Task<List<int>> GetAllProductIdsBoundToPromotionAsync(int promotionId);
    Task<List<int>> GetAllPromotionIdsBoundToProductAsync(int productId);
    Task<Dictionary<int, List<int>>> GetAllPromotionIdsBoundToProductsAsync(IEnumerable<int> productIds);
    Task UpsertAllAsync(int promotionId, List<int>? relatedProductIds);
}