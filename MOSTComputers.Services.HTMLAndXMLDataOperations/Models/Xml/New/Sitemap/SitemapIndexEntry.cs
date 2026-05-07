using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using System.Xml;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.Sitemap;

public sealed class SitemapIndexEntry : IXmlAsyncSerializable
{
    public required string Loc { get; set; }
    public DateTime? LastMod { get; set; }

    public async Task WriteXmlAsync(XmlWriter writer, string rootElementName)
    {
        await writer.WriteStartElementAsync(null, "sitemap", null);

        await writer.WriteElementStringAsync(null, "loc", null, Loc);
        
        if (LastMod != null)
        {
            await writer.WriteElementStringAsync(null, "lastmod", null, LastMod.Value.ToString("O"));
        }

        await writer.WriteEndElementAsync();
    }
}