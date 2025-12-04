using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.Promotions;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.Legacy;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.Legacy.Contracts;
public interface ILegacyProductToXmlProductMappingService
{
    LegacyXmlProduct MapProductDataToXmlProduct(
       Product product,
       List<ProductProperty>? productProperties = null,
       List<LegacyXmlProductImage>? xmlProductImages = null,
       IEnumerable<Promotion>? productPromotions = null,
       SubCategory? productSubCategory = null,
       List<LegacySearchStringPartInfo>? searchStringPartInfos = null);
}