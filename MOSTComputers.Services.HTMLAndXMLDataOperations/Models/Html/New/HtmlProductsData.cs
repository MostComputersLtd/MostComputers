using System.Xml.Serialization;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Html.New;

[Serializable]
[XmlRoot(ElementName = "data")]
public sealed class HtmlProductsData
{
    [XmlArray(ElementName = "productlist")]
    [XmlArrayItem(ElementName = "product")]
    public required List<HtmlProduct> Products { get; init; }
}