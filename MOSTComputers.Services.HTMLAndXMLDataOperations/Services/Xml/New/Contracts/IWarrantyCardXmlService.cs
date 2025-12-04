using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.Documents.WarrantyCardData;
using OneOf;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.New.Contracts;
public interface IWarrantyCardXmlService
{
    Task TrySerializeXmlAsync(Stream outputStream, WarrantyCardXmlFullData xmlData);
    Task<OneOf<string, InvalidXmlResult>> TrySerializeXmlAsync(WarrantyCardXmlFullData xmlData);
}