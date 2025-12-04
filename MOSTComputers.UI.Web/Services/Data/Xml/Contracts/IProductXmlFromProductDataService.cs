using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.ProductData;

namespace MOSTComputers.UI.Web.Services.Data.Xml.Contracts;
public interface IProductXmlFromProductDataService
{
    Task<ProductsXmlFullData> GetXmlObjectDataForProductsAsync(IEnumerable<Product> products, string hostPath);
    Task<List<XmlProduct>> GetXmlProductsFromProductsAsync(IEnumerable<Product> products, string hostPath);
}