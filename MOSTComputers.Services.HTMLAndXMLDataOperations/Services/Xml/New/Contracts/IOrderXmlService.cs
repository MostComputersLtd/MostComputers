using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.Documents.OrderData;
using OneOf;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.New.Contracts;

public interface IOrderXmlService
{
    Task TrySerializeXmlAsync(Stream outputStream, XmlOrder xmlData);
    Task<OneOf<string, InvalidXmlResult>> TrySerializeXmlAsync(XmlOrder xmlData);
}
