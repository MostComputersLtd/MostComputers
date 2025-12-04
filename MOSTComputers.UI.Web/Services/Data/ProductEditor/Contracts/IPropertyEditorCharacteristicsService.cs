using MOSTComputers.Models.Product.Models;
using MOSTComputers.UI.Web.Models.ProductEditor.ProductProperties;

namespace MOSTComputers.UI.Web.Services.Data.ProductEditor.Contracts;
public interface IPropertyEditorCharacteristicsService
{
    List<ProductCharacteristic> GetDifferentCharacteristics(IEnumerable<ProductCharacteristic> characteristics, IEnumerable<int>? characteristicIdsToExclude = null);
    Task<List<ProductCharacteristic>> GetDifferentCharacteristicsForCategoriesAsync(IEnumerable<int> categoryIds, IEnumerable<int>? characteristicIdsToExclude = null);
    Task<List<ProductPropertyEditorPropertyData>> GetProductPropertyEditorDataForProductAsync(int productId, List<int> relatedCategoryIds, List<ProductCharacteristicType> productCharacteristicTypes);
    Task<ProductPropertyEditorPropertyData> GetPropertyEditorPropertyDataAsync(Product product, int? characteristicId, ProductCharacteristicType? productCharacteristicType);
    Task<List<ProductCharacteristic>> GetRemainingCharacteristicsForPropertyAsync(int productId, int? productCharacteristicId, List<int> relatedCategoryIds, List<ProductCharacteristicType> productCharacteristicTypes);
}