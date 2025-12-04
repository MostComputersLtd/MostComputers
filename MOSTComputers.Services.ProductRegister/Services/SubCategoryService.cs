using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts;

namespace MOSTComputers.Services.ProductRegister.Services;
internal sealed class SubCategoryService : ISubCategoryService
{
    public SubCategoryService(ISubCategoryRepository subCategoryRepository)
    {
        _subCategoryRepository = subCategoryRepository;
    }

    private readonly ISubCategoryRepository _subCategoryRepository;

    public async Task<List<SubCategory>> GetAllAsync()
    {
        return await _subCategoryRepository.GetAllAsync();
    }

    public async Task<List<SubCategory>> GetAllInCategoryAsync(int categoryId)
    {
        return await _subCategoryRepository.GetAllInCategoryAsync(categoryId);
    }

    public async Task<List<SubCategory>> GetAllByActivityAsync(bool active)
    {
        return await _subCategoryRepository.GetAllByActivityAsync(active);
    }

    public async Task<List<SubCategory>> GetInCategoryByActivityAsync(int categoryId, bool active)
    {
        return await _subCategoryRepository.GetInCategoryByActivityAsync(categoryId, active);
    }

    public async Task<List<SubCategory>> GetByIdsAsync(List<int> ids)
    {
        return await _subCategoryRepository.GetByIdsAsync(ids);
    }

    public async Task<SubCategory?> GetByIdAsync(int id)
    {
        return await _subCategoryRepository.GetByIdAsync(id);
    }

    public async Task<SubCategory?> GetByNameAsync(string name)
    {
        return await _subCategoryRepository.GetByNameAsync(name);
    }
}