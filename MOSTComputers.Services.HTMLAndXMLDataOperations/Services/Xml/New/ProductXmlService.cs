using OneOf;
using System.Xml;
using System.Xml.Serialization;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.New.Contracts;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.ProductData;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.New;
public sealed class ProductXmlService : IProductXmlService
{
    public ProductXmlService(XmlSerializerFactory xmlSerializerFactory)
    {
        _xmlSerializer = xmlSerializerFactory.CreateSerializer(typeof(ProductsXmlFullData));
    }

    private readonly XmlSerializer _xmlSerializer;

    private const string _invalidXmlDefaultMessage = "Something went wrong";

    public ProductsXmlFullData? DeserializeProductsXml(string xml)
    {
        using StringReader reader = new(xml);

        object? dataAsObject = _xmlSerializer.Deserialize(reader);

        if (dataAsObject is null) return null;

        ProductsXmlFullData data = (ProductsXmlFullData)dataAsObject;

        return data;
    }

    public OneOf<ProductsXmlFullData?, InvalidXmlResult> TryDeserializeProductsXml(string xml)
    {
        try
        {
            using StringReader reader = new(xml);

            object? dataAsObject = _xmlSerializer.Deserialize(reader);

            if (dataAsObject is null) return null;

            ProductsXmlFullData data = (ProductsXmlFullData)dataAsObject;

            return data;
        }
        catch (InvalidOperationException invalidOperationEx)
        {
            if (invalidOperationEx.InnerException is null
                || invalidOperationEx.InnerException is not XmlException)
            {
                return new InvalidXmlResult() { Text = _invalidXmlDefaultMessage };
            }

            return new InvalidXmlResult() { Text = invalidOperationEx.InnerException?.Message };
        }
    }

    public async Task TrySerializeProductsXmlAsync(Stream outputStream, ProductsXmlFullData xmlObjectData)
    {
        XmlWriter? xmlWriter = null;

        try
        {
            xmlWriter = XmlWriter.Create(outputStream, new XmlWriterSettings { Async = true, Indent = true });

            await xmlObjectData.WriteXmlAsync(xmlWriter, "data");
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

    public async Task<OneOf<string, InvalidXmlResult>> TrySerializeProductsXmlAsync(ProductsXmlFullData xmlObjectData)
    {
        using StringWriter stringWriter = new();

        XmlWriter? xmlWriter = null;

        try
        {
            xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Async = true, Indent = true });

            await xmlObjectData.WriteXmlAsync(xmlWriter, "data");
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