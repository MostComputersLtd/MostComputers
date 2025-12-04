using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts;
public interface IProductCharacteristicService
{
    Task<List<ProductCharacteristic>> GetAllByCategoryIdAsync(int categoryId);
    Task<List<ProductCharacteristic>> GetAllByCategoryIdAsync(int categoryId, bool active);
    Task<List<ProductCharacteristic>> GetAllByCategoryIdsAsync(IEnumerable<int> categoryIds);
    Task<List<ProductCharacteristic>> GetAllByCategoryIdsAsync(IEnumerable<int> categoryIds, bool active);
    Task<List<ProductCharacteristic>> GetAllByCategoryIdAndTypeAsync(int categoryId, ProductCharacteristicType productCharacteristicType);
    Task<List<ProductCharacteristic>> GetAllByCategoryIdAndTypeAsync(int categoryId, ProductCharacteristicType productCharacteristicType, bool active);
    Task<List<ProductCharacteristic>> GetAllByCategoryIdAndTypesAsync(int categoryId, IEnumerable<ProductCharacteristicType> productCharacteristicTypes);
    Task<List<ProductCharacteristic>> GetAllByCategoryIdAndTypesAsync(int categoryId, IEnumerable<ProductCharacteristicType> productCharacteristicTypes, bool active);
    Task<List<ProductCharacteristic>> GetAllByCategoryIdsAndTypesAsync(IEnumerable<int> categoryIds, IEnumerable<ProductCharacteristicType> productCharacteristicTypes);
    Task<List<ProductCharacteristic>> GetAllByCategoryIdsAndTypesAsync(IEnumerable<int> categoryIds, IEnumerable<ProductCharacteristicType> productCharacteristicTypes, bool active);
    Task<List<ProductCharacteristic>> GetSelectionByCharacteristicIdsAsync(IEnumerable<int> ids);
    Task<List<ProductCharacteristic>> GetSelectionByCategoryIdAndNamesAsync(int categoryId, List<string> names);
    Task<ProductCharacteristic?> GetByIdAsync(int id);
    Task<ProductCharacteristic?> GetByCategoryIdAndNameAsync(int categoryId, string name);
    Task<ProductCharacteristic?> GetByCategoryIdAndNameAndCharacteristicTypeAsync(int categoryId, string name, ProductCharacteristicType productCharacteristicType);
}