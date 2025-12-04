using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
}