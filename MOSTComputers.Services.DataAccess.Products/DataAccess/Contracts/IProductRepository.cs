using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
public interface IProductRepository
{
    Task<List<int>> GetAllIdsAsync();
    Task<List<int>> GetOnlyExistingIdsAsync(List<int> idsToValidate);
    Task<List<Product>> GetAllAsync();
    Task<List<Product>> GetAllWithStatusesAsync(List<ProductStatus> productStatuses);
    Task<List<Product>> GetAllInCategoriesAsync(IEnumerable<int> categoryIds);
    Task<List<Product>> GetAllInCategoryAsync(int categoryId);
    Task<List<Product>> GetByIdsAsync(IEnumerable<int> ids);
    Task<Product?> GetByIdAsync(int id);
    Task<Product?> GetProductWithHighestIdAsync();
}