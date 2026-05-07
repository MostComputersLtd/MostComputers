using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using System.Xml;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.Sitemap;

public sealed class Sitemap : IXmlAsyncSerializable
{
    public string SitemapSchema { get; init; } = "http://www.sitemaps.org/schemas/sitemap/0.9";
    public List<UrlEntry> UrlEntries { get; set; } = new();

    public async Task WriteXmlAsync(XmlWriter writer, string rootElementName)
    {
        await writer.WriteStartElementAsync(null, "urlset", SitemapSchema);

        foreach (UrlEntry urlEntry in UrlEntries)
        {
            await urlEntry.WriteXmlAsync(writer, "url");
        }

        await writer.WriteEndElementAsync();
    }
}
