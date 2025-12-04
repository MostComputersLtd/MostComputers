using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.Documents.InvoiceData;
using OneOf;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.New.Contracts;
public interface IInvoiceXmlService
{
    Task<OneOf<string, InvalidXmlResult>> TrySerializeXmlAsync(InvoiceXmlFullData xmlData);
    Task TrySerializeXmlAsync(Stream outputStream, InvoiceXmlFullData xmlData);
}