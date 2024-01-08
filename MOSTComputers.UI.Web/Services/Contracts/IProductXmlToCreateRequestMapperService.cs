using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.XMLDataOperations.Models;
using MOSTComputers.UI.Web.Models;
using OneOf;

namespace MOSTComputers.UI.Web.Services.Contracts;
public interface IProductXmlToCreateRequestMapperService
{
    ProductCreateRequest GetProductCreateRequestFromProductXmlDisplay(XmlProductCreateDisplay display);
    Task<OneOf<List<ProductCreateRequest>, ValidationResult, UnexpectedFailureResult, InvalidXmlResult>> GetProductCreateRequestsFromXmlAsync(string xmlText);
    Task<OneOf<List<ProductCreateRequest>, ValidationResult>> GetProductCreateRequestsFromXmlAsync(XmlObjectData xmlObjectData, string xmlTextForImages);
    List<XmlProductCreateDisplay> GetProductXmlDisplayFromProductData(List<Tuple<XmlProduct, ProductCreateRequest>> xmlProductsAndRequests);
    XmlProductCreateDisplay GetProductXmlDisplayFromProductData(XmlProduct xmlProduct, ProductCreateRequest createRequest);
    Task<OneOf<List<XmlProductCreateDisplay>, ValidationResult, InvalidXmlResult, UnexpectedFailureResult>> GetProductXmlDisplayFromXmlAsync(string xmlText);
    Task<OneOf<List<XmlProductCreateDisplay>, ValidationResult>> GetProductXmlDisplayFromXmlAsync(XmlObjectData xmlObjectData, string xmlTextForImages);
    XmlObjectData GetXmlDataFromProducts(List<Product> products);
}