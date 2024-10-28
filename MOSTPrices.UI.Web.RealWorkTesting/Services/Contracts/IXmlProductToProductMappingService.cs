using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models;
using OneOf;

namespace MOSTComputers.UI.Web.RealWorkTesting.Services.Contracts;
public interface IXmlProductToProductMappingService
{
    Task<OneOf<Product, ValidationResult, InvalidXmlResult>> GetProductFromXmlDataAsync(XmlProduct product);
}