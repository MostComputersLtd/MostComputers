using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models;
using OneOf;

namespace MOSTComputers.UI.Web.RealWorkTesting.Services.Contracts;
public interface IProductXmlToProductMappingService
{
    Task<OneOf<Product, ValidationResult>> GetProductFromXmlDataAsync(XmlProduct product, string xml);
}