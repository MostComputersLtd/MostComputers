using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
public interface IProductToXmlProductMappingService
{
    XmlProduct MapToXmlProduct(Product product);
}