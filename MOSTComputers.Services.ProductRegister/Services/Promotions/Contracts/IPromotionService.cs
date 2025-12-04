using MOSTComputers.Models.Product.Models.Promotions;

namespace MOSTComputers.Services.ProductRegister.Services.Promotions.Contracts;
public interface IPromotionService
{
    Task<List<Promotion>> GetAllAsync();
    Task<List<Promotion>> GetAllActiveAsync();
    Task<List<IGrouping<int?, Promotion>>> GetAllForSelectionOfProductsAsync(IEnumerable<int> productIds);
    Task<List<IGrouping<int?, Promotion>>> GetAllActiveForSelectionOfProductsAsync(IEnumerable<int> productIds);
    Task<List<Promotion>> GetAllForProductAsync(int productId);
    Task<List<Promotion>> GetAllActiveForProductAsync(int productId);
}