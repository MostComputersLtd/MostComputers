using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts;
public interface ICategoryService
{
    Task<List<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
}