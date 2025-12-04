using System.Xml.Serialization;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Html.New;

[Serializable]
public sealed class HtmlProduct
{
    [XmlAttribute(AttributeName = "id")]
    public required int Id { get; init; }


    [XmlElement(ElementName = "name")]
    public string? Name { get; init; }

    [XmlArray(ElementName = "properties")]
    [XmlArrayItem(ElementName = "property")]
    public required List<HtmlProductProperty> Properties { get; init; }
}