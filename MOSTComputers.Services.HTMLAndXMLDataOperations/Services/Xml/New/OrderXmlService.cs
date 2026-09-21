using System.Xml;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.Documents.OrderData;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.New.Contracts;
using OneOf;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.New;

internal sealed class OrderXmlService : IOrderXmlService
{
    private const string _invalidXmlDefaultMessage = "Something went wrong";

    public async Task TrySerializeXmlAsync(Stream outputStream, XmlOrder xmlData)
    {
        XmlWriter? xmlWriter = null;

        try
        {
            xmlWriter = XmlWriter.Create(outputStream, new XmlWriterSettings { Async = true, Indent = true });

            await xmlData.WriteXmlAsync(xmlWriter, "order");
        }
        catch (InvalidOperationException)
        {
            if (xmlWriter is not null)
            {
                string errorMessage = _invalidXmlDefaultMessage;

                await xmlWriter.WriteElementStringAsync(null, "Error", null, errorMessage);
            }
        }
        finally
        {
            if (xmlWriter is not null)
            {
                await xmlWriter.DisposeAsync();
            }
        }
    }

    public async Task<OneOf<string, InvalidXmlResult>> TrySerializeXmlAsync(XmlOrder xmlData)
    {
        using StringWriter stringWriter = new();

        XmlWriter? xmlWriter = null;

        try
        {
            xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Async = true, Indent = true });

            await xmlData.WriteXmlAsync(xmlWriter, "data");
        }
        catch (InvalidOperationException)
        {
            return new InvalidXmlResult() { Text = _invalidXmlDefaultMessage };
        }
        finally
        {
            if (xmlWriter is not null)
            {
                await xmlWriter.DisposeAsync();
            }
        }

        return stringWriter.ToString();
    }
}
