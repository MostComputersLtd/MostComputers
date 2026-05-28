namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups.Contracts;

public interface IGroupPromotionContentsToProductsRepository
{
    Task<List<int>> GetAllProductIdsBoundToPromotionAsync(int promotionId);
}