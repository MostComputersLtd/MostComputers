using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using System.Xml;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.Sitemap;

public sealed class UrlEntry : IXmlAsyncSerializable
{
    public required string Loc { get; set; }
    public DateTime? LastMod { get; set; }
    public string? ChangeFreq { get; set; }
    public decimal? Priority { get; set; }

    public async Task WriteXmlAsync(XmlWriter writer, string rootElementName)
    {
        await writer.WriteStartElementAsync(null, "url", null);

        await writer.WriteElementStringAsync(null, "loc", null, Loc);
        
        if (LastMod != null)
        {
            await writer.WriteElementStringAsync(null, "lastmod", null, LastMod.Value.ToString("O"));
        }

        if (ChangeFreq != null)
        {
            await writer.WriteElementStringAsync(null, "changefreq", null, ChangeFreq);
        }

        if (Priority != null)
        {
            await writer.WriteElementStringAsync(null, "priority", null, Priority.Value.ToString());
        }

        await writer.WriteEndElementAsync();
    }
}
