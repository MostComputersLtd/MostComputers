namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups.Contracts;

public interface IGroupPromotionContentsToProductsRepository
{
    Task<List<int>> GetAllProductIdsBoundToPromotionAsync(int promotionId);
    Task<List<int>> GetAllPromotionIdsBoundToProductAsync(int productId);
    Task UpsertAllAsync(int promotionId, List<int>? relatedProductIds);
}