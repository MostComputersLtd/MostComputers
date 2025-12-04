using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.Promotions;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.ProductData;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.New.Contracts;
public interface IProductToXmlProductMappingService
{
    XmlProduct MapProductDataToXmlProduct(
        Product product,
        List<ProductProperty>? productProperties = null,
        List<XmlProductImage>? xmlProductImages = null,
        IEnumerable<Promotion>? productPromotions = null,
        SubCategory? productSubCategory = null,
        List<XmlSearchStringPartInfo>? searchStringPartInfos = null,
        string? promotionPictureUrl = null);

    XmlProduct MapProductDataToXmlProduct(
        Product product,
        List<ProductProperty>? productProperties = null,
        List<XmlProductImage>? xmlProductImages = null,
        List<XmlPromotion>? xmlProductPromotions = null,
        XmlGroupPromotion? xmlGroupPromotion = null,
        SubCategory? productSubCategory = null,
        List<XmlSearchStringPartInfo>? searchStringPartInfos = null);
    string GetCurrencyCodeFromCurrency(Currency? currency);
}