using System.Xml;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
public interface IXmlAsyncSerializable
{
    Task WriteXmlAsync(XmlWriter writer, string rootElementName);
}