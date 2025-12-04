using System.Xml.Serialization;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Html.Legacy;

[Serializable]
public sealed class LegacyHtmlProductProperty
{
    public string? Name { get; init; }
    public string? Value { get; init; }
}