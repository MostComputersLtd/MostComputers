using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.Legacy;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.Legacy.Contracts;
public interface ILegacyProductXmlValidationService
{
    bool IsValidXmlObjectData(LegacyXmlObjectData xmlObjectData);
    bool IsValidXmlProduct(LegacyXmlProduct xmlProduct);
    bool IsValidXmlCategory(LegacyXmlCategory xmlCategory);
    bool IsValidXmlSubCategory(LegacyXmlSubCategory xmlSubCategory);
    bool IsValidXmlManufacturer(LegacyXmlManufacturer xmlManufacturer);
    bool IsValidXmlSearchStringPart(LegacySearchStringPartInfo xmlSearchStringPart);
    bool IsValidXmlProductImage(LegacyXmlProductImage xmlProductImage);
    bool IsValidXmlProductProperty(LegacyXmlProductProperty xmlProductProperty);
    bool IsValidXmlPromotion(LegacyXmlPromotion xmlPromotion);
    bool IsValidXmlFile(LegacyXmlFile xmlFile);
    bool IsValidXmlExchangeRates(LegacyXmlExchangeRatesEurAndUsdPerBgn xmlExchangeRates);
}