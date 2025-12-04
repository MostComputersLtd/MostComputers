using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.Legacy;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.UI.Web.Services.ExternalXmlImport.Contracts;
public interface IProductXmlProvidingService
{
    Task<OneOf<string, NotFound>> GetProductXmlAsync(bool updateCachedProductXml = false);
    Task<OneOf<LegacyXmlObjectData, InvalidXmlResult, NotFound>> GetProductXmlParsedAsync(bool updateCachedProductXml = false);
}