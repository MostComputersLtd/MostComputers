using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.Models.Requests.Product;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models;
using MOSTComputers.UI.Web.Models;
using OneOf;

namespace MOSTComputers.UI.Web.Services.Contracts;
public interface IProductXmlToProductDisplayMappingService
{
    ProductCreateRequest GetProductCreateRequestFromProductXmlDisplay(XmlProductCreateDisplay display);
    List<XmlProductCreateDisplay> GetProductXmlDisplayFromProductData(List<Tuple<XmlProduct, ProductCreateRequest>> xmlProductsAndRequests);
    XmlProductCreateDisplay GetProductXmlDisplayFromProductData(XmlProduct xmlProduct, ProductCreateRequest createRequest);
    Task<OneOf<List<XmlProductCreateDisplay>, ValidationResult, InvalidXmlResult, UnexpectedFailureResult>> GetProductXmlDisplayFromXmlAsync(string xmlText);
    Task<OneOf<List<XmlProductCreateDisplay>, ValidationResult>> GetProductXmlDisplayFromXmlAsync(XmlObjectData xmlObjectData, string xmlTextForImages);
}