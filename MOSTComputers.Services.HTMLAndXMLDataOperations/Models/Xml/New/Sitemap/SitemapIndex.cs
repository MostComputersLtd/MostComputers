using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using System.Xml;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.Sitemap;

public sealed class SitemapIndex : IXmlAsyncSerializable
{
    public string SitemapSchema { get; init; } = "http://www.sitemaps.org/schemas/sitemap/0.9";
    public List<SitemapIndexEntry> Sitemaps { get; set; } = new();

    public async Task WriteXmlAsync(XmlWriter writer, string rootElementName)
    {
        await writer.WriteStartElementAsync(null, "sitemapindex", SitemapSchema);

        foreach (SitemapIndexEntry sitemapEntry in Sitemaps)
        {
            await sitemapEntry.WriteXmlAsync(writer, "sitemap");
        }

        await writer.WriteEndElementAsync();
    }
}
