using System.Xml.Serialization;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Html.New;

[Serializable]
public sealed class HtmlProductProperty
{
    [XmlAttribute(AttributeName = "name")]
    public string? Name { get; init; }

    [XmlAttribute(AttributeName = "order")]
    public string? Order { get; init; }

    [XmlText]
    public string? Value { get; init; }
}