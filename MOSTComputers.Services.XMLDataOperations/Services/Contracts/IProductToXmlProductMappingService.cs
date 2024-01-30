using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.XMLDataOperations.Models;

namespace MOSTComputers.Services.XMLDataOperations.Services.Contracts;
public interface IProductToXmlProductMappingService
{
    XmlProduct MapToXmlProduct(Product product);
}