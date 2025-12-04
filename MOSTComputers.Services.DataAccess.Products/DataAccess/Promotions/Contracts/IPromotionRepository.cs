using MOSTComputers.Models.Product.Models.Promotions;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Contracts;
public interface IPromotionRepository
{
    Task<List<Promotion>> GetAllAsync();
    Task<List<Promotion>> GetAllActiveAsync();
    Task<List<IGrouping<int?, Promotion>>> GetAllForSelectionOfProductsAsync(IEnumerable<int> productIds);
    Task<List<IGrouping<int?, Promotion>>> GetAllActiveForSelectionOfProductsAsync(IEnumerable<int> productIds);
    Task<List<Promotion>> GetAllForProductAsync(int productId);
    Task<List<Promotion>> GetAllActiveForProductAsync(int productId);
}