using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts;
public interface ISubCategoryService
{
    Task<List<SubCategory>> GetAllAsync();
    Task<List<SubCategory>> GetAllByActivityAsync(bool active);
    Task<List<SubCategory>> GetAllInCategoryAsync(int categoryId);
    Task<List<SubCategory>> GetInCategoryByActivityAsync(int categoryId, bool active);
    Task<SubCategory?> GetByIdAsync(int id);
    Task<SubCategory?> GetByNameAsync(string name);
    Task<List<SubCategory>> GetByIdsAsync(List<int> ids);
}