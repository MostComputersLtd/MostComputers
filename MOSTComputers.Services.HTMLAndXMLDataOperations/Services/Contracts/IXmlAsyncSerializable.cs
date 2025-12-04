using System.Xml;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
internal interface IXmlAsyncSerializable
{
    Task WriteXmlAsync(XmlWriter writer, string rootElementName);
}