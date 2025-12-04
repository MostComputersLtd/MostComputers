using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using System.Xml;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.Documents.WarrantyCardData;
public sealed class WarrantyCardXmlFullData : IXmlAsyncSerializable
{
    public List<XmlWarrantyCard>? WarrantyCards { get; set; }

    public bool ShouldDisplayWarrantyCards()
    {
        return WarrantyCards?.Count > 0;
    }

    public async Task WriteXmlAsync(XmlWriter writer, string rootElementName)
    {
        await writer.WriteStartElementAsync(null, rootElementName, null);

        if (ShouldDisplayWarrantyCards())
        {
            foreach (XmlWarrantyCard warrantyCard in WarrantyCards!)
            {
                await warrantyCard.WriteXmlAsync(writer, "warrantyCard");
            }
        }

        await writer.WriteEndElementAsync();
    }
}