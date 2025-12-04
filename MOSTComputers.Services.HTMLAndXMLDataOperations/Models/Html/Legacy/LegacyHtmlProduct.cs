using System.Xml.Serialization;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Html.Legacy;

[Serializable]
public sealed class LegacyHtmlProduct
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public List<LegacyHtmlProductProperty>? Properties { get; set; }
    public string? VendorUrl { get; set; }
    public string? VendorTitle { get; set; }
}