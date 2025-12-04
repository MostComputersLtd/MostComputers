using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts;
public interface IProductService
{
    Task<List<Product>> GetAllAsync();
    Task<List<Product>> GetAllInCategoriesAsync(List<int> categoryIds);
    Task<List<Product>> GetAllInCategoryAsync(int categoryId);
    Task<List<Product>> GetByIdsAsync(List<int> ids);
    Task<Product?> GetByIdAsync(int productId);
    Task<Product?> GetProductWithHighestIdAsync();
    Task<List<Product>> GetAllWithStatusesAsync(List<MOSTComputers.Models.Product.Models.ProductStatus> productStatuses);
}