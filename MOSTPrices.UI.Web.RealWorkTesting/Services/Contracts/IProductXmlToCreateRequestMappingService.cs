using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.Models.Requests.Product;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models;
using OneOf;

namespace MOSTComputers.UI.Web.RealWorkTesting.Services.Contracts;
public interface IProductXmlToCreateRequestMappingService
{
    Task<OneOf<List<ProductCreateRequest>, ValidationResult, UnexpectedFailureResult, InvalidXmlResult>> GetProductCreateRequestsFromXmlAsync(string xmlText);
    Task<OneOf<List<ProductCreateRequest>, ValidationResult>> GetProductCreateRequestsFromXmlAsync(XmlObjectData xmlObjectData, string xmlTextForImages);
    XmlObjectData GetXmlDataFromProducts(List<Product> products);
}