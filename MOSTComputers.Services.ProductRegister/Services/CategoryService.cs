using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts;

namespace MOSTComputers.Services.ProductRegister.Services;
internal sealed class CategoryService : ICategoryService
{
    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    private readonly ICategoryRepository _categoryRepository;

    public async Task<List<Category>> GetAllAsync()
    {
        return await _categoryRepository.GetAllAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _categoryRepository.GetByIdAsync(id);
    }
}