using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using System.Xml;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.Documents.InvoiceData;
public sealed class InvoiceXmlFullData : IXmlAsyncSerializable
{
    public List<XmlInvoice>? Invoices { get; set; }

    public bool ShouldDisplayInvoices()
    {
        return Invoices?.Count > 0;
    }

    public async Task WriteXmlAsync(XmlWriter writer, string rootElementName)
    {
        await writer.WriteStartElementAsync(null, rootElementName, null);

        if (ShouldDisplayInvoices())
        {
            foreach (XmlInvoice invoice in Invoices!)
            {
                await invoice.WriteXmlAsync(writer, "invoice");
            }
        }

        await writer.WriteEndElementAsync();
    }
}