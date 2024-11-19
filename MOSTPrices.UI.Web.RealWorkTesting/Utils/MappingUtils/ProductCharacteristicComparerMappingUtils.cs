using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models;
using MOSTComputers.UI.Web.RealWorkTesting.Models.ProductCharacteristicsComparer;

namespace MOSTComputers.UI.Web.RealWorkTesting.Utils.MappingUtils;

internal static class ProductCharacteristicComparerMappingUtils
{
    internal static LocalCharacteristicDisplayData MapToLocalCharacteristicDisplayData(ProductCharacteristic productCharacteristic)
    {
        return new()
        {
            CategoryId = productCharacteristic.CategoryId,
            Id = productCharacteristic.Id,
            Name = productCharacteristic.Name,
            Meaning = productCharacteristic.Meaning,
            DisplayOrder = productCharacteristic.DisplayOrder,
            KWPrCh = productCharacteristic.KWPrCh,
        };
    }

    internal static List<LocalCharacteristicDisplayData> MapManyToLocalCharacteristicDisplayData(IEnumerable<ProductCharacteristic> productCharacteristics)
    {
        List<LocalCharacteristicDisplayData> output = new();

        foreach (ProductCharacteristic productCharacteristic in productCharacteristics)
        {
            LocalCharacteristicDisplayData localCharacteristicDisplayData = MapToLocalCharacteristicDisplayData(productCharacteristic);

            output.Add(localCharacteristicDisplayData);
        }

        return output;
    }

    internal static ExternalXmlPropertyDisplayData MapToExternalXmlPropertyDisplayData(int categoryId, XmlProductProperty xmlProperty)
    {
        int? order = string.IsNullOrEmpty(xmlProperty.Order) ? null : int.Parse(xmlProperty.Order);

        return new()
        {
            CategoryId = categoryId,
            Name = xmlProperty.Name,
            Order = order,
            Value = xmlProperty.Value,
        };
    }
}