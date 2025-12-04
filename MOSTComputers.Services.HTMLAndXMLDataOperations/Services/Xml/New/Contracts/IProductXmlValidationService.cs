using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.ProductData;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.New.Contracts;
public interface IProductXmlValidationService
{
    bool IsValidXmlObjectData(ProductsXmlFullData xmlObjectData);
    bool IsValidXmlProduct(XmlProduct xmlProduct);
    bool IsValidXmlCategory(XmlCategory xmlCategory);
    bool IsValidXmlSubCategory(XmlSubCategory xmlSubCategory);
    bool IsValidXmlManufacturer(XmlManufacturer xmlManufacturer);
    bool IsValidXmlSearchStringPart(XmlSearchStringPartInfo xmlSearchStringPart);
    bool IsValidXmlProductImage(XmlProductImage xmlProductImage);
    bool IsValidXmlProductProperty(XmlProductProperty xmlProductProperty);
    bool IsValidXmlPromotion(XmlPromotion xmlPromotion);
    bool IsValidXmlFile(XmlFile xmlFile);
    bool IsValidXmlExchangeRates(XmlExchangeRates xmlExchangeRates);
}