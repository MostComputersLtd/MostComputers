using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts;

using static MOSTComputers.Services.ProductRegister.Utils.SearchByIdsUtils;

namespace MOSTComputers.Services.ProductRegister.Services;
internal sealed class ProductService : IProductService
{
    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    private readonly IProductRepository _productRepository;

    public async Task<List<Product>> GetAllAsync()
    {
        return await _productRepository.GetAllAsync();
    }

    public async Task<List<Product>> GetAllWithStatusesAsync(List<MOSTComputers.Models.Product.Models.ProductStatus> productStatuses)
    {
        return await _productRepository.GetAllWithStatusesAsync(productStatuses);
    }

    public async Task<List<Product>> GetAllInCategoryAsync(int categoryId)
    {
        return await _productRepository.GetAllInCategoryAsync(categoryId);
    }

    public async Task<List<Product>> GetAllInCategoriesAsync(List<int> categoryIds)
    {
        categoryIds = categoryIds
            .Distinct()
            .ToList();

        return await _productRepository.GetAllInCategoriesAsync(categoryIds);
    }

    public async Task<List<Product>> GetByIdsAsync(List<int> ids)
    {
        ids = RemoveValuesSmallerThanOne(ids);

        return await _productRepository.GetByIdsAsync(ids);
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        if (id <= 0) return null;

        return await _productRepository.GetByIdAsync(id);
    }

    public async Task<Product?> GetProductWithHighestIdAsync()
    {
        return await _productRepository.GetProductWithHighestIdAsync();
    }
}