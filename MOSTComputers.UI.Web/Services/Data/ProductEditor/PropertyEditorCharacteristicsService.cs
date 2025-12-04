using Microsoft.AspNetCore.Mvc.Rendering;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductProperties.Contacts;
using MOSTComputers.UI.Web.Models.ProductEditor.ProductProperties;
using MOSTComputers.UI.Web.Services.Data.ProductEditor.Contracts;
using MOSTComputers.UI.Web.Utils;

namespace MOSTComputers.UI.Web.Services.Data.ProductEditor;
public class PropertyEditorCharacteristicsService : IPropertyEditorCharacteristicsService
{
    public PropertyEditorCharacteristicsService(IProductCharacteristicService productCharacteristicService,
        IProductPropertyService productPropertyService)
    {
        _productCharacteristicService = productCharacteristicService;
        _productPropertyService = productPropertyService;
    }

    private readonly IProductCharacteristicService _productCharacteristicService;
    private readonly IProductPropertyService _productPropertyService;

    public async Task<List<ProductPropertyEditorPropertyData>> GetProductPropertyEditorDataForProductAsync(
        int productId,
        List<int> relatedCategoryIds,
        List<ProductCharacteristicType> productCharacteristicTypes)
    {
        List<ProductPropertyEditorPropertyData> propertyEditorDataList = new();

        List<ProductCharacteristic> productCharacteristics
            = await _productCharacteristicService.GetAllByCategoryIdsAndTypesAsync(relatedCategoryIds, productCharacteristicTypes, true);

        List<ProductProperty> productProperties = await _productPropertyService.GetAllInProductAsync(productId);

        IEnumerable<int> propertyCharacteristicIds = productProperties
            .Where(x => x.ProductCharacteristicId is not null)
            .Select(x => (int)x.ProductCharacteristicId!);

        foreach (ProductProperty property in productProperties)
        {
            ProductCharacteristic? productCharacteristic = productCharacteristics.FirstOrDefault(x => x.Id == property.ProductCharacteristicId);

            IEnumerable<ProductCharacteristic> relatedCharacteristics = productCharacteristics;

            if (productCharacteristic is not null)
            {
                relatedCharacteristics = productCharacteristics
                    .Where(x => x.KWPrCh == productCharacteristic.KWPrCh);
            }

            IEnumerable<int> propertyCharacteristicIdsToExclude = propertyCharacteristicIds
                .Where(productPropertyId => productPropertyId != property.ProductCharacteristicId);

            List<ProductCharacteristic> remainingCharacteristics
                = GetDifferentCharacteristics(relatedCharacteristics, propertyCharacteristicIdsToExclude);

            ProductPropertyEditorPropertyData propertyEditorData = new()
            {
                ProductId = property.ProductId,
                ProductCharacteristic = productCharacteristic,
                Value = property.Value,
                DisplayOrder = property.DisplayOrder,
                XmlPlacement = property.XmlPlacement,
                RemainingCharacteristics = remainingCharacteristics
            };

            propertyEditorDataList.Add(propertyEditorData);
        }

        return propertyEditorDataList;
    }

    public async Task<ProductPropertyEditorPropertyData> GetPropertyEditorPropertyDataAsync(Product product, int? characteristicId, ProductCharacteristicType? productCharacteristicType)
    {
        List<int> relatedCategoryIds = [-1];

        if (product.CategoryId is not null)
        {
            relatedCategoryIds.Add(product.CategoryId.Value);
        }

        List<ProductCharacteristicType> productCharacteristicTypes = [];

        List<ProductCharacteristic> productCharacteristics;

        if (productCharacteristicType is not null)
        {
            productCharacteristicTypes.Add(productCharacteristicType.Value);

            productCharacteristics = await _productCharacteristicService.GetAllByCategoryIdsAndTypesAsync(relatedCategoryIds, productCharacteristicTypes, true);
        }
        else
        {
            productCharacteristics = await _productCharacteristicService.GetAllByCategoryIdsAsync(relatedCategoryIds, true);
        }

        List<ProductProperty> productProperties = await _productPropertyService.GetAllInProductAsync(product.Id);

        IEnumerable<int> propertyCharacteristicIds = productProperties
            .Where(x => x.ProductCharacteristicId is not null)
            .Select(x => (int)x.ProductCharacteristicId!);

        ProductProperty? existingProperty = productProperties.FirstOrDefault(x => x.ProductCharacteristicId == characteristicId);

        if (existingProperty is not null)
        {
            ProductCharacteristic? existingCharacteristic = productCharacteristics.FirstOrDefault(x => x.Id == characteristicId);

            return new()
            {
                ProductId = product.Id,
                ProductCharacteristic = existingCharacteristic,
                DisplayOrder = existingProperty.DisplayOrder,
                Value = existingProperty.Value,
                XmlPlacement = existingProperty.XmlPlacement,
                RemainingCharacteristics = await GetRemainingCharacteristicsForPropertyAsync(product.Id, characteristicId, relatedCategoryIds, productCharacteristicTypes),
            };
        }

        return new()
        {
            ProductId = product.Id,
            RemainingCharacteristics = await GetRemainingCharacteristicsForPropertyAsync(product.Id, null, relatedCategoryIds, productCharacteristicTypes),
        };
    }

    public async Task<List<ProductCharacteristic>> GetRemainingCharacteristicsForPropertyAsync(
        int productId,
        int? productCharacteristicId,
        List<int> relatedCategoryIds,
        List<ProductCharacteristicType> productCharacteristicTypes)
    {
        List<ProductCharacteristic> productCharacteristics
            = await _productCharacteristicService.GetAllByCategoryIdsAndTypesAsync(relatedCategoryIds, productCharacteristicTypes, true);

        List<ProductProperty> productProperties = await _productPropertyService.GetAllInProductAsync(productId);

        IEnumerable<int> propertyCharacteristicIds = productProperties
            .Where(x => x.ProductCharacteristicId is not null
                && x.ProductCharacteristicId != productCharacteristicId)
            .Select(x => (int)x.ProductCharacteristicId!);

        List<ProductCharacteristic> remainingCharacteristics
            = GetDifferentCharacteristics(productCharacteristics, propertyCharacteristicIds);

        return remainingCharacteristics;
    }

    public async Task<List<ProductCharacteristic>> GetDifferentCharacteristicsForCategoriesAsync(
        IEnumerable<int> categoryIds,
        IEnumerable<int>? characteristicIdsToExclude = null)
    {
        IEnumerable<ProductCharacteristic> characteristicsForProductCategory = await _productCharacteristicService.GetAllByCategoryIdsAndTypesAsync(
            categoryIds, [ProductCharacteristicType.ProductCharacteristic, ProductCharacteristicType.Link], true);

        return GetDifferentCharacteristics(characteristicsForProductCategory, characteristicIdsToExclude);
    }

    public List<ProductCharacteristic> GetDifferentCharacteristics(
        IEnumerable<ProductCharacteristic> characteristics,
        IEnumerable<int>? characteristicIdsToExclude = null)
    {
        List<ProductCharacteristic> differentCharacteristics = new();

        foreach (ProductCharacteristic characteristic in characteristics)
        {
            if (characteristic.Name is null) continue;

            ProductCharacteristic? matchingCharacteristic = differentCharacteristics.FirstOrDefault(x => x.Id == characteristic.Id);

            if (matchingCharacteristic is not null) continue;

            bool? shouldExcludeCharacteristic = characteristicIdsToExclude?.Contains(characteristic.Id);

            if (shouldExcludeCharacteristic == true) continue;

            differentCharacteristics.Add(characteristic);
        }

        return differentCharacteristics;
    }
}