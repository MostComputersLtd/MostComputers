using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.Legacy;

namespace MOSTComputers.UI.Web.Services.Data.Xml.Contracts;
public interface ILegacyProductXmlFromProductDataService
{
    Task<LegacyXmlObjectData> GetLegacyXmlObjectDataForProductsAsync(IEnumerable<Product> products, string hostPath);
    Task<List<LegacyXmlProduct>> GetLegacyXmlProductsFromProductsAsync(IEnumerable<Product> products, string hostPath);
}