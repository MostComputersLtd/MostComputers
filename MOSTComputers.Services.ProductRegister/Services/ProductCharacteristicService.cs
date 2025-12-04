using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts;

namespace MOSTComputers.Services.ProductRegister.Services;
internal sealed class ProductCharacteristicService : IProductCharacteristicService
{
    public ProductCharacteristicService(IProductCharacteristicsRepository productCharacteristicsRepository)
    {
        _productCharacteristicsRepository = productCharacteristicsRepository;
    }

    private readonly IProductCharacteristicsRepository _productCharacteristicsRepository;

    public async Task<List<ProductCharacteristic>> GetAllByCategoryIdAsync(int categoryId)
    {
        return await _productCharacteristicsRepository.GetAllByCategoryIdAsync(categoryId);
    }

    public async Task<List<ProductCharacteristic>> GetAllByCategoryIdAsync(int categoryId, bool active)
    {
        return await _productCharacteristicsRepository.GetAllByCategoryIdAsync(categoryId, active);
    }

    public async Task<List<ProductCharacteristic>> GetAllByCategoryIdsAsync(IEnumerable<int> categoryIds)
    {
        return await _productCharacteristicsRepository.GetAllByCategoryIdsAsync(categoryIds);
    }

    public async Task<List<ProductCharacteristic>> GetAllByCategoryIdsAsync(IEnumerable<int> categoryIds, bool active)
    {
        return await _productCharacteristicsRepository.GetAllByCategoryIdsAsync(categoryIds, active);
    }

    public async Task<List<ProductCharacteristic>> GetAllByCategoryIdAndTypeAsync(int categoryId, ProductCharacteristicType productCharacteristicType)
    {
        return await _productCharacteristicsRepository.GetAllByCategoryIdAndTypeAsync(categoryId, productCharacteristicType);
    }

    public async Task<List<ProductCharacteristic>> GetAllByCategoryIdAndTypeAsync(int categoryId, ProductCharacteristicType productCharacteristicType, bool active)
    {
        return await _productCharacteristicsRepository.GetAllByCategoryIdAndTypeAsync(categoryId, productCharacteristicType, active);
    }

    public async Task<List<ProductCharacteristic>> GetAllByCategoryIdAndTypesAsync(int categoryId, IEnumerable<ProductCharacteristicType> productCharacteristicTypes)
    {
        return await _productCharacteristicsRepository.GetAllByCategoryIdAndTypesAsync(categoryId, productCharacteristicTypes);
    }

    public async Task<List<ProductCharacteristic>> GetAllByCategoryIdAndTypesAsync(int categoryId, IEnumerable<ProductCharacteristicType> productCharacteristicTypes, bool active)
    {
        return await _productCharacteristicsRepository.GetAllByCategoryIdAndTypesAsync(categoryId, productCharacteristicTypes, active);
    }

    public async Task<List<ProductCharacteristic>> GetAllByCategoryIdsAndTypesAsync(IEnumerable<int> categoryIds, IEnumerable<ProductCharacteristicType> productCharacteristicTypes)
    {
        return await _productCharacteristicsRepository.GetAllByCategoryIdsAndTypesAsync(categoryIds, productCharacteristicTypes);
    }

    public async Task<List<ProductCharacteristic>> GetAllByCategoryIdsAndTypesAsync(IEnumerable<int> categoryIds, IEnumerable<ProductCharacteristicType> productCharacteristicTypes, bool active)
    {
        return await _productCharacteristicsRepository.GetAllByCategoryIdsAndTypesAsync(categoryIds, productCharacteristicTypes, active);
    }

    public async Task<List<ProductCharacteristic>> GetSelectionByCategoryIdAndNamesAsync(int categoryId, List<string> names)
    {
        return await _productCharacteristicsRepository.GetSelectionByCategoryIdAndNamesAsync(categoryId, names);
    }

    public async Task<List<ProductCharacteristic>> GetSelectionByCharacteristicIdsAsync(IEnumerable<int> ids)
    {
        if (!ids.Any()) return new();

        List<int> uniqueIds = new();

        foreach (int id in ids)
        {
            if (id <= 0 || uniqueIds.Contains(id)) continue;

            uniqueIds.Add(id);
        }

        return await _productCharacteristicsRepository.GetSelectionByCharacteristicIdsAsync(uniqueIds);
    }

    public async Task<ProductCharacteristic?> GetByIdAsync(int id)
    {
        if (id <= 0) return null;

        return await _productCharacteristicsRepository.GetByIdAsync(id);
    }

    public async Task<ProductCharacteristic?> GetByCategoryIdAndNameAsync(int categoryId, string name)
    {
        return await _productCharacteristicsRepository.GetByCategoryIdAndNameAsync(categoryId, name);
    }

    public async Task<ProductCharacteristic?> GetByCategoryIdAndNameAndCharacteristicTypeAsync(
        int categoryId,
        string name,
        ProductCharacteristicType productCharacteristicType)
    {
        return await _productCharacteristicsRepository.GetByCategoryIdAndNameAndCharacteristicTypeAsync(categoryId, name, productCharacteristicType);
    }
}