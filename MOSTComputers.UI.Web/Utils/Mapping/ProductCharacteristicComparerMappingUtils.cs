using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.Legacy;
using MOSTComputers.UI.Web.Models.ProductCharacteristicsComparer;

namespace MOSTComputers.UI.Web.Utils.Mapping;
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

    internal static ExternalXmlPropertyDisplayData MapToExternalXmlPropertyDisplayData(int categoryId, LegacyXmlProductProperty xmlProperty)
    {
        bool parseOrderSucceeded = int.TryParse(xmlProperty.Order, out int order);

        int? orderToUse = (parseOrderSucceeded) ? order : null;

        return new()
        {
            CategoryId = categoryId,
            Name = xmlProperty.Name,
            Order = orderToUse,
            Value = xmlProperty.Value,
        };
    }
}