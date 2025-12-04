using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
public interface ISubCategoryRepository
{
    Task<List<SubCategory>> GetAllAsync();
    Task<List<SubCategory>> GetAllByActivityAsync(bool active);
    Task<List<SubCategory>> GetAllInCategoryAsync(int categoryId);
    Task<List<SubCategory>> GetInCategoryByActivityAsync(int categoryId, bool active);
    Task<List<SubCategory>> GetByIdsAsync(List<int> ids);
    Task<SubCategory?> GetByIdAsync(int id);
    Task<SubCategory?> GetByNameAsync(string name);
}